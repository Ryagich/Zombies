using MessagePipe;
using UnityEngine;
using VContainer.Unity;
using Zombies.GameModes;

namespace Zombies.Input
{
    public sealed class CursorController : IStartable, System.IDisposable
    {
        private readonly ISubscriber<GameModeChangedMessage> gameModeSubscriber;
        private System.IDisposable subscription;

        public CursorController(ISubscriber<GameModeChangedMessage> gameModeSubscriber)
        {
            this.gameModeSubscriber = gameModeSubscriber;
        }

        public void Start()
        {
            subscription = gameModeSubscriber.Subscribe(message => Apply(message.Mode));
            Apply(GameMode.Gameplay);
        }

        public void Dispose() => subscription?.Dispose();

        private static void Apply(GameMode mode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
