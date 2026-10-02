using VContainer.Unity;
using Zombies.GameModes;
using Zombies.Levels;

namespace Zombies.NPC
{
    public sealed class LevelCompletionController : IStartable, System.IDisposable
    {
        private readonly HumansController humans;
        private readonly LevelSelectionState levelSelection;
        private readonly GameModeRequestService gameModeRequest;

        public LevelCompletionController(HumansController humans, LevelSelectionState levelSelection, GameModeRequestService gameModeRequest)
        {
            this.humans = humans;
            this.levelSelection = levelSelection;
            this.gameModeRequest = gameModeRequest;
        }

        public void Start() => humans.AllPeopleDied += CompleteLevel;
        public void Dispose() => humans.AllPeopleDied -= CompleteLevel;

        private void CompleteLevel()
        {
            gameModeRequest.Request(GameMode.Victory);
            levelSelection.CompleteCurrentLevel();
        }
    }
}
