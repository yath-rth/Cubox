package com.game.cubox.objects

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

    operator fun minus(vector3: Vector3): Vector3 {
        return Vector3(x - vector3.x, y - vector3.y, z - vector3.z)
    }

    operator fun times(a: Float): Vector3 {
        return Vector3(x * a, y * a, z * a)
    }

    operator fun div(a: Float): Vector3 {
        return Vector3(x / a, y / a, z / a)
    }


    companion object {
        val Zero = Vector3(0f, 0f, 0f)
        val One  = Vector3(1f, 1f, 1f)

        fun rotateVector(v: Vector3, rot: Vector3): Vector3 {
            val yaw = Math.toRadians(rot.y.toDouble())
            val sin = kotlin.math.sin(yaw)
            val cos = kotlin.math.cos(yaw)

            return Vector3(
                (v.x * cos - v.z * sin).toFloat(),
                v.y,
                (v.x * sin + v.z * cos).toFloat()
            )
        }

        fun directionFromRotation(rotation: Vector3): Vector3 {
            val r = Math.toRadians(-rotation.y.toDouble())
            val x = kotlin.math.sin(r).toFloat()
            val z = kotlin.math.cos(r).toFloat()
            return Vector3(x, 0f, z)
        }

        fun sqrMagnitude(vector3: Vector3): Float{
            return vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z;
        }
        
        private fun deg2rad(d: Float) = (d * Math.PI / 180.0).toFloat()

        fun transformPoint(position: Vector3, euler: Vector3, local: Vector3): Vector3 {

            val cx = kotlin.math.cos(deg2rad(euler.x))
            val sx = kotlin.math.sin(deg2rad(euler.x))

            val cy = kotlin.math.cos(deg2rad(euler.y))
            val sy = kotlin.math.sin(deg2rad(euler.y))

            val cz = kotlin.math.cos(deg2rad(euler.z))
            val sz = kotlin.math.sin(deg2rad(euler.z))

            // Rotation matrix for Unity's order Z * X * Y
            val m00 = cy * cz + sy * sx * sz
            val m01 = cz * sy * sx - cy * sz
            val m02 = cx * sy

            val m10 = cx * sz
            val m11 = cx * cz
            val m12 = -sx

            val m20 = cy * sx * sz - cz * sy
            val m21 = cy * cz * sx + sy * sz
            val m22 = cx * cy

            // rotate local offset
            val rx = m00 * local.x + m01 * local.y + m02 * local.z
            val ry = m10 * local.x + m11 * local.y + m12 * local.z
            val rz = m20 * local.x + m21 * local.y + m22 * local.z

            // add player position
            return Vector3(
                position.x + rx,
                position.y + ry,
                position.z + rz
            )
        }


    }
}