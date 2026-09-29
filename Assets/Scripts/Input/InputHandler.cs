using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
using Zombies.GameModes;

namespace Zombies.Input
{
    public sealed class InputHandler : IStartable, ITickable, System.IDisposable
    {
        private readonly InputConfig config;
        private readonly CameraMovementConfig cameraMovementConfig;
        private readonly IPublisher<CameraEdgeMoveMessage> edgeMovePublisher;
        private readonly IPublisher<CameraDragMoveMessage> dragMovePublisher;
        private readonly IPublisher<PauseInputMessage> pausePublisher;
        private readonly IPublisher<GroundClickedMessage> groundClickPublisher;
        private readonly IPublisher<CameraKeyboardMoveMessage> keyboardMovePublisher;
        private readonly Camera camera;
        private readonly GameModeController gameModeController;

        private InputActionMap actionMap;
        private InputAction pauseAction;
        private bool wasDragging;
        private Vector2 lastPointerPosition;

        public InputHandler(
            InputConfig config,
            CameraMovementConfig cameraMovementConfig,
            IPublisher<CameraEdgeMoveMessage> edgeMovePublisher,
            IPublisher<CameraDragMoveMessage> dragMovePublisher,
            IPublisher<PauseInputMessage> pausePublisher,
            IPublisher<GroundClickedMessage> groundClickPublisher,
            IPublisher<CameraKeyboardMoveMessage> keyboardMovePublisher,
            Camera camera,
            GameModeController gameModeController)
        {
            this.config = config;
            this.cameraMovementConfig = cameraMovementConfig;
            this.edgeMovePublisher = edgeMovePublisher;
            this.dragMovePublisher = dragMovePublisher;
            this.pausePublisher = pausePublisher;
            this.groundClickPublisher = groundClickPublisher;
            this.keyboardMovePublisher = keyboardMovePublisher;
            this.camera = camera;
            this.gameModeController = gameModeController;
        }

        public void Start()
        {
            if (!HasRequiredActions())
            {
                Debug.LogError("InputConfig must define PointerPosition, PointerDelta, CameraDrag, and Pause.");
                return;
            }

            actionMap = config.PointerPosition.action.actionMap;
            actionMap.Enable();
            pauseAction = config.Pause.action;
            pauseAction.started += OnPause;
            config.PointerPosition.action.performed += OnPointerPositionChanged;
            config.CameraDrag.action.started += OnDragStarted;
            config.CameraDrag.action.canceled += OnGroundClick;
        }

        public void Tick()
        {
            if (actionMap == null || !Application.isFocused || gameModeController.Current == GameMode.Pause)
            {
                return;
            }

            if (config.CameraDrag.action.IsPressed())
            {
                PublishDragMovement();
                return;
            }

            PublishEdgeMovement();
            PublishKeyboardMovement();
        }

        public void Dispose()
        {
            if (pauseAction != null)
            {
                pauseAction.started -= OnPause;
                config.PointerPosition.action.performed -= OnPointerPositionChanged;
                config.CameraDrag.action.started -= OnDragStarted;
                config.CameraDrag.action.canceled -= OnGroundClick;
            }

            actionMap?.Disable();
        }

        private bool HasRequiredActions()
        {
            return config != null
                   && config.PointerPosition?.action != null
                   && config.PointerDelta?.action != null
                   && config.CameraDrag?.action != null
                   && config.CameraKeyboardMove?.action != null
                   && config.Pause?.action != null;
        }

        private void PublishEdgeMovement()
        {
            var position = config.PointerPosition.action.ReadValue<Vector2>();
            var direction = new Vector2(
                position.x <= cameraMovementConfig.ScreenEdgeSize ? -1f : position.x >= Screen.width - cameraMovementConfig.ScreenEdgeSize ? 1f : 0f,
                position.y <= cameraMovementConfig.ScreenEdgeSize ? -1f : position.y >= Screen.height - cameraMovementConfig.ScreenEdgeSize ? 1f : 0f);

            if (direction.sqrMagnitude > 0f)
            {
                edgeMovePublisher.Publish(new CameraEdgeMoveMessage(direction.normalized));
            }
        }

        private void PublishDragMovement()
        {
            var delta = config.PointerDelta.action.ReadValue<Vector2>();
            if (delta.sqrMagnitude > 0f)
            {
                wasDragging = true;
                dragMovePublisher.Publish(new CameraDragMoveMessage(delta));
            }
        }

        private void PublishKeyboardMovement()
        {
            Vector2 direction = config.CameraKeyboardMove.action.ReadValue<Vector2>();
            if (direction.sqrMagnitude > 0f)
                keyboardMovePublisher.Publish(new CameraKeyboardMoveMessage(direction));
        }

        private void OnPause(InputAction.CallbackContext _) => pausePublisher.Publish(new PauseInputMessage());

        private void OnGroundClick(InputAction.CallbackContext _)
        {
            if (wasDragging || !Application.isFocused || gameModeController.Current != GameMode.Gameplay || camera == null)
                return;

            Ray ray = camera.ScreenPointToRay(lastPointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, config.GroundClickLayers.value))
                groundClickPublisher.Publish(new GroundClickedMessage(hit.point));
        }

        private void OnDragStarted(InputAction.CallbackContext _) => wasDragging = false;
        private void OnPointerPositionChanged(InputAction.CallbackContext context) => lastPointerPosition = context.ReadValue<Vector2>();
    }
}
