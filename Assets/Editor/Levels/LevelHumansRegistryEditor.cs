using UnityEditor;
using UnityEngine;
using Zombies.Levels;

namespace Zombies.Editor.Levels
{
    [CustomEditor(typeof(LevelHumansRegistry))]
    public sealed class LevelHumansRegistryEditor : UnityEditor.Editor
    {
        private SerializedProperty peopleProperty;

        private void OnEnable()
        {
            peopleProperty = serializedObject.FindProperty("people");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(peopleProperty, true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"People on level: {((LevelHumansRegistry)target).PeopleCount}", EditorStyles.miniLabel);

            if (!GUILayout.Button("Refresh People"))
            {
                return;
            }

            foreach (var selectedTarget in targets)
            {
                var registry = (LevelHumansRegistry)selectedTarget;
                Undo.RecordObject(registry, "Refresh Level People");
                registry.RefreshPeople();
                EditorUtility.SetDirty(registry);
            }

            serializedObject.Update();
        }
    }
}
