package com.game.cubox.config

import com.game.cubox.logic.EnemyManager
import com.game.cubox.logic.GameManager
import com.game.cubox.logic.PlayerManager
import com.game.cubox.logic.WorldManager
import jakarta.annotation.PostConstruct
import org.springframework.stereotype.Component
import java.util.concurrent.Executors
import java.util.concurrent.TimeUnit

@Component
class GameLoop(
    private val playerService: PlayerManager,
    private val worldManager: WorldManager,
    private val enemyManager: EnemyManager,
    private val gameManager: GameManager
) {

    @PostConstruct
    fun init() {
        start()
    }

    private companion object {
        const val TICKS_PER_SECOND = 20
        const val TICK_DURATION_MS = 1000L / TICKS_PER_SECOND
    }

    private val scheduler = Executors.newSingleThreadScheduledExecutor()

    fun start() {
        worldManager.createWorld()

        scheduler.scheduleAtFixedRate(
            { tick() },
            0,
            TICK_DURATION_MS,
            TimeUnit.MILLISECONDS
        )
    }

    private fun tick() {
        playerService.updatePosition()
        playerService.shoot()
        enemyManager.updateEnemies()
        gameManager.collisionCheck()
        gameManager.updateWorld()
    }
}
