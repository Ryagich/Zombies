using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VContainer;

namespace Zombies.Loading
{
    public sealed class LoadSceneUI : MonoBehaviour
    {
        private static readonly string[] AnimationFrames = { "-", "/", "|", "\\" };

        [SerializeField] private TMP_Text animationText;
        [SerializeField] private TMP_Text progressText;

        private LoadSceneConfig config;
        private SceneLoadingService sceneLoadingService;
        private AsyncOperation loadOperation;
        private float animationTimer;
        private int animationFrameIndex;
        private bool readyToActivate;

        [Inject]
        public void Construct(LoadSceneConfig loadSceneConfig, SceneLoadingService loadingService)
        {
            config = loadSceneConfig;
            sceneLoadingService = loadingService;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            if (config == null || sceneLoadingService == null)
            {
                Debug.LogError("LoadSceneUI was not initialized by LoadSceneLifetimeScope.", this);
                return;
            }

            ResetUi();
            if (!sceneLoadingService.HasPendingRequest)
            {
                sceneLoadingService.PrepareDirectLoad(config.MenuSceneName, false);
            }

            StartCoroutine(StartLoadingAfterFirstFrame());
        }

        private IEnumerator StartLoadingAfterFirstFrame()
        {
            yield return null;
            loadOperation = SceneManager.LoadSceneAsync(sceneLoadingService.TargetSceneName);
            if (loadOperation != null)
            {
                loadOperation.allowSceneActivation = false;
            }
        }

        private void Update()
        {
            if (loadOperation == null)
            {
                return;
            }

            var progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            UpdateProgress(progress);
            if (progress < 1f)
            {
                UpdateAnimation();
                return;
            }

            if (!readyToActivate)
            {
                readyToActivate = true;
                if (animationText != null) animationText.text = string.Empty;
            }

            if (!sceneLoadingService.WaitForInputBeforeActivation || HasAnyInput())
            {
                sceneLoadingService.ClearRequest();
                loadOperation.allowSceneActivation = true;
                return;
            }

            UpdateReadyPrompt();
        }

        private void ResetUi()
        {
            if (animationText != null) animationText.text = AnimationFrames[0];
            if (progressText != null)
            {
                progressText.text = "0%";
                SetTextAlpha(progressText, 1f);
            }
        }

        private void UpdateProgress(float progress)
        {
            if (!readyToActivate && progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
                SetTextAlpha(progressText, 1f);
            }
        }

        private void UpdateAnimation()
        {
            if (animationText == null) return;
            animationTimer += Time.unscaledDeltaTime;
            if (animationTimer < config.AnimationFrameSeconds) return;
            animationTimer = 0f;
            animationFrameIndex = (animationFrameIndex + 1) % AnimationFrames.Length;
            animationText.text = AnimationFrames[animationFrameIndex];
        }

        private void UpdateReadyPrompt()
        {
            if (progressText == null) return;
            progressText.text = config.PressAnyKeyText.GetLocalizedString();
            var pulse = (Mathf.Sin(Time.unscaledTime * config.ReadyTextBlinkSpeed) + 1f) * 0.5f;
            SetTextAlpha(progressText, Mathf.Lerp(config.ReadyTextMinAlpha, config.ReadyTextMaxAlpha, pulse));
        }

        private static bool HasAnyInput()
        {
            return Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame
                   || Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame
                                                 || Mouse.current.rightButton.wasPressedThisFrame
                                                 || Mouse.current.middleButton.wasPressedThisFrame)
                   || Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;
        }

        private static void SetTextAlpha(TMP_Text text, float alpha)
        {
            var color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}
