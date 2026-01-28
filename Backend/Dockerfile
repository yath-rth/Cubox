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

# 2. Copy the source code - try copying the whole directory to be safe
COPY . .

# 3. DEBUG: This will show us in the logs exactly what Docker sees
RUN ls -R src/main/kotlin

# 4. Run CLEAN and BOOTJAR
# We use --info to see exactly why tasks might be skipped
RUN ./gradlew clean bootJar --info --no-daemon


# ---- Run stage ----
FROM eclipse-temurin:21

WORKDIR /app

COPY --from=build /app/build/libs/cubox-0.0.1-SNAPSHOT.jar app.jar

EXPOSE 8080

CMD ["java", "-jar", "app.jar"]
