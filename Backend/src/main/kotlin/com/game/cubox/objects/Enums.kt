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