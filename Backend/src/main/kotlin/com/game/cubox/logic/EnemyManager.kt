package com.game.cubox.logic

import com.game.cubox.objects.Enemy
import com.game.cubox.objects.EnemyState
import com.game.cubox.objects.Room
import com.game.cubox.objects.Vector3
import lombok.experimental.Helper
import org.springframework.stereotype.Component
import java.util.*
import kotlin.math.floor
import kotlin.math.sqrt

@Component
class EnemyManager(
    private val worldManager: WorldManager,
    private val playerManager: PlayerManager,
    private val roomManager: RoomManager
) {
    //Enemy Movement Variables
    private val enemySpeed = 0.08f
    val enemySize = 0.75f

    fun updateEnemies() {
        for (room in roomManager.rooms.values) {
            room.grid.clear()

            if (room.players.isEmpty()) { //Return if no player is currently in the room
                room.enemies.clear()
                return
            }

            for (enemy in room.enemies.values) {
                val cell = Cell(
                    floor(enemy.position.x / enemySize).toInt(),
                    floor(enemy.position.z / enemySize).toInt()
                )

                room.grid.computeIfAbsent(cell) { mutableListOf() }.add(enemy)
            }

            if (room.timer > room.lastSpawnTime && room.players.isNotEmpty()) {
                room.lastSpawnTime = room.timer + room.timeBTWspawn;
                createEnemy(room)
            }

            for (id in room.enemies.keys) {
                val enemy = room.enemies[id] ?: continue
                val player = (room.players[enemy.targetId]
                    ?: room.players[playerManager.getClosestPlayer(enemy.position, room)]) ?: continue

                enemy.direction = player.position - enemy.position
                if (HelperFunctions.distance(
                        player.position,
                        enemy.position
                    ) > (enemySize + playerManager.PLAYERSIZE + 0.1f)
                ) {
                    enemy.position += enemy.direction * enemySpeed
                }

                if (enemy.health <= 0) {
                    if (room.players.keys.contains(enemy.lastPlayerThatDamaged)) {
                        val _player = room.players[enemy.lastPlayerThatDamaged]
                        if (_player != null) _player.score++
                    }
                    room.deadEnemies.add(id)
                }
            }

            resolveEnemyCollisions(room)

            for (id in room.deadEnemies) {
                if (!room.enemies.containsKey(id)) continue
                room.enemies.remove(id)
            }

            if (room.players.isNotEmpty()) room.timer += playerManager.deltaTime
        }
    }

    fun resolveEnemyCollisions(room: Room) {
        val offsets = listOf(
            -1 to -1, 0 to -1, 1 to -1,
            -1 to 0, 0 to 0, 1 to 0,
            -1 to 1, 0 to 1, 1 to 1
        )

        for (enemy in room.enemies.values) {

            val cx = floor(enemy.position.x / enemySize).toInt()
            val cz = floor(enemy.position.z / enemySize).toInt()

            for ((dx, dz) in offsets) {
                val list = room.grid[Cell(cx + dx, cz + dz)] ?: continue

                for (other in list) {
                    if (other === enemy) continue

                    val delta = enemy.position - other.position
                    val distSq = Vector3.sqrMagnitude(delta)
                    val minDist = enemySize + enemySize

                    if (distSq < minDist * minDist) {
                        val dist = sqrt(distSq)
                        val overlap = minDist - dist
                        val correction = delta / dist * (overlap / 2f)

                        enemy.position += Vector3(correction.x, 0f, correction.z)
                        other.position -= Vector3(correction.x, 0f, correction.z)
                    }
                }
            }


            enemy.position = HelperFunctions.checkIfOutOfBounds(
                enemy.position,
                room.mapX * 1f,
                room.mapY * 1f,
                enemySize
            )
        }
    }

    fun createEnemy(room: Room) {
        val spawnPosition = worldManager.getRandomPosition(room)
        val playerID = playerManager.getClosestPlayer(spawnPosition, room) ?: return
        val player = room.players[playerID] ?: return

        val enemy = Enemy(EnemyState.NONE, spawnPosition, player.position - spawnPosition, 5, playerID, 30, "")
        room.enemies[UUID.randomUUID().toString().slice(0..5)] = enemy
    }
}