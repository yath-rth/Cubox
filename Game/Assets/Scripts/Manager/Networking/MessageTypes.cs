using System;
using UnityEngine;

[Serializable]
public class PlayerInputMessage
{//Add a roomid string as well to allow multiple rooms together
    public String playerId;
    public InputType input;

    public PlayerInputMessage(String id, InputType input)
    {
        playerId = id;
        this.input = input;
    }
}

[Serializable]
public class ServerMessage
{
    public ServerMessageType type;
    public Vector3 position;
}