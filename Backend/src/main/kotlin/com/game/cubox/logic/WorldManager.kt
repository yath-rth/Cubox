package com.game.cubox.logic

import com.game.cubox.objects.Vector3
import lombok.experimental.Helper
import org.springframework.stereotype.Component

@Component
class WorldManager {

    private var map: Map<String, Int> = emptyMap()
    var mapX: Int = 0
    var mapY: Int = 0

    fun createWorld(){
        val _map = mapOf(
            "X" to (22..29).random(),
            "Y" to (20..27).random(),
            "MaxX" to 29,
            "MaxY" to 27
        )

        mapX = (_map["X"] ?: 0)
        mapY = (_map["Y"] ?: 0)

        map = _map
    }

    fun getMap(): Map<String, Int> = map

    fun getRandomPosition(): Vector3 {
        return Vector3(
            x = ((-mapX .. mapX).random()) * 1f,
            y = 0.25f,
            z = ((-mapY .. mapY).random()) * 1f,
        )
    }
}

data class Cell(
    val x: Int,
    val y: Int
)