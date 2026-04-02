// State pattern - tracks the current game lifecycle phase.
public enum GameStateType
{
    Menu,
    Playing,
    Paused,
    GameOver
}

public static class GameState
{
    public static GameStateType Current { get; set; } = GameStateType.Menu;
}
