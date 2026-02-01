package com.game.cubox.objects

enum class ServerMessageType {
    WELCOME,
    JOINED,
    UPDATE,
    PLAYER_JOIN,
    PLAYER_EXIT,
    EXIT
}

enum class RoomState{
    NONE,
    OPEN,
    PLAYING,
    CLOSED
}

enum class EnemyState{
    NONE,
    CHASING,
    ATTACK,
    DEAD
}

enum class PlayerState{
    NONE,
    MOVING,
    SHOOTING
}