package com.game.cubox.Messages

import kotlinx.serialization.Serializable

@Serializable
class ClientMessage(
    val playerId: String,
    val input: Int
)