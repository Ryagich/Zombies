using UnityEditor;
using UnityEngine;

namespace Zombies.Editor.NPC
{
    public static class ManScopeSetup
    {
        [MenuItem("Tools/Zombies/Configure Man Scopes")]
        public static void Configure()
        {
            foreach (var prefabGuid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var root = PrefabUtility.LoadPrefabContents(path);
                var changed = false;
                foreach (var scope in root.GetComponentsInChildren<ManLifetimeScope>(true))
                {
                    var serializedScope = new SerializedObject(scope);
                    var autoRun = serializedScope.FindProperty("autoRun");
                    if (autoRun != null && autoRun.boolValue)
                    {
                        autoRun.boolValue = false;
                        serializedScope.ApplyModifiedPropertiesWithoutUndo();
                        changed = true;
                    }
                }

                if (changed)
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                PrefabUtility.UnloadPrefabContents(root);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
