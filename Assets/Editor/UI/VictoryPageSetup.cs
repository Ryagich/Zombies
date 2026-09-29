using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization.Components;
using Zombies.UI;

namespace Zombies.Editor.UI
{
    public static class VictoryPageSetup
    {
        private const string PausePath = "Assets/Prefabs/UI/Pages/Pause Page.prefab";
        private const string VictoryPath = "Assets/Prefabs/UI/Pages/Victory Page.prefab";
        private const string ConfigPath = "Assets/Configs/UI/UIConfig.asset";

        [MenuItem("Tools/Zombies/Setup Victory Page")]
        public static void Setup()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(VictoryPath) == null)
                AssetDatabase.CopyAsset(PausePath, VictoryPath);

            var root = PrefabUtility.LoadPrefabContents(VictoryPath);
            root.name = "Victory Page";
            foreach (var localizer in root.GetComponentsInChildren<LocalizeStringEvent>(true))
            {
                var textObjectName = localizer.gameObject.name;
                localizer.StringReference.TableReference = "Tables";
                localizer.StringReference.TableEntryReference = textObjectName == "Title" ? "Victory_Title" : "Victory_Continue";
            }
            PrefabUtility.SaveAsPrefabAsset(root, VictoryPath);
            PrefabUtility.UnloadPrefabContents(root);

            var config = AssetDatabase.LoadAssetAtPath<UIConfig>(ConfigPath);
            var prefab = AssetDatabase.LoadAssetAtPath<RectTransform>(VictoryPath);
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("<VictoryPagePrefab>k__BackingField").objectReferenceValue = prefab;
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}
