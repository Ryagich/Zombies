using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Zombies.NPC
{
    public sealed class GroundClickZombieSpawner : IStartable, System.IDisposable
    {
        private readonly GameLifetimeScope gameScope;
        private readonly ISubscriber<Zombies.Input.GroundClickedMessage> subscriber;
        private System.IDisposable subscription;

        public GroundClickZombieSpawner(GameLifetimeScope gameScope, ISubscriber<Zombies.Input.GroundClickedMessage> subscriber)
        {
            this.gameScope = gameScope;
            this.subscriber = subscriber;
        }

        public void Start() => subscription = subscriber.Subscribe(message => Spawn(message.Position));
        public void Dispose() => subscription?.Dispose();

        private void Spawn(Vector3 position)
        {
            if (gameScope.ZombiePrefab == null)
            {
                Debug.LogError("Zombie prefab is not assigned to GameLifetimeScope.");
                return;
            }
            var zombie = Object.Instantiate(gameScope.ZombiePrefab, position, Quaternion.identity, gameScope.transform);
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
