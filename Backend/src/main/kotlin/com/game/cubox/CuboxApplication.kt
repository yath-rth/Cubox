package com.game.cubox

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class CuboxApplication

fun main(args: Array<String>) {
	runApplication<CuboxApplication>(*args)
}
