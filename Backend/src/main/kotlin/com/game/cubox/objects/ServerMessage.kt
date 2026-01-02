package com.game.cubox.Messages

import com.game.cubox.config.Vector3
import kotlinx.serialization.Serializable

@Serializable
class ServerMessage(
    var type: ServerMessageType,
    var position: Vector3 = Vector3.Zero
) {
    constructor(pos: Vector3): this(type = ServerMessageType.PLAYER, position = pos)
}