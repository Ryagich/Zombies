namespace Zombies.Levels
{
    /// <summary>Published after a player selects an available map location.</summary>
    public readonly struct LocationSelectedMessage
    {
        public readonly int LocationIndex;

        public LocationSelectedMessage(int locationIndex) => LocationIndex = locationIndex;
    }
}
