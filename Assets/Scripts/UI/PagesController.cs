using VContainer.Unity;
using MessagePipe;
using Zombies.GameModes;

namespace Zombies.UI
{
    public sealed class PagesController : IStartable, System.IDisposable
    {
        private readonly LevelPreparationPage levelPreparationPage;
        private readonly PlayPage playPage;
        private readonly PausePage pausePage;
        private readonly VictoryPage victoryPage;
        private readonly GameModeRequestService gameModeRequestService;
        private BasePage currentPage;

        public PagesController(LevelPreparationPage levelPreparationPage, PlayPage playPage, PausePage pausePage, VictoryPage victoryPage,
            GameModeRequestService gameModeRequestService, ISubscriber<GameModeChangedMessage> gameModeChanged)
        {
            this.levelPreparationPage = levelPreparationPage;
            this.playPage = playPage;
            this.pausePage = pausePage;
            this.victoryPage = victoryPage;
            this.gameModeRequestService = gameModeRequestService;
            levelPreparationPage.PlayRequested += ShowPlayPage;
            playPage.PauseRequested += ShowPausePage;
            pausePage.ResumeRequested += ShowPlayPage;
            victoryPage.ContinueRequested += ShowLevelPreparationPage;
            gameModeChanged.Subscribe(message =>
            {
                if (message.Mode == GameMode.Victory)
                    Show(PageType.Victory);
            });
        }

        public void Start() => Show(PageType.LevelPreparation);

        public void Dispose()
        {
            levelPreparationPage.PlayRequested -= ShowPlayPage;
            playPage.PauseRequested -= ShowPausePage;
            pausePage.ResumeRequested -= ShowPlayPage;
            victoryPage.ContinueRequested -= ShowLevelPreparationPage;
            currentPage?.Hide();
        }

        public void Show(PageType type)
        {
            var nextPage = ResolvePage(type);
            if (nextPage == currentPage)
            {
                return;
            }

            currentPage?.Hide();
            currentPage = nextPage;
            currentPage.Show();
        }

        private BasePage ResolvePage(PageType type)
        {
            return type switch
            {
                PageType.LevelPreparation => levelPreparationPage,
                PageType.Play => playPage,
                PageType.Pause => pausePage,
                PageType.Victory => victoryPage,
                _ => levelPreparationPage
            };
        }

        private void ShowPlayPage()
        {
            Show(PageType.Play);
            gameModeRequestService.Request(GameMode.Gameplay);
        }

        private void ShowPausePage()
        {
            Show(PageType.Pause);
            gameModeRequestService.Request(GameMode.Pause);
        }

        private void ShowLevelPreparationPage()
        {
            Show(PageType.LevelPreparation);
            gameModeRequestService.Request(GameMode.LevelPreview);
        }
    }
}
