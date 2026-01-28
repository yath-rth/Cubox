# ---- Build stage ----
FROM eclipse-temurin:21 AS build

WORKDIR /app

# Copy gradle files
COPY gradlew .
COPY gradle gradle
COPY build.gradle.kts settings.gradle.kts ./

# Download dependencies
RUN chmod +x gradlew
RUN ./gradlew dependencies

# Copy source code
COPY src src

# Build the boot jar
RUN ./gradlew bootJar


# ---- Run stage ----
FROM eclipse-temurin:21

WORKDIR /app

COPY --from=build /app/build/libs/cubox-0.0.1-SNAPSHOT.jar app.jar

EXPOSE 8080

CMD ["java", "-jar", "app.jar"]
