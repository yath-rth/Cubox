package com.game.cubox.logic

import com.game.cubox.objects.*
import kotlinx.serialization.json.Json
import org.springframework.stereotype.Component
import org.springframework.web.socket.WebSocketSession
import java.sql.Time
import java.util.*

@Component
class PlayerManager(
    private val roomManager: RoomManager
) {

    private val SPEED = 1.2f
    private val SHOOTINGSPEED = .3f
    val PLAYERSIZE = 1.5f
    val BULLETSIZE = 0.01f
    private val BulletSpeed = 80f
    val deltaTime = 0.02f
    private val BulletDmg = 20
    val MAGSIZE = 20
    val RELOADTIME = .5f

    private val muzzleOffset: Vector3 = Vector3(.752f, 0f, 0.95f)

    fun addPlayer(id: String, session: WebSocketSession, room: Room) {
        val player =
            PlayerEntity(
                state = PlayerState.NONE,
                session = session,
                position = Vector3(0f, PLAYERSIZE, 0f),
                rotation = Vector3.Zero,
                inputState = 0,
                shootInput = 0,
                color = HelperFunctions.getRandomColor(),
                health = 100,
                fireRate = 0.05f,
                lastShootTIme = 0f,
                timer = 0f,
                ammo = MAGSIZE,
                isReloading = 0,
                lastReloadTIme = 0f,
                score = 0
            )
        room.players[id] = player

        val msg = ServerMessage(
            type = ServerMessageType.PLAYER_JOIN,
            players = room.players.toMap()
                .mapValues {
                    PlayerDTO(
                        it.value.position,
                        it.value.rotation,
                        it.value.color,
                        it.value.health,
                        it.value.isReloading,
                        it.value.score
                    )
                })

        if ((room.players.keys.size) > 1) {
            for (ids in room.players.keys) {
                val _player = room.players[ids] ?: continue

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

    fun removePlayer(id: String, room: Room) {
        room.players.remove(id)

        val msg = ServerMessage(
            type = ServerMessageType.PLAYER_EXIT,
            players = room.players.toMap()
                .mapValues {
                    PlayerDTO(
                        it.value.position,
                        it.value.rotation,
                        it.value.color,
                        it.value.health,
                        it.value.isReloading,
                        it.value.score
                    )
                })

        for (ids in room.players.keys) {
            val player = room.players[ids] ?: continue
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

    fun updateInputState(msg: ClientMessage, room: Room) {
        try {
            val player = room.players[msg.playerId]
            if (player != null) {
                if (msg.inputType == 1) player.inputState = msg.input ?: player.inputState
                else if (msg.inputType == 2) player.shootInput = msg.shootInput ?: player.shootInput

                player.rotation = msg.rotation ?: player.rotation

                if (player.shootInput == 1) player.state = PlayerState.SHOOTING
                else if (player.inputState != 0) player.state = PlayerState.MOVING
                else player.state = PlayerState.NONE
            }
        } catch (e: Exception) {
            println(e.message)
        }
    }

    fun updatePosition() {
        for(room in roomManager.rooms.values) {
            for (player in room.players.values) {
                if (player.state == PlayerState.MOVING) player.position += HelperFunctions.checkInput(player.inputState) * SPEED
                else if (player.state == PlayerState.SHOOTING) player.position += HelperFunctions.checkInput(player.inputState) * SHOOTINGSPEED
                player.timer += deltaTime

                //To check if players are out of bounds
                player.position = HelperFunctions.checkIfOutOfBounds(
                    player.position,
                    mapX = room.mapX * 1f,
                    mapY = room.mapY * 1f,
                    PLAYERSIZE
                )
            }

            for (id in room.bullets.keys) {
                val _bullet = room.bullets[id] ?: continue

                _bullet.position += _bullet.direction * deltaTime
                _bullet.lifetime -= deltaTime
                if (_bullet.lifetime <= 0) room.bulletsToRemove.add(id);
            }

            for (id in room.bulletsToRemove) {
                room.bullets.remove(id)
            }
            room.bulletsToRemove.clear()
        }
    }

    fun shoot() {
        for(room in roomManager.rooms.values) {
            for (id in room.players.keys) {
                val player = room.players[id] ?: continue
                if (player.isReloading == 1) {
                    if (player.lastReloadTIme < player.timer) {
                        player.ammo = MAGSIZE
                        player.isReloading = 0
                    } else continue
                }

                if (player.shootInput == 0) continue

                if (player.timer < player.lastShootTIme) continue
                player.lastShootTIme = player.timer + player.fireRate

                val muzzleWorldPos = Vector3.transformPoint(player.position, player.rotation, muzzleOffset)

                val bullet = Bullet(
                    position = muzzleWorldPos,
                    direction = Vector3.directionFromRotation(player.rotation * -1f) * BulletSpeed,
                    owner = id,
                    lifetime = 0.15f,
                    BulletDmg
                )

                room.bullets[UUID.randomUUID().toString().slice(0..5)] = bullet
                player.ammo--

                if (player.ammo <= 0 && player.isReloading == 0) {
                    player.isReloading = 1
                    player.lastReloadTIme = player.timer + RELOADTIME
                }
            }
        }
    }

    fun getClosestPlayer(position: Vector3, room: Room): String? {
        var distance = 9999f
        var _player: String? = null

        if (room.players.isNotEmpty()) _player = room.players.keys.first()

        for (id in room.players.keys) {
            val player = room.players[id] ?: continue

            if (distance > HelperFunctions.distance(player.position, position)) {
                distance = HelperFunctions.distance(player.position, position)
                _player = id
            }
        }

        return _player
    }
}