using VContainer.Unity;
using MessagePipe;
using Zombies.GameModes;
using Zombies.Levels;

namespace Zombies.UI
{
    public sealed class PagesController : IStartable, System.IDisposable
    {
        private readonly MapPage mapPage;
        private readonly LevelPreparationPage levelPreparationPage;
        private readonly PlayPage playPage;
        private readonly PausePage pausePage;
        private readonly VictoryPage victoryPage;
        private readonly DefeatPage defeatPage;
        private readonly MapController mapController;
        private readonly LevelController levelController;
        private readonly GameModeRequestService gameModeRequestService;
        private BasePage currentPage;

        public PagesController(MapPage mapPage, LevelPreparationPage levelPreparationPage, PlayPage playPage, PausePage pausePage, VictoryPage victoryPage,
            DefeatPage defeatPage,
            MapController mapController,
            LevelController levelController,
            GameModeRequestService gameModeRequestService, ISubscriber<GameModeChangedMessage> gameModeChanged,
            ISubscriber<LocationSelectedMessage> locationSelected)
        {
            this.mapPage = mapPage;
            this.levelPreparationPage = levelPreparationPage;
            this.playPage = playPage;
            this.pausePage = pausePage;
            this.victoryPage = victoryPage;
            this.defeatPage = defeatPage;
            this.mapController = mapController;
            this.levelController = levelController;
            this.gameModeRequestService = gameModeRequestService;
            levelPreparationPage.PlayRequested += ShowPlayPage;
            levelPreparationPage.MapRequested += ShowMap;
            playPage.PauseRequested += ShowPausePage;
            pausePage.ResumeRequested += ShowPlayPage;
            victoryPage.ContinueRequested += ShowLevelPreparationPage;
            defeatPage.ContinueRequested += ShowLevelPreparationPage;
            gameModeChanged.Subscribe(message =>
            {
                if (message.Mode == GameMode.Victory)
                    Show(PageType.Victory);
                else if (message.Mode == GameMode.Defeat)
                    Show(PageType.Defeat);
            });
            locationSelected.Subscribe(_ => ShowLevelPreparationPage());
        }

        public void Start() => Show(PageType.Map);

        public void Dispose()
        {
            levelPreparationPage.PlayRequested -= ShowPlayPage;
            levelPreparationPage.MapRequested -= ShowMap;
            playPage.PauseRequested -= ShowPausePage;
            pausePage.ResumeRequested -= ShowPlayPage;
            victoryPage.ContinueRequested -= ShowLevelPreparationPage;
            defeatPage.ContinueRequested -= ShowLevelPreparationPage;
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
                PageType.Map => mapPage,
                PageType.LevelPreparation => levelPreparationPage,
                PageType.Play => playPage,
                PageType.Pause => pausePage,
                PageType.Victory => victoryPage,
                PageType.Defeat => defeatPage,
                _ => mapPage
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

        private void ShowMap()
        {
            levelController.ClearCurrentLevel();
            mapController.ShowMap();
            Show(PageType.Map);
        }
    }
}
