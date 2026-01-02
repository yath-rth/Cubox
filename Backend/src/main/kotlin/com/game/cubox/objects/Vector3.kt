package com.game.cubox.config

import kotlinx.serialization.Serializable

@Serializable
data class Vector3(
    val x: Float,
    val y: Float,
    val z: Float
) {

    operator fun plus(vector3: Vector3): Vector3 {
        return Vector3(x + vector3.x, y + vector3.y, z + vector3.z)
    }

    operator fun times(a: Float): Vector3{
        return Vector3(x * a, y * a, z * a)
    }

    companion object {
        val Zero = Vector3(0f, 0f, 0f)
        val One  = Vector3(1f, 1f, 1f)
    }
}