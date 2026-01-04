using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClientMessage
{
    public String playerId;
    public InputType? inputType = null;
    public InputDir? input = null;
    public RotationDTO? rotation = null;

    public ClientMessage(String id, InputType type, InputDir input)
    {
        playerId = id;
        this.inputType = type;
        this.input = input;
    }

    public ClientMessage(String id, Vector3 rotation)
    {
        playerId = id;
        this.rotation = new RotationDTO(rotation);
    }

    public ClientMessage(String id, InputType type, InputDir input, Vector3 rotation)
    {
        playerId = id;
        inputType = type;
        this.input = input;
        this.rotation = new RotationDTO(rotation);
    }
}

[Serializable]
public class ServerMessage
{
    public ServerMessageType type;
    public String id = null;
    public Dictionary<String, int> mapSize = null;
    public Dictionary<String, PlayerDTO> players = null;
    public Dictionary<String, BulletDTO> bullets = null;
}

public class PlayerDTO
{
    public Vector3 position;
    public Vector3 rotation;
    public string color;
}

public class BulletDTO
{
    public Vector3 position;
    public Vector3 direction;
    public float lifetime;
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