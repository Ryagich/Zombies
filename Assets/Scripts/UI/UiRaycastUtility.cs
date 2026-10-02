using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zombies.UI
{
    public static class UiRaycastUtility
    {
        /// <summary>Keeps raycasts only on graphics that are actual button targets.</summary>
        public static void DisableNonInteractiveRaycasts(Transform root)
        {
            var buttonTargets = new HashSet<Graphic>();
            foreach (var button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.targetGraphic != null)
                    buttonTargets.Add(button.targetGraphic);
            }

            foreach (var graphic in root.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = buttonTargets.Contains(graphic);
        }
    }
}
