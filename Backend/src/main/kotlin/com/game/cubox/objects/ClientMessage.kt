package com.game.cubox.objects

import kotlinx.serialization.Serializable

@Serializable
class ClientMessage(
    val playerId: String,
    val inputType: Int? = null,
    val shootInput: Int? = null,
    val input: Int? = null,
    val rotation: Vector3? = null
)