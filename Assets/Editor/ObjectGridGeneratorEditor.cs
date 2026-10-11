using UnityEditor;
using UnityEngine;
using Zombies.EditorTools;

namespace Zombies.Editor
{
    [CustomEditor(typeof(ObjectGridGenerator))]
    public sealed class ObjectGridGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), new GUIContent("Prefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("counts"), new GUIContent("Amount (X, Y, Z)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spacing"), new GUIContent("Spacing (X, Y, Z)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("firstOffset"), new GUIContent("Offset A"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondOffset"), new GUIContent("Offset B"), true);
            serializedObject.ApplyModifiedProperties();

            var generator = (ObjectGridGenerator)target;
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (GUILayout.Button("Generate"))
                    generator.Generate();

                if (GUILayout.Button("Clear"))
                    generator.Clear();
            }
        }
    }
}
