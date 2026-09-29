using UnityEditor;
using UnityEngine;
using Zombies.NPC;

namespace Zombies.Editor.NPC
{
    public static class NpcConfigSetup
    {
        private const string ConfigFolder = "Assets/Configs/NPC";

        [MenuItem("Tools/Zombies/Setup NPC Health and Attack Configs")]
        public static void Setup()
        {
            var manHealth = GetOrCreate<HealthConfig>("ManHealthConfig");
            var zombieHealth = GetOrCreate<HealthConfig>("ZombieHealthConfig");
            var manAttack = GetOrCreate<AttackConfig>("ManAttackConfig");
            var zombieAttack = GetOrCreate<AttackConfig>("ZombieAttackConfig");

            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var men = root.GetComponentsInChildren<global::ManLifetimeScope>(true);
                    var zombies = root.GetComponentsInChildren<global::ZombieLifetimeScope>(true);
                    if (men.Length == 0 && zombies.Length == 0)
                        continue;

                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);
                    foreach (var man in men)
                        Assign(man, "healthConfig", manHealth, "attackConfig", manAttack);
                    foreach (var zombie in zombies)
                        Assign(zombie, "healthConfig", zombieHealth, "attackConfig", zombieAttack);
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("NPC health and attack configs have been set up.");
        }

        private static T GetOrCreate<T>(string fileName) where T : ScriptableObject
        {
            var path = $"{ConfigFolder}/{fileName}.asset";
            var config = AssetDatabase.LoadAssetAtPath<T>(path);
            if (config != null)
                return config;

            config = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(config, path);
            return config;
        }

        private static void Assign(Object target, string fieldName, Object value, string secondFieldName, Object secondValue)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(fieldName).objectReferenceValue = value;
            serialized.FindProperty(secondFieldName).objectReferenceValue = secondValue;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
