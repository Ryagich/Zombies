using UnityEngine;
using VContainer;
using VContainer.Unity;
using Zombies.NPC;

namespace Zombies.UI
{
    public sealed class PlayPage : BasePage
    {
        public event System.Action PauseRequested;

        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly HumansController humansController;
        private RectTransform pageRoot;
        private PlayPageHolder holder;

        public override PageType Type => PageType.Play;

        public PlayPage(UIConfig config, Canvas canvas, IObjectResolver resolver, HumansController humansController)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
            this.humansController = humansController;
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
            holder = pageRoot.GetComponent<PlayPageHolder>();
            if (holder == null || holder.PauseButton == null || holder.TitleText == null)
            {
                Debug.LogError("Play Page prefab must contain PlayPageHolder with PauseButton assigned.", pageRoot);
                return;
            }

            holder.PauseButton.onClick.AddListener(OnPauseClicked);
            humansController.PeopleProgressChanged += RefreshPeopleTitle;
            RefreshPeopleTitle(humansController.DeadPeopleCount, humansController.TotalPeopleCount);
        }

        public override void Hide()
        {
            if (pageRoot == null)
                return;

            if (holder != null && holder.PauseButton != null)
                holder.PauseButton.onClick.RemoveListener(OnPauseClicked);

            humansController.PeopleProgressChanged -= RefreshPeopleTitle;

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
    }
}
