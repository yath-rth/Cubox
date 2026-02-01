package com.game.cubox.logic

import com.game.cubox.objects.Room
import org.springframework.stereotype.Component

@Component
class RoomManager {
    val rooms: MutableMap<String, Room> = mutableMapOf()
}