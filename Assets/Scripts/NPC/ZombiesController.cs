using System.Collections.Generic;
using UnityEngine;

namespace Zombies.NPC
{
    public sealed class ZombiesController
    {
        private readonly List<ZombieLifetimeScope> zombies = new();
        public IReadOnlyList<ZombieLifetimeScope> Zombies => zombies;

        public void Register(ZombieLifetimeScope zombie)
        {
            if (zombie != null && !zombies.Contains(zombie)) zombies.Add(zombie);
        }

        public void Unregister(ZombieLifetimeScope zombie) => zombies.Remove(zombie);

        public void Clear()
        {
            foreach (var zombie in new List<ZombieLifetimeScope>(zombies))
            {
                if (zombie == null)
                    continue;

                // Destruction is deferred, so hide the zombie immediately to
                // prevent it from participating in the next level's navmesh bake.
                zombie.gameObject.SetActive(false);
                Object.Destroy(zombie.gameObject);
            }

            zombies.Clear();
        }

        public ZombieLifetimeScope FindClosestAlive(Vector3 position)
        {
            ZombieLifetimeScope closest = null;
            var closestDistance = float.PositiveInfinity;
            foreach (var zombie in zombies)
            {
                if (zombie == null || zombie.Hp == null || zombie.Hp.IsDead || !zombie.gameObject.activeInHierarchy)
                    continue;

                var distance = (zombie.transform.position - position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closest = zombie;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}
