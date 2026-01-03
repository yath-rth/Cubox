using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClientMessage
{
    public String playerId;
    public InputType? input = null;
    public RotationDTO? rotation = null;

    public ClientMessage(String id, InputType input)
    {
        playerId = id;
        this.input = input;
    }

    public ClientMessage(String id, Vector3 rotation)
    {
        playerId = id;
        this.rotation = new RotationDTO(rotation);
    }
}

[Serializable]
public class ServerMessage
{
    public ServerMessageType type;
    public String id = null;
    public Dictionary<String, TransformDTO> players = null;
}

public class TransformDTO
{
    public Vector3 position;
    public Vector3 rotation;
}

public class RotationDTO
{
    public float x;
    public float y;
    public float z;

    public RotationDTO(Vector3 rot)
    {
        x = rot.x;
        y = rot.y;
        z = rot.z;
    }
}