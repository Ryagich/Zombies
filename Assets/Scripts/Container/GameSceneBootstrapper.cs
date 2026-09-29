using UnityEngine;
using VContainer;
using Zombies.Bootstrap;

namespace Zombies.Container
{
    public sealed class GameSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private global::GameLifetimeScope gameScopePrefab;
        [SerializeField] private Camera gameCamera;

        private async void Awake()
        {
            var projectScope = ProjectLifetimeScope.Instance;
            if (projectScope == null)
            {
                Debug.LogError("ProjectLifetimeScope was not initialized.", this);
                return;
            }

            await projectScope.Container.Resolve<BootCompletion>().WaitAsync();
            var gameScope = BuildChildScope(gameScopePrefab, projectScope);
            gameScope?.SetGameCamera(gameCamera);
            gameScope?.Build();
            gameScope?.BuildCanvasScope();
        }

        private TScope BuildChildScope<TScope>(TScope prefab, VContainer.Unity.LifetimeScope parentScope)
            where TScope : VContainer.Unity.LifetimeScope
        {
            if (prefab == null)
            {
                Debug.LogError($"{typeof(TScope).Name} prefab is not assigned.", this);
                return null;
            }

            var scope = Instantiate(prefab);
            scope.gameObject.SetActive(false);
            scope.parentReference.Object = parentScope;
            scope.gameObject.SetActive(true);
            return scope;
        }
    }
}
