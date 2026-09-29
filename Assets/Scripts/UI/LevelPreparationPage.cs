using UnityEngine;
using VContainer;
using VContainer.Unity;
using Zombies.Levels;

namespace Zombies.UI
{
    public sealed class LevelPreparationPage : BasePage
    {
        public event System.Action PlayRequested;

        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly LevelSelectionState selectionState;
        private RectTransform pageRoot;
        private LevelPreparationPageHolder holder;

        public override PageType Type => PageType.LevelPreparation;

        public LevelPreparationPage(UIConfig config, Canvas canvas, IObjectResolver resolver, LevelSelectionState selectionState)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
            this.selectionState = selectionState;
        }

        public override void Show()
        {
            if (pageRoot != null)
            {
                return;
            }

            if (config.LevelPreparationPagePrefab == null)
            {
                Debug.LogError("LevelPreparationPagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.LevelPreparationPagePrefab, canvasRect);
            pageRoot.name = config.LevelPreparationPagePrefab.name;
            holder = pageRoot.GetComponent<LevelPreparationPageHolder>();
            if (holder == null)
            {
                Debug.LogError("Level Preparation Page prefab must contain LevelPreparationPageHolder.", pageRoot);
                return;
            }

            holder.LeftButton.onClick.AddListener(selectionState.SelectPrevious);
            holder.RightButton.onClick.AddListener(selectionState.SelectNext);
            holder.PlayButton.onClick.AddListener(OnPlayClicked);
            selectionState.LevelChanged += Refresh;
            Refresh(selectionState.CurrentLevel);
        }

        public override void Hide()
        {
            if (pageRoot == null)
            {
                return;
            }

            if (holder != null)
            {
                holder.LeftButton.onClick.RemoveListener(selectionState.SelectPrevious);
                holder.RightButton.onClick.RemoveListener(selectionState.SelectNext);
                holder.PlayButton.onClick.RemoveListener(OnPlayClicked);
                holder = null;
            }

            selectionState.LevelChanged -= Refresh;
            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void Refresh(LevelDefinition level)
        {
            if (holder == null)
            {
                return;
            }

            holder.TitleText.text = level?.LevelName.GetLocalizedString() ?? string.Empty;
            holder.PeopleText.text = level?.LevelPrefab?.GetComponent<LevelHumansRegistry>()?.PeopleCount.ToString() ?? "0";
            holder.LeftButton.interactable = selectionState.CanSelectPrevious;
            holder.RightButton.interactable = selectionState.CanSelectNext;
            var isUnlocked = level != null && level.IsUnlocked;
            holder.PlayButton.gameObject.SetActive(isUnlocked);
            holder.CloseImage.gameObject.SetActive(!isUnlocked);
        }

        private void OnPlayClicked() => PlayRequested?.Invoke();
    }
}
