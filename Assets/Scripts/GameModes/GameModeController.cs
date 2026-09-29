using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Zombies.GameModes
{
    public sealed class GameModeController : IStartable
    {
        private readonly IPublisher<GameModeChangedMessage> changedPublisher;
        private readonly GameModeRequestService requestService;

        public GameMode Current { get; private set; } = GameMode.LevelPreview;

        public GameModeController(
            IPublisher<GameModeChangedMessage> changedPublisher,
            ISubscriber<ChangeGameModeRequest> requestSubscriber,
            ISubscriber<PauseInputMessage> pauseSubscriber,
            GameModeRequestService requestService)
        {
            this.changedPublisher = changedPublisher;
            this.requestService = requestService;
            requestSubscriber.Subscribe(request => SetMode(request.Mode));
            pauseSubscriber.Subscribe(_ => TogglePause());
            requestService.Requested += mode => SetMode(mode);
        }

        public void Start() => SetMode(GameMode.LevelPreview, force: true);

        private void TogglePause() => SetMode(Current == GameMode.Gameplay ? GameMode.Pause : GameMode.Gameplay);

        private void SetMode(GameMode mode, bool force = false)
        {
            if (!force && mode == Current)
            {
                return;
            }

            Current = mode;
            var gameplay = mode != GameMode.Pause && mode != GameMode.Victory;
            Time.timeScale = gameplay ? 1f : 0f;
            changedPublisher.Publish(new GameModeChangedMessage(mode));
        }
    }
}
