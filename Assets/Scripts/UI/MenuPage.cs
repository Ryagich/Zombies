using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Zombies.UI
{
    public sealed class MenuPage : BasePage
    {
        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private RectTransform pageRoot;
        private Button playButton;

        public event System.Action PlayRequested;
        public override PageType Type => PageType.Menu;

        public MenuPage(UIConfig config, Canvas canvas, IObjectResolver resolver)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
        }

        public override void Show()
        {
            if (pageRoot != null)
            {
                return;
            }

            if (config.MenuPagePrefab == null)
            {
                Debug.LogError("MenuPagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.MenuPagePrefab, canvasRect);
            pageRoot.name = config.MenuPagePrefab.name;
            UiRaycastUtility.DisableNonInteractiveRaycasts(pageRoot);
            pageRoot.gameObject.AddComponent<MenuPageVisibility>();

            playButton = FindPlayButton(pageRoot);
            if (playButton == null)
            {
                Debug.LogError("Menu Page prefab must contain a Button named 'Play Button'.", pageRoot);
                return;
            }

            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        public override void Hide()
        {
            if (pageRoot == null)
            {
                return;
            }

            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayButtonClicked);
                playButton = null;
            }

            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void OnPlayButtonClicked() => PlayRequested?.Invoke();

        private static Button FindPlayButton(Component root)
        {
            foreach (var button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.name == "Play Button")
                {
                    return button;
                }
            }

            return null;
        }
    }
}
