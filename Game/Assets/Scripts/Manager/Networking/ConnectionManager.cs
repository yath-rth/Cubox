using UnityEngine;
using NativeWebSocket;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data.Common;


public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;
    WebSocket ws;
    public String playerId { get; private set; }
    [SerializeField] GameObject playerPrefab;
    Dictionary<string, PlayerNetworkObject> players = new Dictionary<string, PlayerNetworkObject>();

    bool hasPlayerId = false;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }

    async void Start()
    {
        ws = new WebSocket("ws://localhost:8080/game");

        ws.OnOpen += () =>
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

            //Debug.Log(message);
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
    }

    public async void SendInput(InputType input)
    {
        if (!hasPlayerId) return;

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage pim = new ClientMessage(playerId, input);
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

        if (ws.State == WebSocketState.Open)
        {
            ClientMessage msg = new ClientMessage(playerId, Player.playerInstance.transform.eulerAngles);
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
                    if(nObj != null) players[id] = nObj;
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

            if(msg.players != null) spawnPlayer(msg);
        }

        if (msg.type == ServerMessageType.PLAYER_JOIN && msg.players != null)
        {
            spawnPlayer(msg);
        }

        if (msg.players != null && msg.type == ServerMessageType.UPDATE)
        {
            foreach (string id in players.Keys)
            {
                if (id != playerId) players[id].UpdateTransforms(msg.players[id]);
                else players[id].UpdatePosition(msg.players[id].position);
            }
        }
    }
}