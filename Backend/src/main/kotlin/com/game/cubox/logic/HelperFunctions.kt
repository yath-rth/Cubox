package com.game.cubox.logic

import com.game.cubox.objects.Vector3
import org.springframework.web.socket.TextMessage
import org.springframework.web.socket.WebSocketSession
import kotlin.math.sqrt
import kotlin.random.Random

class HelperFunctions {
    companion object {

        private val worldColors = listOf(
            "#C5C5C5",
            "#FFFFFF",
            "#4C799F",
            "#A1C3EE",
            "#EEBCA1",
            "#EEEDA1",
            "#CBA1EE",
            "#A1EEAC",
            "#A1B0EE",
            "#F8B47B",
            "#DAEEA1"
        )

        fun checkInput(msg: Int): Vector3 {
            return when (msg) {
                1 -> Vector3(0f, 0f, 1f)
                2 -> Vector3(0f, 0f, -1f)
                3 -> Vector3(1f, 0f, 0f)
                4 -> Vector3(-1f, 0f, 0f)
                5 -> Vector3(0.71f, 0f, 0.71f)
                6 -> Vector3(-0.71f, 0f, 0.71f)
                7 -> Vector3(0.71f, 0f, -0.71f)
                8 -> Vector3(-0.71f, 0f, -0.71f)
                else -> Vector3.Zero
            }
        }

        fun getRandomColor(): String {
            return worldColors[worldColors.indices.random()]
        }

        fun distance(a: Vector3, b: Vector3): Float {
            return sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z))
        }

        fun safeSend(session: WebSocketSession, msg: String) {
            synchronized(session) {
                session.sendMessage(TextMessage(msg))
            }
        }

        fun checkIfOutOfBounds(position: Vector3, mapX: Float, mapY: Float, PLAYERSIZE: Float): Vector3 {
            var newPos: Vector3 = position

            if (position.x + PLAYERSIZE > mapX) {
                newPos = Vector3(mapX - PLAYERSIZE, PLAYERSIZE, newPos.z)
            }
            if (position.x - PLAYERSIZE < -mapX) {
                newPos = Vector3(-mapX + PLAYERSIZE, PLAYERSIZE, newPos.z)
            }
            if (position.z + PLAYERSIZE > mapY) {
                newPos = Vector3(newPos.x, PLAYERSIZE, mapY - PLAYERSIZE)
            }
            if (position.z - PLAYERSIZE < -mapY) {
                newPos = Vector3(newPos.x, PLAYERSIZE, -mapY + PLAYERSIZE)
            }

            return newPos;
        }


    }
}