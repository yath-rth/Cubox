package com.game.cubox.logic

import com.game.cubox.objects.*
import kotlinx.serialization.json.Json
import org.springframework.stereotype.Component
import org.springframework.web.socket.WebSocketSession
import java.sql.Time
import java.util.*

@Component
class PlayerManager(
    private val worldManager: WorldManager,
//    private val enemyManager: EnemyManager
) {

    private val SPEED = 1f
    val PLAYERSIZE = 0.5f
    val BULLETSIZE = 0.01f
    private val BulletSpeed = 80f
    val deltaTime = 0.02f
    private val BulletDmg = 10

    private val muzzleOffset: Vector3 = Vector3(.752f, 0f, 0.95f)

    private val players: MutableMap<String, PlayerEntity> = mutableMapOf()
    private val bullets: MutableMap<String, Bullet> = mutableMapOf()
    private var enemies: Map<String, Enemy> = emptyMap()
    private val bulletsToRemove: MutableList<String> = mutableListOf()

    fun addPlayer(id: String, session: WebSocketSession) {
        val player =
            PlayerEntity(
                session,
                Vector3(0f, PLAYERSIZE, 0f),
                Vector3.Zero,
                0,
                0,
                HelperFunctions.getRandomColor(),
                100,
                0.05f,
                0f,
                0f
            )
        players[id] = player

        val msg = ServerMessage(
            type = ServerMessageType.PLAYER_JOIN,
            players = players.toMap()
                .mapValues { PlayerDTO(it.value.position, it.value.rotation, it.value.color, it.value.health) })

        if ((players.keys.size) > 1) {
            for (ids in players.keys) {
                val _player = players[ids] ?: continue

                if (id != ids && _player.session.isOpen) {
                    HelperFunctions.safeSend(
                        _player.session,
                        Json.encodeToString(
                            ServerMessage.serializer(), msg
                        )
                    )
                }
            }
        }
    }

    fun getPlayers() = players
    fun getBullets() = bullets

    fun removePlayer(id: String) {
        players.remove(id)

        val msg = ServerMessage(
            type = ServerMessageType.PLAYER_EXIT,
            players = players.toMap()
                .mapValues { PlayerDTO(it.value.position, it.value.rotation, it.value.color, it.value.health) })

        if ((players.keys.size) > 1) {
            for (ids in players.keys) {
                val player = players[ids] ?: continue

                if (id != ids && player.session.isOpen) {
                    HelperFunctions.safeSend(
                        player.session,
                        Json.encodeToString(
                            ServerMessage.serializer(), msg
                        )
                    )
                }
            }
        }
    }

    fun updateEnemies(enemies: Map<String, Enemy>) {
        this.enemies = enemies
    }

    fun updateInputState(msg: ClientMessage) {
        try {
            val player = players[msg.playerId]
            if (player != null) {
                if (msg.inputType == 1) player.inputState = msg.input ?: player.inputState
                else if (msg.inputType == 2) player.shootInput = msg.shootInput ?: player.shootInput

                player.rotation = msg.rotation ?: player.rotation
            }
        } catch (e: Exception) {
            println(e.message)
        }
    }

    fun updatePosition() {
        for (player in players.values) {
            player.position += HelperFunctions.checkInput(player.inputState) * SPEED
            player.timer += deltaTime

            //To check if players are out of bounds
            player.position = HelperFunctions.checkIfOutOfBounds(
                player.position,
                mapX = worldManager.mapX * 1f,
                mapY = worldManager.mapY * 1f,
                PLAYERSIZE
            )
        }

        for (id in bullets.keys) {
            val _bullet = bullets[id] ?: continue

            _bullet.position += _bullet.direction * deltaTime
            _bullet.lifetime -= deltaTime
            if (_bullet.lifetime <= 0) bulletsToRemove.add(id);
        }

        for (id in bulletsToRemove) {
            bullets.remove(id)
        }
        bulletsToRemove.clear()
    }

    fun shoot() {
        for (id in players.keys) {
            val player = players[id] ?: continue
            if (player.shootInput == 0) continue

            if(player.timer < player.lastShootTIme) continue
            player.lastShootTIme = player.timer + player.fireRate

            val muzzleWorldPos = Vector3.transformPoint(player.position, player.rotation, muzzleOffset)

            val bullet = Bullet(
                position = muzzleWorldPos,
                direction = Vector3.directionFromRotation(player.rotation * -1f) * BulletSpeed,
                owner = id,
                lifetime = 0.15f,
                BulletDmg
            )

            bullets[UUID.randomUUID().toString().slice(0..5)] = bullet
        }
    }

    fun getClosestPlayer(position: Vector3): String? {
        var distance = 9999f
        var _player: String? = null

        if (players.isNotEmpty()) _player = players.keys.first()

        for (id in players.keys) {
            val player = players[id] ?: continue

            if (distance > HelperFunctions.distance(player.position, position)) {
                distance = HelperFunctions.distance(player.position, position)
                _player = id
            }
        }

        return _player
    }

    fun getPlayer(id: String?): PlayerEntity? {
        if (!players.containsKey(id)) return null
        return players[id]
    }
}