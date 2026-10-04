#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using Zombies.NPC;

namespace Zombies.EditorTools
{
    /// <summary>Keeps every ZombieStorage populated when ZombieConfig assets change.</summary>
    public sealed class ZombieStorageAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            var hasImportedOrMovedConfig = importedAssets.Concat(movedAssets).Any(IsZombieConfigPath);
            var hasDeletedOrMovedAsset = deletedAssets.Concat(movedFromAssetPaths).Any(path => path.EndsWith(".asset"));
            if (!hasImportedOrMovedConfig && !hasDeletedOrMovedAsset)
                return;

            foreach (var storageGuid in AssetDatabase.FindAssets("t:ZombieStorage"))
            {
                var storagePath = AssetDatabase.GUIDToAssetPath(storageGuid);
                var storage = AssetDatabase.LoadAssetAtPath<ZombieStorage>(storagePath);
                if (storage == null)
                    continue;

                var serializedStorage = new SerializedObject(storage);
                var configs = serializedStorage.FindProperty("<Configs>k__BackingField");
                var configGuids = AssetDatabase.FindAssets("t:ZombieConfig")
                    .OrderBy(guid => AssetDatabase.LoadAssetAtPath<ZombieConfig>(AssetDatabase.GUIDToAssetPath(guid)).BrainCost)
                    .ToArray();
                configs.arraySize = configGuids.Length;
                for (var index = 0; index < configGuids.Length; index++)
                    configs.GetArrayElementAtIndex(index).objectReferenceValue = AssetDatabase.LoadAssetAtPath<ZombieConfig>(AssetDatabase.GUIDToAssetPath(configGuids[index]));

                serializedStorage.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static bool IsZombieConfigPath(string path)
        {
            return AssetDatabase.LoadAssetAtPath<ZombieConfig>(path) != null;
        }
    }
}
#endif
