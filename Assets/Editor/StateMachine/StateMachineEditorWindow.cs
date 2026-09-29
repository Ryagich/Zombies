using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zombies.StateMachine.Graph;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.Editor.StateMachine
{
    /// <summary>Asset editor for the generic state-machine graph. The first node is the entry state.</summary>
    public sealed class StateMachineEditorWindow : EditorWindow
    {
        private const float NodeWidth = 250f;
        private const float NodeHeight = 150f;
        private StateMachineGraph graph;
        private Vector2 pan;
        private Vector2 scroll;
        private string statesFolder = "Assets/StateMachines/States";
        private string transitionsFolder = "Assets/StateMachines/Transitions";
        private Node selectedNode;

        [MenuItem("Tools/State Machine Editor")]
        public static void Open() => GetWindow<StateMachineEditorWindow>("State Machine Editor");

        private void OnGUI()
        {
            DrawToolbar();
            if (graph == null)
            {
                EditorGUILayout.HelpBox("Create or load a StateMachineGraph asset.", MessageType.Info);
                return;
            }

            graph.Nodes ??= new List<Node>();
            DrawCanvas();
            DrawInspector();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("New Graph", EditorStyles.toolbarButton)) CreateGraph();
                if (GUILayout.Button("Add State", EditorStyles.toolbarButton) && graph != null) AddState();
                graph = (StateMachineGraph)EditorGUILayout.ObjectField(graph, typeof(StateMachineGraph), false, GUILayout.Width(250));
                GUILayout.FlexibleSpace();
                statesFolder = GUILayout.TextField(statesFolder, EditorStyles.toolbarTextField, GUILayout.Width(190));
                transitionsFolder = GUILayout.TextField(transitionsFolder, EditorStyles.toolbarTextField, GUILayout.Width(190));
            }
        }

        private void DrawCanvas()
        {
            Rect canvas = new Rect(0, 20, position.width * .68f, position.height - 20);
            EditorGUI.DrawRect(canvas, new Color(.12f, .12f, .12f));
            DrawGrid(canvas, 20, new Color(1, 1, 1, .04f));
            DrawGrid(canvas, 100, new Color(1, 1, 1, .08f));
            Handles.BeginGUI();
            foreach (Node node in graph.Nodes)
            {
                if (node?.State == null) continue;
                foreach (Transition transition in node.State.Transitions)
                {
                    Node target = graph.Nodes.Find(candidate => candidate?.State == transition?.TargetState);
                    if (target == null) continue;
                    Vector2 from = node.Position + pan + new Vector2(NodeWidth, NodeHeight * .5f);
                    Vector2 to = target.Position + pan + new Vector2(0, NodeHeight * .5f);
                    Handles.DrawBezier(from, to, from + Vector2.right * 60, to + Vector2.left * 60, Color.white, null, 3);
                }
            }
            Handles.EndGUI();

            BeginWindows();
            for (int index = 0; index < graph.Nodes.Count; index++)
            {
                Node node = graph.Nodes[index];
                if (node == null) continue;
                Rect rect = new Rect(node.Position + pan, new Vector2(NodeWidth, NodeHeight));
                GUI.color = index == 0 ? new Color(.75f, 1f, .75f) : Color.white;
                Rect movedRect = GUI.Window(index, rect, _ => DrawNode(node), node.State != null ? node.State.name : "Missing State");
                node.Position = movedRect.position - pan;
                GUI.color = Color.white;
            }
            EndWindows();

            Event evt = Event.current;
            if (evt.type == EventType.MouseDrag && evt.button == 2 && canvas.Contains(evt.mousePosition)) { pan += evt.delta; Repaint(); }
            if (evt.type == EventType.MouseDown && evt.button == 1 && canvas.Contains(evt.mousePosition)) ShowCanvasMenu(evt.mousePosition - pan);
        }

        private void DrawNode(Node node)
        {
            if (GUILayout.Button("Select")) selectedNode = node;
            if (node == graph.Nodes[0]) GUILayout.Label("ENTRY", EditorStyles.miniBoldLabel);
            foreach (Transition transition in node.State.Transitions)
                GUILayout.Label("→ " + (transition?.TargetState != null ? transition.TargetState.name : "No target"), EditorStyles.miniLabel);
            GUI.DragWindow(new Rect(0, 0, NodeWidth, 20));
        }

        private void DrawInspector()
        {
            Rect area = new Rect(position.width * .68f, 20, position.width * .32f, position.height - 20);
            GUILayout.BeginArea(area, EditorStyles.helpBox);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("State Inspector", EditorStyles.boldLabel);
            if (selectedNode?.State == null) EditorGUILayout.HelpBox("Select a node.", MessageType.Info);
            else
            {
                var serialized = new SerializedObject(selectedNode.State);
                var property = serialized.GetIterator();
                property.NextVisible(true);
                while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
                if (GUILayout.Button("Add Transition")) AddTransition(selectedNode.State);
                if (GUILayout.Button("Remove Node")) { graph.Nodes.Remove(selectedNode); selectedNode = null; MarkDirty(); }
                serialized.ApplyModifiedProperties();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void ShowCanvasMenu(Vector2 position)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add State"), false, () => AddState(position));
            menu.ShowAsContext();
        }

        private void CreateGraph()
        {
            var asset = CreateInstance<StateMachineGraph>();
            string path = EditorUtility.SaveFilePanelInProject("Create State Machine Graph", "StateMachineGraph", "asset", "Choose a destination.");
            if (string.IsNullOrEmpty(path)) { DestroyImmediate(asset); return; }
            AssetDatabase.CreateAsset(asset, path); AssetDatabase.SaveAssets(); graph = asset;
        }

        private void AddState() => AddState(new Vector2(80 + graph.Nodes.Count * 35, 80 + graph.Nodes.Count * 25));
        private void AddState(Vector2 position)
        {
            EnsureFolder(statesFolder);
            var state = CreateInstance<State>(); state.name = "State";
            AssetDatabase.CreateAsset(state, AssetDatabase.GenerateUniqueAssetPath(statesFolder + "/State.asset"));
            graph.Nodes.Add(new Node(state) { Position = position }); selectedNode = graph.Nodes[^1]; MarkDirty(); AssetDatabase.SaveAssets();
        }

        private void AddTransition(State state)
        {
            EnsureFolder(transitionsFolder);
            var transition = CreateInstance<Transition>(); transition.name = state.name + " Transition";
            AssetDatabase.CreateAsset(transition, AssetDatabase.GenerateUniqueAssetPath(transitionsFolder + "/" + transition.name + ".asset"));
            state.Transitions.Add(transition); EditorUtility.SetDirty(state); AssetDatabase.SaveAssets();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string[] parts = path.Split('/'); string current = parts[0];
            for (int index = 1; index < parts.Length; index++) { string next = current + "/" + parts[index]; if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[index]); current = next; }
        }
        private void MarkDirty() { EditorUtility.SetDirty(graph); Repaint(); }
        private static void DrawGrid(Rect area, float spacing, Color color) { Handles.BeginGUI(); Handles.color = color; for (float x = area.x; x < area.xMax; x += spacing) Handles.DrawLine(new Vector3(x, area.y), new Vector3(x, area.yMax)); for (float y = area.y; y < area.yMax; y += spacing) Handles.DrawLine(new Vector3(area.x, y), new Vector3(area.xMax, y)); Handles.EndGUI(); }
    }
}
