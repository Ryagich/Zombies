using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Zombies.Levels;
using Zombies.Localization;

namespace Zombies.UI
{
    public sealed class MapPage : BasePage
    {
        private readonly UIConfig config;
        private readonly RectTransform canvasRect;
        private readonly IObjectResolver resolver;
        private readonly LevelCatalogConfig levelCatalog;
        private readonly ISubscriber<LocationHoverMessage> locationHoverSubscriber;
        private RectTransform pageRoot;
        private MapPageHolder holder;
        private System.IDisposable locationHoverSubscription;

        public override PageType Type => PageType.Map;

        public MapPage(
            UIConfig config,
            Canvas canvas,
            IObjectResolver resolver,
            LevelCatalogConfig levelCatalog,
            ISubscriber<LocationHoverMessage> locationHoverSubscriber)
        {
            this.config = config;
            canvasRect = canvas.GetComponent<RectTransform>();
            this.resolver = resolver;
            this.levelCatalog = levelCatalog;
            this.locationHoverSubscriber = locationHoverSubscriber;
        }

        public override void Show()
        {
            if (pageRoot != null)
                return;

            if (config.MapPagePrefab == null)
            {
                Debug.LogError("MapPagePrefab must be assigned in UIConfig.");
                return;
            }

            pageRoot = resolver.Instantiate(config.MapPagePrefab, canvasRect);
            pageRoot.name = config.MapPagePrefab.name;
            UiRaycastUtility.DisableNonInteractiveRaycasts(pageRoot);
            holder = pageRoot.GetComponent<MapPageHolder>();
            if (holder == null || holder.TitleText == null)
            {
                Debug.LogError("Map Page prefab must contain MapPageHolder with TitleText assigned.", pageRoot);
                return;
            }

            holder.TitleText.text = string.Empty;
            locationHoverSubscription = locationHoverSubscriber.Subscribe(RefreshTitle);
        }

        public override void Hide()
        {
            if (pageRoot == null)
                return;

            locationHoverSubscription?.Dispose();
            locationHoverSubscription = null;
            holder = null;
            Object.Destroy(pageRoot.gameObject);
            pageRoot = null;
        }

        private void RefreshTitle(LocationHoverMessage message)
        {
            if (holder?.TitleText == null)
                return;

            if (message.LocationIndex < 0 || levelCatalog?.Locations == null || message.LocationIndex >= levelCatalog.Locations.Count)
            {
                holder.TitleText.text = string.Empty;
                return;
            }

            var location = levelCatalog.Locations[message.LocationIndex];
            var levels = location?.Levels;
            var totalCount = levels?.Count ?? 0;
            var unlockedCount = 0;
            if (levels != null)
            {
                foreach (var level in levels)
                {
                    if (level?.IsUnlocked == true)
                        unlockedCount++;
                }
            }

            holder.TitleText.text = $"{location?.LocationName.GetLocalizedStringCached()}\n{unlockedCount}/{totalCount}";
        }
    }
}
