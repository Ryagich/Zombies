using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Zombies.UI
{
    public sealed class PausePage : BasePage
    {
        public event System.Action ResumeRequested;

        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private RectTransform pageRoot;
        private PausePageHolder holder;

        public override PageType Type => PageType.Pause;

        public PausePage(UIConfig config, Canvas canvas, IObjectResolver resolver)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
        }

        public override void Show()
        {
            if (pageRoot != null)
                return;

            if (config.PausePagePrefab == null)
            {
                Debug.LogError("PausePagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.PausePagePrefab, canvasRect);
            pageRoot.name = config.PausePagePrefab.name;
            holder = pageRoot.GetComponent<PausePageHolder>();
            if (holder == null || holder.ResumeButton == null)
            {
                Debug.LogError("Pause Page prefab must contain PausePageHolder with ResumeButton assigned.", pageRoot);
                return;
            }

            holder.ResumeButton.onClick.AddListener(OnResumeClicked);
        }

        public override void Hide()
        {
            if (pageRoot == null)
                return;

            if (holder != null && holder.ResumeButton != null)
                holder.ResumeButton.onClick.RemoveListener(OnResumeClicked);

            holder = null;
            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void OnResumeClicked() => ResumeRequested?.Invoke();
    }
}
