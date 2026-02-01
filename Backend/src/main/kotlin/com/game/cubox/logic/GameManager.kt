package com.game.cubox.logic

import com.game.cubox.objects.*
import kotlinx.serialization.json.Json
import org.springframework.stereotype.Component
import kotlin.math.floor
import kotlin.math.sqrt

@Component
class GameManager(
    private val playerManager: PlayerManager,
    private val enemyManager: EnemyManager,
    private val roomManager: RoomManager
) {
    fun updateWorld() {
        for (roomObj in roomManager.rooms) {
            val room = roomObj.value
            if (room.players.isEmpty()) return

            val msg =
                ServerMessage(
                    id = roomObj.key,
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
                        },
                    bullets = room.bullets.toMap()
                        .mapValues {
                            BulletDTO(
                                it.value.position,
                                it.value.direction,
                                it.value.lifetime,
                                it.value.owner
                            )
                        },
                    enemies = room.enemies.toMap()
                        .mapValues {
                            EnemyDTO(
                                it.value.enemyState,
                                it.value.position,
                                room.players[it.value.targetId]?.position ?: it.value.direction,
                                it.value.health
                            )
                        }
                )

            for (player in room.players.values) {
                if (!player.session.isOpen) continue
                HelperFunctions.safeSend(player.session, Json.encodeToString(ServerMessage.serializer(), msg))
            }
        }
    }

    fun collisionCheck() {
        for(room in roomManager.rooms.values) {
            for (bullet in room.bullets.values) {
                val cell = Cell(
                    floor(bullet.position.x / enemyManager.enemySize).toInt(),
                    floor(bullet.position.y / enemyManager.enemySize).toInt()
                )
                room.bulletGrid.computeIfAbsent(cell) { mutableListOf() }.add(bullet)
            }

            checkBulletCollision(room)

            for (id in room.bullets.keys) {
                val bullet = room.bullets[id] ?: continue

                //TODO: Implement bullet collision with enemies and breakable objects
                //instead of hardcoding a type of object can get damaged make a new list for like damageable objects
                //like the script you have in unity which gives any object health and option to break even if they have a movement script attached to them
                //ie a modular system which separates movement logic from health logic as static objects can also be broken and moving objects not

                // Uncomment this if u want friendly fire in the game
//            for (playerId in players.keys) {
//                val player = players[playerId] ?: continue
//                if (playerId == bullet.owner) continue
//
//                val hit = HelperFunctions.distance(bullet.position, player.position) <=
//                        (PLAYERSIZE + BULLETSIZE + 0.1f)
//
//                if (hit) {
//                    player.health -= BulletDmg
//                    bulletsToRemove.add(id)
//                    break   // stop checking this bullet against others
//                }
//            }

            }

            for (id in room.bulletsToRemove) {
                room.bullets.remove(id)
            }
            room.bulletsToRemove.clear()
        }
    }

    fun checkBulletCollision(room: Room) {
        val offsets = listOf(
            -1 to -1, 0 to -1, 1 to -1,
            -1 to 0, 0 to 0, 1 to 0,
            -1 to 1, 0 to 1, 1 to 1
        )

        for (id in room.bullets.keys) {
            val bullet = room.bullets[id] ?: continue

            val cx = floor(bullet.position.x / enemyManager.enemySize).toInt()
            val cz = floor(bullet.position.z / enemyManager.enemySize).toInt()

            for ((dx, dz) in offsets) {
                val list = room.grid[Cell(cx + dx, cz + dz)] ?: continue

                for (other in list) {
                    val delta = bullet.position - other.position
                    val distSq = Vector3.sqrMagnitude(delta)
                    val minDist = enemyManager.enemySize + playerManager.BULLETSIZE

                    if (distSq < minDist * minDist) {
                        other.health -= bullet.damage
                        other.lastPlayerThatDamaged = bullet.owner
                        room.bulletsToRemove.add(id)
                    }
                }
            }
        }
    }

}