using System.Collections.Generic;
using UnityEngine;

namespace Zombies.Levels
{
    public sealed class LevelHumansRegistry : MonoBehaviour
    {
        [SerializeField] private List<ManLifetimeScope> people = new();

        public IReadOnlyList<ManLifetimeScope> People => people;
        public int PeopleCount => people?.Count ?? 0;

#if UNITY_EDITOR
        private void OnValidate() => RefreshPeople();

        [ContextMenu("Refresh People")]
        public void RefreshPeople()
        {
            people = new List<ManLifetimeScope>(GetComponentsInChildren<ManLifetimeScope>(true));
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
