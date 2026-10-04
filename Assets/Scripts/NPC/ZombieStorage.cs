using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "ZombieStorage", menuName = "Zombies/NPC/Zombie Storage")]
    public sealed class ZombieStorage : ScriptableObject
    {
        [field: SerializeField] public List<ZombieConfig> Configs { get; private set; } = new();
        public IReadOnlyList<ZombieConfig> SortedConfigs => Configs;

        private void OnEnable() => SortConfigs();

        private void OnValidate()
        {
#if UNITY_EDITOR
            CollectConfigs();
#endif
            SortConfigs();
        }

#if UNITY_EDITOR
        [ContextMenu("Collect Zombie Configs")]
        private void CollectConfigs()
        {
            var guids = AssetDatabase.FindAssets("t:ZombieConfig");
            Configs.Clear();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<ZombieConfig>(path);
                if (config != null)
                    Configs.Add(config);
            }
        }
#endif

        private void SortConfigs()
        {
            Configs.RemoveAll(config => config == null);
            Configs.Sort((left, right) => left.BrainCost.CompareTo(right.BrainCost));
        }
    }
}
