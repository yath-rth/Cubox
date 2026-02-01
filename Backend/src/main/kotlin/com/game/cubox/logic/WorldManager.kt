package com.game.cubox.logic

import com.game.cubox.objects.Room
import com.game.cubox.objects.Vector3
import lombok.experimental.Helper
import org.springframework.stereotype.Component

@Component
class WorldManager {
    fun createWorld(room: Room) {
        val _map = mapOf(
            "X" to (22..29).random(),
            "Y" to (20..27).random(),
            "MaxX" to 29,
            "MaxY" to 27
        )

        room.mapX = (_map["X"] ?: 0)
        room.mapY = (_map["Y"] ?: 0)

        room.map = _map
    }

    fun getRandomPosition(room: Room): Vector3 {
        return Vector3(
            x = ((-room.mapX..room.mapX).random()) * 1f,
            y = 0.25f,
            z = ((-room.mapY..room.mapY).random()) * 1f,
        )
    }
}

data class Cell(
    val x: Int,
    val y: Int
)