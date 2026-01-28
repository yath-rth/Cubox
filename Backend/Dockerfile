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

COPY . .
RUN ls -R src/main/kotlin
RUN chmod +x gradlew
RUN ./gradlew clean bootJar --no-daemon


# ---- Run stage ----
FROM eclipse-temurin:21

WORKDIR /app

COPY --from=build /app/build/libs/cubox-0.0.1-SNAPSHOT.jar renamed-app.jar

EXPOSE 8080

CMD ["java", "-jar", "renamed-app.jar"]
