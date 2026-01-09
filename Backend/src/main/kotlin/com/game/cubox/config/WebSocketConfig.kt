package com.game.cubox.config

import org.springframework.context.annotation.Configuration
import org.springframework.web.socket.config.annotation.EnableWebSocket
import org.springframework.web.socket.config.annotation.WebSocketConfigurer
import org.springframework.web.socket.config.annotation.WebSocketHandlerRegistry
import org.springframework.web.socket.handler.WebSocketHandlerDecoratorFactory

@Configuration
@EnableWebSocket
class WebSocketConfig(
    private val myHandler: MyTextHandler,
    private val handshakeInterceptor: HandshakeInterceptor,
) : WebSocketConfigurer {

    override fun registerWebSocketHandlers(registry: WebSocketHandlerRegistry) {
        registry.addHandler(myHandler, "/game")
            .setAllowedOrigins("*")
            .addInterceptors(handshakeInterceptor)
    }

}