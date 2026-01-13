public enum InputDir
{
    NONE,
    FRONT,
    BACK,
    RIGHT,
    LEFT,
    FRIGHT,
    FLEFT,
    BRIGHT,
    BLEFT
}

public enum InputType
{
    NONE, MOVE, SHOOT
}

public enum ServerMessageType
{
    WELCOME,
    UPDATE,
    PLAYER_JOIN,
    PLAYER_EXIT,
    EXIT
}

public enum EnemyState
{
    NONE,
    CHASING,
    ATTACK,
    DEAD
}