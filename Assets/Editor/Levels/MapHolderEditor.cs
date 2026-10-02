using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Zombies.Levels;

namespace Zombies.Editor.Levels
{
    [CustomEditor(typeof(MapHolder))]
    public sealed class MapHolderEditor : UnityEditor.Editor
    {
        private ReorderableList interactiveObjectsList;

        private void OnEnable()
        {
            var entries = serializedObject.FindProperty("interactiveObjects");
            interactiveObjectsList = new ReorderableList(serializedObject, entries, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Interactive Objects"),
                elementHeight = EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing * 3f,
                drawElementCallback = DrawInteractiveObject,
                onAddCallback = list =>
                {
                    list.serializedProperty.arraySize++;
                    list.index = list.serializedProperty.arraySize - 1;
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            interactiveObjectsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInteractiveObject(Rect rect, int index, bool isActive, bool isFocused)
        {
            var entry = interactiveObjectsList.serializedProperty.GetArrayElementAtIndex(index);
            var objectProperty = entry.FindPropertyRelative("<Target>k__BackingField");
            var locationProperty = entry.FindPropertyRelative("<LocationIndex>k__BackingField");
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var locationNames = GetLocationNames(GetProjectLevelCatalog());

            rect.y += EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineHeight), objectProperty, new GUIContent("Object"));
            rect.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
            DrawLocationPopup(new Rect(rect.x, rect.y, rect.width, lineHeight), locationProperty, locationNames);
        }

        private static void DrawLocationPopup(Rect rect, SerializedProperty property, string[] locationNames)
        {
            if (locationNames.Length == 0)
            {
                EditorGUI.HelpBox(rect, "Assign a Level Catalog Config.", MessageType.Info);
                property.intValue = -1;
                return;
            }

            property.intValue = EditorGUI.Popup(rect, "Location", Mathf.Clamp(property.intValue, 0, locationNames.Length - 1), locationNames);
        }

        private static string[] GetLocationNames(LevelCatalogConfig catalog)
        {
            if (catalog?.Locations == null)
                return System.Array.Empty<string>();

            var names = new string[catalog.Locations.Count];
            for (var index = 0; index < names.Length; index++)
            {
                var key = catalog.Locations[index]?.LocationName?.TableEntryReference.Key;
                names[index] = string.IsNullOrWhiteSpace(key) ? $"Location {index + 1}" : key;
            }

            return names;
        }

        private static LevelCatalogConfig GetProjectLevelCatalog()
        {
            const string projectScopePath = "Assets/Resources/Project/ProjectLifetimeScope.prefab";
            return AssetDatabase.LoadAssetAtPath<ProjectLifetimeScope>(projectScopePath)?.LevelCatalogConfig;
        }
    }
}
