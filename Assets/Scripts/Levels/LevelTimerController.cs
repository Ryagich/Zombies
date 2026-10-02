using System;
using MessagePipe;
using VContainer.Unity;
using Zombies.GameModes;

namespace Zombies.Levels
{
    /// <summary>
    /// Runs the countdown for the currently selected level while gameplay is active.
    /// </summary>
    public sealed class LevelTimerController : IStartable, ITickable, IDisposable
    {
        private readonly LevelSelectionState selectionState;
        private readonly GameModeRequestService gameModeRequest;
        private readonly ISubscriber<GameModeChangedMessage> gameModeChanged;

        private IDisposable gameModeSubscription;
        private bool levelStarted;
        private bool isRunning;
        private float accumulatedSeconds;
        private int remainingSeconds;

        public event Action<int> TimeChanged;

        public int RemainingSeconds => remainingSeconds;

        public LevelTimerController(
            LevelSelectionState selectionState,
            GameModeRequestService gameModeRequest,
            ISubscriber<GameModeChangedMessage> gameModeChanged)
        {
            this.selectionState = selectionState;
            this.gameModeRequest = gameModeRequest;
            this.gameModeChanged = gameModeChanged;
        }

        public void Start()
        {
            gameModeSubscription = gameModeChanged.Subscribe(OnGameModeChanged);
            ResetTimer();
        }

        public void Dispose() => gameModeSubscription?.Dispose();

        public void Tick()
        {
            if (!isRunning)
                return;

            accumulatedSeconds += UnityEngine.Time.deltaTime;
            while (accumulatedSeconds >= 1f && isRunning)
            {
                accumulatedSeconds -= 1f;
                remainingSeconds = Math.Max(remainingSeconds - 1, 0);
                TimeChanged?.Invoke(remainingSeconds);

                if (remainingSeconds != 0)
                    continue;

                isRunning = false;
                gameModeRequest.Request(GameMode.Defeat);
            }
        }

        public static string FormatTime(int seconds)
        {
            seconds = Math.Max(seconds, 0);
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }

        private void OnGameModeChanged(GameModeChangedMessage message)
        {
            switch (message.Mode)
            {
                case GameMode.LevelPreview:
                    levelStarted = false;
                    isRunning = false;
                    ResetTimer();
                    break;
                case GameMode.Gameplay:
                    if (!levelStarted)
                    {
                        levelStarted = true;
                        ResetTimer();
                    }

                    isRunning = remainingSeconds > 0;
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }

        private void ResetTimer()
        {
            accumulatedSeconds = 0f;
            remainingSeconds = selectionState.CurrentLevel?.CompletionTimeSeconds
                ?? LevelDefinition.DefaultCompletionTimeSeconds;
            TimeChanged?.Invoke(remainingSeconds);
        }
    }
}
