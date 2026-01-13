package com.game.cubox.objects

import kotlinx.serialization.Serializable

@Serializable
class ServerMessage(
    var type: ServerMessageType,
    var id: String? = null,
    var mapSize: Map<String, Int>? = null,
    var players: Map<String, PlayerDTO>? = null,
    var bullets: Map<String, BulletDTO>? = null,
    var enemies: Map<String, EnemyDTO>? = null
) {
    constructor(
        players: Map<String, PlayerDTO>,
        bullets: Map<String, BulletDTO>,
        enemies: Map<String, EnemyDTO>?
    ) : this(type = ServerMessageType.UPDATE, players = players, bullets = bullets, enemies = enemies)
}

@Serializable
data class PlayerDTO(
    var position: Vector3,
    var rotation: Vector3,
    var color: String,
    var health: Int,
    var isReloading: Int
)

@Serializable
data class BulletDTO(
    var position: Vector3,
    var direction: Vector3,
    var lifetime: Float,
    var owner: String
)

@Serializable
data class EnemyDTO(
    var state: EnemyState,
    var position: Vector3,
    var direction: Vector3,
    var health: Int
)