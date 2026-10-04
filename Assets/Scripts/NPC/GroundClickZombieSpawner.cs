using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Zombies.NPC
{
    public sealed class GroundClickZombieSpawner : IStartable, System.IDisposable
    {
        private readonly GameLifetimeScope gameScope;
        private readonly ISubscriber<Zombies.Input.GroundClickedMessage> subscriber;
        private readonly ZombieSelectionController selection;
        private System.IDisposable subscription;

        public GroundClickZombieSpawner(GameLifetimeScope gameScope, ISubscriber<Zombies.Input.GroundClickedMessage> subscriber, ZombieSelectionController selection)
        {
            this.gameScope = gameScope;
            this.subscriber = subscriber;
            this.selection = selection;
        }

        public void Start() => subscription = subscriber.Subscribe(message => Spawn(message.Position));
        public void Dispose() => subscription?.Dispose();

        private void Spawn(Vector3 position)
        {
            if (selection.SelectedConfig == null || !selection.TryConsumeSelected())
                return;

            var zombiePrefab = selection.SelectedConfig.ZombiePrefab;
            if (zombiePrefab == null)
            {
                Debug.LogError("Selected ZombieConfig has no zombie prefab.");
                return;
            }
            var zombie = Object.Instantiate(zombiePrefab, position, Quaternion.identity, gameScope.transform);
            var scope = zombie.GetComponent<ZombieLifetimeScope>();
            if (scope == null)
            {
                Debug.LogError("Zombie prefab must contain ZombieLifetimeScope.", zombie);
                Object.Destroy(zombie);
                return;
            }
            scope.parentReference.Object = gameScope;
            scope.Build();
        }
    }
}
