using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClientMessage
{
    public ClientMessageType type;
    public String roomId;
    public string playerId;
    public InputType? inputType = null;
    public int? shootInput = null;
    public InputDir? input = null;
    public RotationDTO? rotation = null;

    public ClientMessage(string id, InputType type, InputDir input)
    {
        this.type = ClientMessageType.UPDATE;
        playerId = id;
        inputType = type;
        this.input = input;
    }

    public ClientMessage(ClientMessageType type, string roomId, string id)
    {
        this.type = type;
        this.roomId = roomId;
        this.playerId = id;
    }

    public ClientMessage(string roomId, string id, Vector3 rotation)
    {
        this.type = ClientMessageType.UPDATE;
        this.roomId = roomId;
        playerId = id;
        this.rotation = new RotationDTO(rotation);
    }

    public ClientMessage(string roomId, string id, InputType type, int shootInput, Vector3 rotation)
    {
        this.type = ClientMessageType.UPDATE;
        this.roomId = roomId;
        playerId = id;
        inputType = type;
        this.shootInput = shootInput;
        this.rotation = new RotationDTO(rotation);
    }

    public ClientMessage(string roomId, string id, InputType type, InputDir input, Vector3 rotation)
    {
        this.type = ClientMessageType.UPDATE;
        this.roomId = roomId;
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
    public string id = null;
    public Dictionary<string, int> mapSize = null;
    public Dictionary<string, PlayerDTO> players = null;
    public Dictionary<string, BulletDTO> bullets = null;
    public Dictionary<string, EnemyDTO> enemies = null;
}

public class PlayerDTO
{
    public Vector3 position;
    public Vector3 rotation;
    public string color;
    public int health;
    public int isReloading;
    public int score;
}

public class BulletDTO
{
    public Vector3 position;
    public Vector3 direction;
    public float lifetime;
    public string owner;
}

public class EnemyDTO
{
    public EnemyState state;
    public Vector3 position;
    public Vector3 direction;
    public int health;
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