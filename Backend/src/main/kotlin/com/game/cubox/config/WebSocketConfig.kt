package com.game.cubox

import org.springframework.web.socket.config.annotation.WebSocketConfigurer
import org.springframework.web.socket.config.annotation.WebSocketHandlerRegistry
import org.springframework.web.socket.handler.TextWebSocketHandler
import org.springframework.web.socket.server.HandshakeInterceptor

class WebSocketConfig(
    private val myHandler: myTextHandler,
    private val handshakeInterceptor: handshakeInterceptor
): WebSocketConfigurer {

    override fun registerWebSocketHandlers(registry: WebSocketHandlerRegistry) {
        registry.addHandler(myHandler, "")
            .setAllowedOrigins("*")
            .addInterceptors(handshakeInterceptor)
    }

}