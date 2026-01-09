package com.game.cubox.logic

import com.game.cubox.objects.Enemy
import com.game.cubox.objects.Vector3
import lombok.experimental.Helper
import org.springframework.stereotype.Component
import java.util.*
import kotlin.math.floor
import kotlin.math.sqrt

@Component
class EnemyManager(
    private val worldManager: WorldManager,
    private val playerManager: PlayerManager
) {

    private val enemies: MutableMap<String, Enemy> = mutableMapOf()
    private val deadEnemies: MutableList<String> = mutableListOf()
    private val grid = mutableMapOf<Cell, MutableList<Enemy>>()

    //Enemy Movement Variables
    private val enemySpeed = 0.1f
    val enemySize = 0.75f

    //Variables to spawn
    private var timer = 0f
    private val timeBTWspawn = 5f
    private var lastSpawnTime = 0f

    fun getEnemies() = enemies
    fun getGrid() = grid

    fun updateEnemies() {
        grid.clear()

        for (enemy in enemies.values) {
            val cell = Cell(
                floor(enemy.position.x / enemySize).toInt(),
                floor(enemy.position.z / enemySize).toInt()
            )

            grid.computeIfAbsent(cell) { mutableListOf() }.add(enemy)
        }

        if (timer > lastSpawnTime && playerManager.getPlayers().isNotEmpty()) {
            lastSpawnTime = timer + timeBTWspawn;
            createEnemy()
        }

        for (id in enemies.keys) {
            val enemy = enemies[id] ?: continue
            val player = (playerManager.getPlayer(enemy.targetId)
                ?: playerManager.getPlayer(playerManager.getClosestPlayer(enemy.position))) ?: continue

            enemy.direction = player.position - enemy.position
            if(HelperFunctions.distance(player.position, enemy.position) > (enemySize + playerManager.PLAYERSIZE + 0.1f)) enemy.position += enemy.direction * enemySpeed

            if (enemy.health <= 0) {
                deadEnemies.add(id)
            }
        }

        resolveEnemyCollisions()

        for (id in deadEnemies) {
            if (!enemies.containsKey(id)) continue
            enemies.remove(id)
        }

        if (playerManager.getPlayers().isNotEmpty()) timer += playerManager.deltaTime;
        playerManager.updateEnemies(enemies)
    }

    fun resolveEnemyCollisions() {
        val offsets = listOf(
            -1 to -1, 0 to -1, 1 to -1,
            -1 to 0, 0 to 0, 1 to 0,
            -1 to 1, 0 to 1, 1 to 1
        )

        for (enemy in enemies.values) {

            val cx = floor(enemy.position.x / enemySize).toInt()
            val cz = floor(enemy.position.z / enemySize).toInt()

            for ((dx, dz) in offsets) {
                val list = grid[Cell(cx + dx, cz + dz)] ?: continue

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
                worldManager.mapX * 1f,
                worldManager.mapY * 1f,
                enemySize
            )
        }
    }

    fun createEnemy() {
        val spawnPosition = worldManager.getRandomPosition()
        val playerID = playerManager.getClosestPlayer(spawnPosition) ?: return
        val player = playerManager.getPlayer(playerID) ?: return

        val enemy = Enemy(spawnPosition, player.position - spawnPosition, 5, playerID, 30)
        enemies[UUID.randomUUID().toString().slice(0..5)] = enemy
    }
}