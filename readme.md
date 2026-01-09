## CUBOX

---

#### A multiplayer 3D top down shooter

---

### TECH:

- Spring Boot with Kotlin
- Unity Engine

---

### BASIC LOGIC:
The players join a room and communicate with the server using websockets. 
The room data is then stored in a database. This is uses a server authoritative model in which the server decides
if the sent move is valid and should be processed eliminating wrong moves from being registered. This model also reduces
the processing required in the frontend of the project.

---

### FUTURE GOALS:
- Implement proper game finding logic i.e. random match-making as well
- Add JWT - authentication and Google oauth
- Store player match history and add a levels system