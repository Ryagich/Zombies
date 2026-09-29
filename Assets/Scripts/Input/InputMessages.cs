using UnityEngine;

namespace Zombies.Input
{
    public readonly struct CameraEdgeMoveMessage
    {
        public readonly Vector2 Direction;

        public CameraEdgeMoveMessage(Vector2 direction) => Direction = direction;
    }

    public readonly struct CameraDragMoveMessage
    {
        public readonly Vector2 Delta;

        public CameraDragMoveMessage(Vector2 delta) => Delta = delta;
    }

    public readonly struct CameraKeyboardMoveMessage
    {
        public readonly Vector2 Direction;
        public CameraKeyboardMoveMessage(Vector2 direction) => Direction = direction;
    }
}
