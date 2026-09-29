using Unity.AI.Navigation;
using UnityEngine;

namespace Zombies.Navigation
{
    public sealed class GameNavMeshController
    {
        private readonly GameLifetimeScope gameScope;
        private NavMeshSurface surface;

        public GameNavMeshController(GameLifetimeScope gameScope)
        {
            this.gameScope = gameScope;
        }

        public void Rebuild()
        {
            if (gameScope.NavMeshSurfacePrefab == null)
            {
                Debug.LogError("NavMeshSurface prefab is not assigned to GameLifetimeScope.");
                return;
            }

            if (surface == null)
            {
                surface = Object.Instantiate(gameScope.NavMeshSurfacePrefab, gameScope.transform);
                surface.name = gameScope.NavMeshSurfacePrefab.name;
            }

            surface.RemoveData();
            surface.BuildNavMesh();
        }
    }
}
