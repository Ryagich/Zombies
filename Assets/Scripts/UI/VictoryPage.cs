using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Zombies.UI
{
    public sealed class VictoryPage : BasePage
    {
        public event System.Action ContinueRequested;

        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private RectTransform pageRoot;
        private PausePageHolder holder;

        public override PageType Type => PageType.Victory;

        public VictoryPage(UIConfig config, Canvas canvas, IObjectResolver resolver)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
        }

        public override void Show()
        {
            if (pageRoot != null)
                return;
            if (config.VictoryPagePrefab == null)
            {
                Debug.LogError("VictoryPagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.VictoryPagePrefab, canvasRect);
            pageRoot.name = config.VictoryPagePrefab.name;
            UiRaycastUtility.DisableNonInteractiveRaycasts(pageRoot);
            holder = pageRoot.GetComponent<PausePageHolder>();
            if (holder == null || holder.ResumeButton == null)
            {
                Debug.LogError("Victory Page prefab must contain a button holder.", pageRoot);
                return;
            }
            holder.ResumeButton.onClick.AddListener(OnContinueClicked);
        }

        public override void Hide()
        {
            if (pageRoot == null)
                return;
            if (holder != null && holder.ResumeButton != null)
                holder.ResumeButton.onClick.RemoveListener(OnContinueClicked);
            holder = null;
            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void OnContinueClicked() => ContinueRequested?.Invoke();
    }
}
