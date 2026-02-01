package com.game.cubox.objects

import com.game.cubox.logic.Cell
import kotlinx.serialization.Serializable
import org.springframework.web.socket.WebSocketSession
import java.util.UUID

@Serializable
data class PlayerEntity(
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
    var lastReloadTIme: Float,
    var score: Int
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
    var health: Int,
    var lastPlayerThatDamaged: String,
)


data class Room(
    var status: RoomState = RoomState.OPEN,

    var map: Map<String, Int> = emptyMap(),
    var mapX: Int = 0,
    var mapY: Int = 0,

    val bulletsToRemove: MutableList<String> = mutableListOf(),
    val bulletGrid: MutableMap<Cell, MutableList<Bullet>> = mutableMapOf(),

    val players: MutableMap<String, PlayerEntity> = mutableMapOf(),
    val bullets: MutableMap<String, Bullet> = mutableMapOf(),

    var enemies: MutableMap<String, Enemy> = mutableMapOf(),
    val deadEnemies: MutableList<String> = mutableListOf(),
    val grid: MutableMap<Cell, MutableList<Enemy>> = mutableMapOf(),
    var timer: Float = 0f,
    val timeBTWspawn: Float = 0.5f,
    var lastSpawnTime: Float = 0f,
)
