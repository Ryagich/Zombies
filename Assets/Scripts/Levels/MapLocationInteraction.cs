using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zombies.Levels
{
    [DisallowMultipleComponent]
    public sealed class MapLocationInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private MapHolder holder;
        private int entryIndex;
        private Vector3 initialScale;
        private Tween scaleTween;

        public void Initialize(MapHolder mapHolder, int index)
        {
            holder = mapHolder;
            entryIndex = index;
            initialScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (holder == null)
                return;

            holder.SetHoveredLocation(entryIndex);
            var config = holder.InteractionConfig;
            if (config == null)
                return;

            scaleTween?.Kill();
            scaleTween = transform.DOScale(initialScale * config.HoverScale, config.HoverDuration)
                .SetEase(config.HoverEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            holder?.SetHoveredLocation(-1);
            if (holder?.InteractionConfig == null)
                return;

            scaleTween?.Kill();
            scaleTween = transform.DOScale(initialScale, holder.InteractionConfig.HoverDuration)
                .SetEase(holder.InteractionConfig.HoverEase);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left
                && holder != null
                && holder.IsLocationAvailable(entryIndex))
            {
                holder.SelectLocation(entryIndex);
            }
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            scaleTween = null;
            transform.localScale = initialScale;
        }
    }
}
