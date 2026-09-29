using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Zombies.UI
{
    /// <summary>Shows the menu on player activity, then fades it after a short idle period.</summary>
    [DisallowMultipleComponent]
    public sealed class MenuPageVisibility : MonoBehaviour
    {
        private enum VisibilityPhase { Hidden, Restoring, Showing, Fading }

        [SerializeField, Min(0f)] private float fadeOutTime = 1f;
        [SerializeField, Min(0f)] private float visibleTime = 2f;
        [SerializeField, Min(0f)] private float alphaRestoreTime = 1f;

        private CanvasGroup canvasGroup;
        private float alpha;
        private float phaseStartAlpha;
        private float phaseTargetAlpha;
        private float phaseDuration;
        private float phaseElapsed;
        private VisibilityPhase phase;

        private void Awake()
        {
            if (!TryGetComponent(out canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            alpha = 0f;
            phase = VisibilityPhase.Hidden;
            ApplyAlpha();
        }

        private void Update()
        {
            if (HasPlayerActivity())
            {
                BeginVisibilitySequence();
            }

            UpdateVisibility(Time.unscaledDeltaTime);
            ApplyAlpha();
        }

        private static bool HasPlayerActivity()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }

            var mouse = Mouse.current;
            if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > 0f || mouse.scroll.ReadValue().sqrMagnitude > 0f))
            {
                return true;
            }

            foreach (var device in InputSystem.devices)
            {
                foreach (var control in device.allControls)
                {
                    if (control is ButtonControl { wasPressedThisFrame: true })
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void BeginVisibilitySequence()
        {
            if (phase == VisibilityPhase.Showing)
            {
                phaseElapsed = 0f;
                return;
            }

            var remainingDuration = alphaRestoreTime * Mathf.Clamp01(1f - alpha);
            if (remainingDuration <= 0f)
            {
                alpha = 1f;
                phaseDuration = visibleTime;
                phaseElapsed = 0f;
                phase = VisibilityPhase.Showing;
                return;
            }

            phaseStartAlpha = alpha;
            phaseTargetAlpha = 1f;
            phaseDuration = remainingDuration;
            phaseElapsed = 0f;
            phase = VisibilityPhase.Restoring;
        }

        private void UpdateVisibility(float deltaTime)
        {
            switch (phase)
            {
                case VisibilityPhase.Restoring:
                    alpha = AdvanceAlpha(deltaTime);
                    if (phaseElapsed < phaseDuration) return;
                    alpha = 1f;
                    phaseDuration = visibleTime;
                    phaseElapsed = 0f;
                    phase = VisibilityPhase.Showing;
                    return;
                case VisibilityPhase.Showing:
                    alpha = 1f;
                    phaseElapsed += deltaTime;
                    if (phaseElapsed < phaseDuration) return;
                    phaseStartAlpha = 1f;
                    phaseTargetAlpha = 0f;
                    phaseDuration = fadeOutTime;
                    phaseElapsed = 0f;
                    phase = VisibilityPhase.Fading;
                    return;
                case VisibilityPhase.Fading:
                    alpha = AdvanceAlpha(deltaTime);
                    if (phaseElapsed < phaseDuration) return;
                    alpha = 0f;
                    phaseDuration = 0f;
                    phaseElapsed = 0f;
                    phase = VisibilityPhase.Hidden;
                    return;
                default:
                    alpha = 0f;
                    return;
            }
        }

        private float AdvanceAlpha(float deltaTime)
        {
            if (phaseDuration <= 0f)
            {
                phaseElapsed = phaseDuration;
                return phaseTargetAlpha;
            }

            phaseElapsed = Mathf.Min(phaseElapsed + deltaTime, phaseDuration);
            return Mathf.Lerp(phaseStartAlpha, phaseTargetAlpha, phaseElapsed / phaseDuration);
        }

        private void ApplyAlpha()
        {
            canvasGroup.alpha = alpha;
            canvasGroup.interactable = alpha > 0f;
            canvasGroup.blocksRaycasts = alpha > 0f;
        }
    }
}
