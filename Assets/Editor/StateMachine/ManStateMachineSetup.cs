using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using Zombies.NPC;
using Zombies.NPC.StateMachine;
using Zombies.StateMachine.Graph;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.Editor.StateMachine
{
    public static class ManStateMachineSetup
    {
        private const string GraphPath = "Assets/Content/StateMachines/ManStateMachineGraph.asset";
        private const string ConfigPath = "Assets/Configs/NPC/ManPatrolConfig.asset";
        private const string ManPrefabPath = "Assets/Prefabs/NPC/Man.prefab";

        [MenuItem("Tools/Zombies/Setup Man Idle and Patrol")]
        public static void Setup()
        {
            var graph = AssetDatabase.LoadAssetAtPath<StateMachineGraph>(GraphPath);
            if (graph == null)
            {
                Debug.LogError($"State machine graph was not found at '{GraphPath}'.");
                return;
            }

            var config = AssetDatabase.LoadAssetAtPath<ManPatrolConfig>(ConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<ManPatrolConfig>();
                AssetDatabase.CreateAsset(config, ConfigPath);
            }

            ClearGraphSubAssets(graph);
            BuildGraph(graph);
            ConfigureManPrefabs(config);
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Man Idle/Patrol state machine has been set up.");
        }

        private static void BuildGraph(StateMachineGraph graph)
        {
            var idle = AddSubAsset<State>(graph, "Man Idle");
            idle.Behaviours.Add(AddSubAsset<ManIdleBehaviour>(graph, "Man Idle Behaviour"));

            var patrol = AddSubAsset<State>(graph, "Man Patrol");
            patrol.Behaviours.Add(AddSubAsset<ManPatrolBehaviour>(graph, "Man Patrol Behaviour"));

            var enterPatrol = AddSubAsset<Transition>(graph, "Idle To Patrol");
            enterPatrol.TargetState = patrol;
            enterPatrol.Conditions.Add(AddSubAsset<ManHasPatrolPointsCondition>(graph, "Has Patrol Points"));
            idle.Transitions.Add(enterPatrol);

            var exitPatrol = AddSubAsset<Transition>(graph, "Patrol To Idle");
            exitPatrol.TargetState = idle;
            exitPatrol.Conditions.Add(AddSubAsset<ManHasNoPatrolPointsCondition>(graph, "Has No Patrol Points"));
            patrol.Transitions.Add(exitPatrol);

            graph.Nodes = new List<Node>
            {
                new(idle) { Position = new Vector2(80f, 160f) },
                new(patrol) { Position = new Vector2(420f, 160f) },
            };
        }

        private static void ConfigureManPrefabs(ManPatrolConfig config)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefabRoot = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var scopes = prefabRoot.GetComponentsInChildren<global::ManLifetimeScope>(true);
                    if (scopes.Length == 0)
                        continue;

                    // The removed prototype components were present only on Man objects.
                    // Clear their now-missing serialized entries before Unity saves the prefab.
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabRoot);

                    foreach (var scope in scopes)
                    {
                        var serializedScope = new SerializedObject(scope);
                        serializedScope.FindProperty("patrolConfig").objectReferenceValue = config;
                        serializedScope.ApplyModifiedPropertiesWithoutUndo();
                        ExcludeFromNavMeshBake(scope.gameObject);
                        foreach (var point in scope.PatrolPoints)
                            if (point != null)
                                ExcludeFromNavMeshBake(point);
                    }

                    EditorUtility.SetDirty(prefabRoot);
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        private static void ExcludeFromNavMeshBake(GameObject gameObject)
        {
            var modifier = gameObject.GetComponent<NavMeshModifier>();
            if (modifier == null)
                modifier = gameObject.AddComponent<NavMeshModifier>();

            modifier.ignoreFromBuild = true;
            modifier.applyToChildren = true;
            EditorUtility.SetDirty(modifier);
        }

        private static T AddSubAsset<T>(Object parent, string assetName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = assetName;
            AssetDatabase.AddObjectToAsset(asset, parent);
            return asset;
        }

        private static void ClearGraphSubAssets(StateMachineGraph graph)
        {
            var path = AssetDatabase.GetAssetPath(graph);
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                if (asset != graph)
                    Object.DestroyImmediate(asset, true);
        }
    }
}
