using UnityEngine;
using VContainer;
using VContainer.Unity;
using Zombies.NPC;
using Zombies.Levels;
using System.Collections.Generic;

namespace Zombies.UI
{
    public sealed class PlayPage : BasePage
    {
        public event System.Action PauseRequested;

        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly HumansController humansController;
        private readonly LevelTimerController levelTimer;
        private readonly BrainController brains;
        private readonly ZombieSelectionController zombieSelection;
        private readonly ZombieStorage zombieStorage;
        private readonly List<ZombieCard> zombieCards = new();
        private RectTransform pageRoot;
        private PlayPageHolder holder;

        public override PageType Type => PageType.Play;

        public PlayPage(UIConfig config, Canvas canvas, IObjectResolver resolver, HumansController humansController, LevelTimerController levelTimer,
            BrainController brains, ZombieSelectionController zombieSelection, ZombieStorage zombieStorage)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
            this.humansController = humansController;
            this.levelTimer = levelTimer;
            this.brains = brains;
            this.zombieSelection = zombieSelection;
            this.zombieStorage = zombieStorage;
        }

        public override void Show()
        {
            if (pageRoot != null)
                return;

            if (config.PlayPagePrefab == null)
            {
                Debug.LogError("PlayPagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.PlayPagePrefab, canvasRect);
            pageRoot.name = config.PlayPagePrefab.name;
            UiRaycastUtility.DisableNonInteractiveRaycasts(pageRoot);
            holder = pageRoot.GetComponent<PlayPageHolder>();
            if (holder == null || holder.PauseButton == null || holder.TitleText == null || holder.TimeText == null ||
                holder.BrainCountText == null || holder.BrainRestoreFill == null || holder.ZombiesListContent == null)
            {
                Debug.LogError("Play Page prefab must contain PlayPageHolder with PauseButton assigned.", pageRoot);
                return;
            }

            holder.PauseButton.onClick.AddListener(OnPauseClicked);
            humansController.PeopleProgressChanged += RefreshPeopleTitle;
            levelTimer.TimeChanged += RefreshTime;
            brains.Changed += RefreshZombieUi;
            zombieSelection.Changed += RefreshZombieUi;
            RefreshPeopleTitle(humansController.DeadPeopleCount, humansController.TotalPeopleCount);
            RefreshTime(levelTimer.RemainingSeconds);
            BuildZombieCards();
            RefreshZombieUi();
        }

        public override void Hide()
        {
            if (pageRoot == null)
                return;

            if (holder != null && holder.PauseButton != null)
                holder.PauseButton.onClick.RemoveListener(OnPauseClicked);

            humansController.PeopleProgressChanged -= RefreshPeopleTitle;
            levelTimer.TimeChanged -= RefreshTime;
            brains.Changed -= RefreshZombieUi;
            zombieSelection.Changed -= RefreshZombieUi;
            zombieCards.Clear();

            holder = null;
            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void OnPauseClicked() => PauseRequested?.Invoke();

        private void RefreshPeopleTitle(int current, int total)
        {
            if (holder?.TitleText != null)
                holder.TitleText.text = $"{current} / {total}";
        }

        private void RefreshTime(int seconds)
        {
            if (holder?.TimeText != null)
                holder.TimeText.text = LevelTimerController.FormatTime(seconds);
        }

        private void BuildZombieCards()
        {
            if (config.ZombieCardPrefab == null || config.ZombieRowPrefab == null || zombieStorage == null)
            {
                Debug.LogError("UIConfig must assign ZombieCardPrefab and ZombieRowPrefab, and ZombieStorage must be assigned.");
                return;
            }

            for (var index = 0; index < zombieStorage.SortedConfigs.Count; index += 2)
            {
                var row = resolver.Instantiate(config.ZombieRowPrefab, holder.ZombiesListContent);
                for (var cardIndex = index; cardIndex < index + 2 && cardIndex < zombieStorage.SortedConfigs.Count; cardIndex++)
                {
                    var card = resolver.Instantiate(config.ZombieCardPrefab, row);
                    card.Bind(zombieStorage.SortedConfigs[cardIndex], zombieSelection.Select);
                    zombieCards.Add(card);
                }
            }
        }

        private void RefreshZombieUi()
        {
            if (holder == null)
                return;

            holder.BrainCountText.text = $"{brains.CurrentBrains} / {brains.MaxBrains}";
            holder.BrainRestoreFill.fillAmount = brains.RestoreProgress;
            foreach (var card in zombieCards)
            {
                if (card == null)
                    continue;
                card.Refresh(brains.CurrentBrains, zombieSelection.GetCooldownProgress(card.Config), card.Config == zombieSelection.SelectedConfig);
            }
        }
    }
}
