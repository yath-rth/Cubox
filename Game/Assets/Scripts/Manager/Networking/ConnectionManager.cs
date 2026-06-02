using UnityEngine;
using NativeWebSocket;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data.Common;
using System.Linq;
using Unity.Mathematics;
using TMPro;

//The deployed server url
//wss://cubox.onrender.com/game

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;
    BulletManager bulletMan;
    EnemySpawner enemySpawn;
    WebSocket ws;
    public string url, playerId;
    public string roomId;
    [SerializeField] GameObject playerPrefab;
    Dictionary<string, PlayerNetworkObject> players = new Dictionary<string, PlayerNetworkObject>();
    bool hasPlayerId = false, hasJoined = false;

    [SerializeField] TMP_Text roomIdText;

    private void Awake()
    {
        Application.targetFrameRate = 165;

        if (instance != null) Destroy(this);
        instance = this;

        bulletMan = GetComponent<BulletManager>();
        enemySpawn = GetComponent<EnemySpawner>();
    }

    async void Start()
    {
        Connect();
    }

    public void UpdateRoomId(string _roomId)
    {
        roomId = _roomId;
    }

    public async void JoinRoom()
    {
        if (roomId.Length != 6) return;

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage msg = new ClientMessage(ClientMessageType.JOIN, roomId, playerId);
            await ws.SendText(JsonConvert.SerializeObject(msg, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));

            hasJoined = true;
        }
    }

    public async void CreateRoom()
    {
        if (ws.State == WebSocketState.Open)
        {
            ClientMessage msg = new ClientMessage(ClientMessageType.JOIN, "CreateRoom", playerId);
            await ws.SendText(JsonConvert.SerializeObject(msg, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));

            hasJoined = true;
        }
    }

    async void Connect()
    {
        ws = new WebSocket(url);

        ws.OnOpen += async () =>
        {
            Debug.Log("Connection open!");
        };

        ws.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        ws.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        ws.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            var jsonMsg = JsonConvert.DeserializeObject<ServerMessage>(message);

            Debug.Log(message);
            onMessageRecieve(jsonMsg);
        };

        // Keep sending messages at every 0.1s
        InvokeRepeating("SendRotation", 0.0f, 0.1f);

        // waiting for messages
        await ws.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif

        if (roomIdText != null)
        {
            roomIdText.text = roomId;
        }
    }

    public async void SendShootInput(InputType type, int shootInput)
    {
        if (!hasPlayerId) return;
        if (!hasJoined) return;

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage cm = new ClientMessage(roomId, playerId, type, shootInput, Player.playerInstance.transform.eulerAngles);
            await ws.SendText(JsonConvert.SerializeObject(cm, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }

    public async void SendInput(InputType type, InputDir input)
    {
        if (!hasPlayerId) return;
        if (!hasJoined) return;

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage pim = new ClientMessage(roomId, playerId, type, input, Player.playerInstance.transform.eulerAngles);
            await ws.SendText(JsonConvert.SerializeObject(pim, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }

    async void SendRotation()
    {
        if (!hasPlayerId) return;
        if (!hasJoined) return;

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage msg = new ClientMessage(roomId, playerId, Player.playerInstance.transform.eulerAngles);
            await ws.SendText(JsonConvert.SerializeObject(msg, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }

    private async void OnApplicationQuit()
    {
        await ws.Close();
    }

    void spawnPlayer(ServerMessage msg)
    {
        if (playerPrefab != null)
        {
            foreach (string id in msg.players.Keys)
            {
                if (!players.ContainsKey(id))
                {
                    GameObject obj = Instantiate(playerPrefab, msg.players[id].position, Quaternion.Euler(msg.players[id].rotation));
                    PlayerNetworkObject nObj = obj.GetComponent<PlayerNetworkObject>();
                    if (nObj != null)
                    {
                        nObj.SetUp(msg, id);
                        players[id] = nObj;
                    }
                    else Debug.LogError("Network object not found");
                }
            }
        }
    }

    void onMessageRecieve(ServerMessage msg)
    {
        if (msg.type == ServerMessageType.WELCOME && msg.id != null)
        {
            playerId = msg.id;
            players[playerId] = Player.playerInstance.GetComponent<PlayerNetworkObject>();
            hasPlayerId = true;

            if (msg.players != null) spawnPlayer(msg);
        }

        if (msg.type == ServerMessageType.JOINED && msg.id != null)
        {
            if (players[playerId] != null) players[playerId].SetUp(msg, playerId);
            if (msg.mapSize != null) grid.Grid.SetUpWorldGrid(msg.mapSize);
            if (msg.players != null) spawnPlayer(msg);
        }

        if (msg.type == ServerMessageType.PLAYER_JOIN && msg.players != null)
        {
            spawnPlayer(msg);
        }

        if (msg.players != null && msg.type == ServerMessageType.UPDATE)
        {
            if (roomId != msg.id) roomId = msg.id;
            if (players.Keys.ToList() != msg.players.Keys.ToList()) spawnPlayer(msg);

            foreach (string id in players.Keys)
            {
                if (!players.ContainsKey(id) || !msg.players.ContainsKey(id)) continue;

                if (id != playerId) players[id].UpdateTransforms(msg.players[id], true);
                else players[id].UpdateTransforms(msg.players[id], false);
            }

            bulletMan.updateBullets(msg, players);
            enemySpawn.updateEnemies(msg);
        }

        if (msg.type == ServerMessageType.PLAYER_EXIT)
        {
            if (msg.players == null) return;
            foreach (string id in players.Keys.ToList())
            {
                if (!msg.players.ContainsKey(id))
                {
                    PlayerNetworkObject obj = players[id];
                    players.Remove(id);
                    Debug.Log(obj.gameObject.name);
                    DestroyImmediate(obj.gameObject);
                }
            }
        }
    }
}