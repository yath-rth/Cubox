package com.game.cubox.objects

enum class ServerMessageType {
    WELCOME,
    UPDATE,
    PLAYER_JOIN,
    PLAYER_EXIT,
    EXIT
}

enum class InputType {
    NONE,
    MOVE,
    SHOOT
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