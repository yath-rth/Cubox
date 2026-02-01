package com.game.cubox.config

import com.game.cubox.logic.*
import com.game.cubox.objects.*
import kotlinx.serialization.json.Json
import org.springframework.stereotype.Component
import org.springframework.web.socket.CloseStatus
import org.springframework.web.socket.TextMessage
import org.springframework.web.socket.WebSocketSession
import org.springframework.web.socket.handler.TextWebSocketHandler
import org.w3c.dom.Text
import java.util.*

@Component
class MyTextHandler(
    private val playerManager: PlayerManager,
    private val roomManager: RoomManager,
    private val worldManager: WorldManager
) : TextWebSocketHandler() {

    override fun handleTextMessage(session: WebSocketSession, message: TextMessage) {
        super.handleTextMessage(session, message)
        //println(message.payload)
        val msg = Json.decodeFromString(ClientMessage.serializer(), message.payload)

        when (msg.type) {
            0 -> {
                var room: Room? = null

                if (msg.roomId == "CreateRoom") {
                    val id = UUID.randomUUID().toString().slice(0..6).uppercase()
                    roomManager.rooms[id] = Room()
                    room = roomManager.rooms[id] ?: return
                    playerManager.addPlayer(
                        session.attributes["playerId"].toString(),
                        session,
                        room
                    ) //Use msg.playerid later on when a proper menu is implemented
                    session.attributes["roomId"] = msg.roomId
                    worldManager.createWorld(room)
                }

                if (roomManager.rooms.keys.contains(msg.roomId)) {
                    //Can add a limit for number of players

                    room = roomManager.rooms[msg.roomId] ?: return
                    if (room.status != RoomState.OPEN) return
                    if (room.players.keys.contains(msg.playerId)) return
                    playerManager.addPlayer(msg.playerId, session, room)
                    session.attributes["roomId"] = msg.roomId
                }

                if (room == null) return
                val _msg = ServerMessage(
                    ServerMessageType.JOINED,
                    msg.roomId,
                    room.map,
                    players = room.players.toMap()
                        .mapValues {
                            PlayerDTO(
                                it.value.position,
                                it.value.rotation,
                                it.value.color,
                                it.value.health,
                                it.value.isReloading,
                                it.value.score
                            )
                        },
                    bullets = room.bullets.toMap()
                        .mapValues {
                            BulletDTO(
                                it.value.position,
                                it.value.direction,
                                it.value.lifetime,
                                it.value.owner
                            )
                        },
                    enemies = room.enemies.toMap()
                        .mapValues {
                            EnemyDTO(
                                it.value.enemyState,
                                it.value.position,
                                room.players[it.value.targetId]?.position ?: it.value.direction,
                                it.value.health
                            )
                        }
                )
                HelperFunctions.safeSend(session, Json.encodeToString(ServerMessage.serializer(), _msg))
            }

            1 -> {
                val room = roomManager.rooms[msg.roomId] ?: return
                playerManager.updateInputState(msg, room)
            }

            2 -> TODO()
        }
    }

    override fun afterConnectionEstablished(session: WebSocketSession) {
        super.afterConnectionEstablished(session)

        val id = UUID.randomUUID().toString()
        session.attributes["playerId"] = id

        val msg = ServerMessage(
            ServerMessageType.WELCOME,
            id = id,
            mapSize = null,
            players = null
        )
        HelperFunctions.safeSend(session, Json.encodeToString(ServerMessage.serializer(), msg))
    }

    override fun afterConnectionClosed(session: WebSocketSession, status: CloseStatus) {
        super.afterConnectionClosed(session, status)
        val room = roomManager.rooms[session.attributes["roomId"].toString()] ?: return
        println("removing player ${session.attributes["playerId"].toString()}")
        playerManager.removePlayer(session.attributes["playerId"].toString(), room)
    }

}