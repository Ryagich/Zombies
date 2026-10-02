namespace Zombies.Levels
{
    /// <summary>Published when the pointer enters or leaves a map location.</summary>
    public readonly struct LocationHoverMessage
    {
        public readonly int LocationIndex;

        public LocationHoverMessage(int locationIndex) => LocationIndex = locationIndex;
    }
}
