using MessagePipe;
using UnityEngine;
using VContainer.Unity;
using Zombies.GameModes;
using Zombies.CameraSystem;

namespace Zombies.Input
{
    public sealed class CameraMovementController : IStartable, ITickable, System.IDisposable
    {
        private readonly CameraMovementConfig config;
        private readonly Camera camera;
        private readonly ISubscriber<CameraEdgeMoveMessage> edgeMoveSubscriber;
        private readonly ISubscriber<CameraDragMoveMessage> dragMoveSubscriber;
        private readonly ISubscriber<GameModeChangedMessage> gameModeSubscriber;
        private readonly ISubscriber<CameraKeyboardMoveMessage> keyboardMoveSubscriber;

        private System.IDisposable edgeMoveSubscription;
        private System.IDisposable dragMoveSubscription;
        private System.IDisposable gameModeSubscription;
        private System.IDisposable keyboardMoveSubscription;
        private Vector2 edgeDirection;
        private Vector2 dragDelta;
        private Vector2 keyboardDirection;
        private bool isGameplay;
        private CameraBounds cameraBounds;

        public CameraMovementController(
            CameraMovementConfig config,
            Camera camera,
            ISubscriber<CameraEdgeMoveMessage> edgeMoveSubscriber,
            ISubscriber<CameraDragMoveMessage> dragMoveSubscriber,
            ISubscriber<GameModeChangedMessage> gameModeSubscriber,
            ISubscriber<CameraKeyboardMoveMessage> keyboardMoveSubscriber)
        {
            this.config = config;
            this.camera = camera;
            this.edgeMoveSubscriber = edgeMoveSubscriber;
            this.dragMoveSubscriber = dragMoveSubscriber;
            this.gameModeSubscriber = gameModeSubscriber;
            this.keyboardMoveSubscriber = keyboardMoveSubscriber;
        }

        public void Start()
        {
            edgeMoveSubscription = edgeMoveSubscriber.Subscribe(message => edgeDirection = message.Direction);
            dragMoveSubscription = dragMoveSubscriber.Subscribe(message => dragDelta += message.Delta);
            gameModeSubscription = gameModeSubscriber.Subscribe(message => isGameplay = message.Mode != GameMode.Pause);
            keyboardMoveSubscription = keyboardMoveSubscriber.Subscribe(message => keyboardDirection = message.Direction);
            isGameplay = true;
        }

        public void Tick()
        {
            if (camera == null || !isGameplay)
            {
                dragDelta = Vector2.zero;
                edgeDirection = Vector2.zero;
                keyboardDirection = Vector2.zero;
                return;
            }

            Vector2 combinedDirection = edgeDirection + keyboardDirection;
            var movement = GetPlanarDirection(combinedDirection.normalized) * (config.EdgeMoveSpeed * Time.deltaTime);
            if (dragDelta.sqrMagnitude > 0f)
            {
                var multiplier = config.InvertDrag ? -1f : 1f;
                movement += GetPlanarDirection(dragDelta) * (config.DragMoveSensitivity * multiplier);
            }

            camera.transform.position += movement;
            ClampPositionToBounds();
            edgeDirection = Vector2.zero;
            keyboardDirection = Vector2.zero;
            dragDelta = Vector2.zero;
        }

        public void SetBounds(CameraBounds bounds)
        {
            cameraBounds = bounds;
            ClampPositionToBounds();
        }

        public void Dispose()
        {
            edgeMoveSubscription?.Dispose();
            dragMoveSubscription?.Dispose();
            gameModeSubscription?.Dispose();
            keyboardMoveSubscription?.Dispose();
        }

        private Vector3 GetPlanarDirection(Vector2 input)
        {
            var forward = camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            var right = camera.transform.right;
            right.y = 0f;
            right.Normalize();
            return right * input.x + forward * input.y;
        }

        private void ClampPositionToBounds()
        {
            if (camera == null || cameraBounds == null)
                return;

            Bounds bounds = cameraBounds.WorldBounds;
            Vector3 position = camera.transform.position;
            position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
            position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);
            camera.transform.position = position;
        }
    }
}
