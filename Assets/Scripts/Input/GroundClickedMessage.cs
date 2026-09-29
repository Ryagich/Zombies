using UnityEngine;

namespace Zombies.Input
{
    public readonly struct GroundClickedMessage
    {
        public readonly Vector3 Position;
        public GroundClickedMessage(Vector3 position) => Position = position;
    }
}
