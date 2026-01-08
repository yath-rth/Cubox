package com.game.cubox.objects

import kotlinx.serialization.Serializable
import org.springframework.web.socket.WebSocketSession

@Serializable
data class PlayerEntity (
    var session: WebSocketSession,
    var position: Vector3,
    var rotation: Vector3,
    var inputState: Int,
    var shootInput: Int,
    var color: String,
    var health: Int
)

@Serializable
data class Bullet(
    var position: Vector3,
    var direction: Vector3,
    var owner: String,
    var lifetime: Float
)
