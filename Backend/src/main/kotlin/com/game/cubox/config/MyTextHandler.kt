package com.game.cubox.config

import com.game.cubox.logic.HelperFunctions
import com.game.cubox.logic.PlayerManager
import com.game.cubox.logic.WorldManager
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
    private val worldManager: WorldManager
) : TextWebSocketHandler() {

    override fun handleTextMessage(session: WebSocketSession, message: TextMessage) {
        super.handleTextMessage(session, message)
        //println(message.payload)
        val msg = Json.decodeFromString(ClientMessage.serializer(), message.payload)
        playerManager.updateInputState(msg)
    }

    override fun afterConnectionEstablished(session: WebSocketSession) {
        super.afterConnectionEstablished(session)

        val id = UUID.randomUUID().toString()
        session.attributes["playerId"] = id
        playerManager.addPlayer(id, session)

        val msg = ServerMessage(
            ServerMessageType.WELCOME,
            id = id,
            mapSize = worldManager.getMap(),
            players = playerManager.getPlayers().toMap()
                .mapValues { PlayerDTO(it.value.position, it.value.rotation, it.value.color, it.value.health) })
        HelperFunctions.safeSend(session, Json.encodeToString(ServerMessage.serializer(), msg))

        println("connection has been made for $session && $id")
    }

    override fun afterConnectionClosed(session: WebSocketSession, status: CloseStatus) {
        super.afterConnectionClosed(session, status)
        playerManager.removePlayer(id = session.attributes["playerId"].toString())
        println("connection has been lost for $session")
    }

}