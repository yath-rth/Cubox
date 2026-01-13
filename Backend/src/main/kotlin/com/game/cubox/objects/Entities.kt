package com.game.cubox.objects

import kotlinx.serialization.Serializable
import org.springframework.web.socket.WebSocketSession

@Serializable
data class PlayerEntity (
    var state: PlayerState,
    var session: WebSocketSession,
    var position: Vector3,
    var rotation: Vector3,
    var inputState: Int,
    var shootInput: Int,
    var color: String,
    var health: Int,
    var fireRate: Float,
    var lastShootTIme: Float,
    var timer: Float,
    var ammo: Int,
    var isReloading: Int,
    var lastReloadTIme: Float
)

@Serializable
data class Bullet(
    var position: Vector3,
    var direction: Vector3,
    var owner: String,
    var lifetime: Float,
    var damage: Int
)

@Serializable
data class Enemy(
    var enemyState: EnemyState,
    var position: Vector3,
    var direction: Vector3,
    var damage: Int,
    var targetId: String,
    var health: Int
)
