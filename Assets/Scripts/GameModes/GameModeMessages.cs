namespace Zombies.GameModes
{
    public readonly struct ChangeGameModeRequest
    {
        public readonly GameMode Mode;
        public ChangeGameModeRequest(GameMode mode) => Mode = mode;
    }

    public readonly struct GameModeChangedMessage
    {
        public readonly GameMode Mode;
        public GameModeChangedMessage(GameMode mode) => Mode = mode;
    }

    public readonly struct PauseInputMessage { }
}
