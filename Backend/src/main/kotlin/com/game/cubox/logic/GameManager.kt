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
) {

    private val bulletsToRemove: MutableList<String> = mutableListOf()
    private val bulletGrid = mutableMapOf<Cell, MutableList<Bullet>>()

    fun updateWorld() {
        if (playerManager.getPlayers().isEmpty()) return

        val msg =
            ServerMessage(
                players = playerManager.getPlayers().toMap()
                    .mapValues { PlayerDTO(it.value.position, it.value.rotation, it.value.color, it.value.health, it.value.isReloading) },
                bullets = playerManager.getBullets().toMap()
                    .mapValues { BulletDTO(it.value.position, it.value.direction, it.value.lifetime, it.value.owner) },
                enemies = enemyManager.getEnemies().toMap()
                    .mapValues { EnemyDTO(it.value.enemyState, it.value.position, playerManager.getPlayers()[it.value.targetId]?.position ?: it.value.direction, it.value.health) }
            )

        for (player in playerManager.getPlayers().values) {
            if (!player.session.isOpen) continue
            HelperFunctions.safeSend(player.session, Json.encodeToString(ServerMessage.serializer(), msg))
        }
    }

    fun collisionCheck() {
        for(bullet in playerManager.getBullets().values){
            val cell = Cell(
                floor(bullet.position.x / enemyManager.enemySize).toInt(),
                floor(bullet.position.y / enemyManager.enemySize).toInt()
            )
            bulletGrid.computeIfAbsent(cell) { mutableListOf() }.add(bullet)
        }

        checkBulletCollision()

        for (id in playerManager.getBullets().keys) {
            val bullet = playerManager.getBullets()[id] ?: continue

            //TODO: Implement bullet collision with enemies and breakable objects
            //instead of hardcoding a type of object can get damaged make a new list for like damageable objects
            //like the script you have in unity which gives any object health and option to break even if they have a movement script attached to them

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

        for (id in bulletsToRemove) {
            playerManager.getBullets().remove(id)
        }
        bulletsToRemove.clear()
    }

    fun checkBulletCollision(){
        val offsets = listOf(
            -1 to -1, 0 to -1, 1 to -1,
            -1 to 0, 0 to 0, 1 to 0,
            -1 to 1, 0 to 1, 1 to 1
        )

        for (id in playerManager.getBullets().keys) {
            val bullet = playerManager.getBullets()[id] ?: continue

            val cx = floor(bullet.position.x / enemyManager.enemySize).toInt()
            val cz = floor(bullet.position.z / enemyManager.enemySize).toInt()

            for ((dx, dz) in offsets) {
                val list = enemyManager.getGrid()[Cell(cx + dx, cz + dz)] ?: continue

                for (other in list) {
                    val delta = bullet.position - other.position
                    val distSq = Vector3.sqrMagnitude(delta)
                    val minDist = enemyManager.enemySize + playerManager.BULLETSIZE

                    if (distSq < minDist * minDist) {
                        other.health -= bullet.damage
                        bulletsToRemove.add(id)
                    }
                }
            }
        }
    }

}