using UnityEngine;

namespace Zombies.Container
{
    public static class ProjectBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (ProjectLifetimeScope.Instance != null)
            {
                return;
            }

            var prefab = Resources.Load<ProjectLifetimeScope>("Project/ProjectLifetimeScope");
            if (prefab == null)
            {
                Debug.LogError("ProjectLifetimeScope prefab not found at Resources/Project/ProjectLifetimeScope.");
                return;
            }

            Object.Instantiate(prefab).name = prefab.name;
        }
    }
}
