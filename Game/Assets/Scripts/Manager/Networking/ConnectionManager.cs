using UnityEngine;
using Unity;

using NativeWebSocket;
using UnityEngine.Events;
using System;
using UnityEditor.VersionControl;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;
    WebSocket ws;
    public String playerId { get; private set; }
    public UnityEvent<ServerMessage> onMessage;

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
            var jsonMsg = JsonUtility.FromJson<ServerMessage>(message);
            onMessage?.Invoke(jsonMsg);
        };

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendSnapShot", 0.0f, 0.3f);

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
        if (ws.State == WebSocketState.Open)
        {
            PlayerInputMessage pim = new PlayerInputMessage(playerId, input);
            await ws.SendText(JsonUtility.ToJson(pim).ToString());
        }
    }

    async void SendSnapShot()
    {
        if (ws.State == WebSocketState.Open)
        {

        }
    }

    private async void OnApplicationQuit()
    {
        await ws.Close();
    }
}