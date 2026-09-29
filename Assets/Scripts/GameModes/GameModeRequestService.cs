using System;
namespace Zombies.GameModes { public sealed class GameModeRequestService { public event Action<GameMode> Requested; public void Request(GameMode mode) => Requested?.Invoke(mode); } }
