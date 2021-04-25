using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Events;
using System.IO;


public class DialogueEditor : EditorWindow
{

    List<Node> nodes;
    List<Connection> connections;

    GUIStyle nodeStyle;
    GUIStyle selectedNodeStyle;
    GUIStyle inPointStyle;
    GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private Vector2 windowSize = new Vector2(800f, 600f);
    private Rect zoomableCanvas;

    private float scaling = 1f;
    private float maxScaling = 10f;
    private float minScaling = 0.1f;
    private float _scale_per_delta = 0.1f / 20; //за 20 пунктов delta меняем размер изображения на 10%
    private float lastScaling;
    private Vector2 scalingPivot;
    private Vector2 scalingCoords;
    private bool optimize = false;
    private bool isDragged = false;

    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow()
    {
        DialogueEditor window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("Dialogue Editor");
        window.position = new Rect(window.position.x, window.position.y, window.windowSize.x, window.windowSize.y);
    }

    private void OnEnable()
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        if (connections == null)
        {
            connections = new List<Connection>();
        }


        lastScaling = scaling;

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        nodes.Add(new StartNode(new Vector2(windowSize.x / 4, windowSize.y / 4), 100, 40, nodeStyle, selectedNodeStyle, outPointStyle, OnClickOutPoint));
    }

    void OnGUI()
    {
        windowSize.x = position.width;
        windowSize.y = position.height;
        zoomableCanvas = new Rect(0f, 0f, windowSize.x, windowSize.y);

        if (scaling != lastScaling)
        {
            lastScaling = scaling;
            GUI.changed = true;
        }

        EditorZoomArea.Begin(scaling, zoomableCanvas, scalingPivot);
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessUnusedEvents(Event.current);
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        EditorZoomArea.End();

        if (GUI.Button(new Rect(5, 5, 50, 25), "SAVE"))
        {
            NewSave();
        }

        if (GUI.Button(new Rect(60, 5, 50, 25), "LOAD"))
        {
            Load();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    void DrawNodes()
    {
        if (nodes != null)
        {
            foreach (Node n in nodes)
            {
                n.Draw(optimize);
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    void ProcessUnusedEvents(Event e)
    {
        bool isScaled = false;
        switch (e.type)
        {
            case EventType.MouseDrag:
                {
                    if (e.button == 0)
                    {
                        isDragged = true;
                    }
                    break;
                }
            case EventType.MouseUp:
                {
                    if (e.button == 0)
                    {
                        isDragged = false;
                        GUI.changed = true;
                    }
                    break;
                }
            case EventType.ScrollWheel:
                {
                    isScaled = true;
                    GUI.changed = true;
                    break;
                }
        }
        optimize = isDragged || isScaled;
    }

    void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                {
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    if (e.button == 2)
                    {
                        scaling = 0.5f;
                        scalingPivot = new Vector2(400f, 300f);
                        Vector2 scaledCoordsMousePos = ScaledCoords(e.mousePosition);
                        scalingCoords += (scaledCoordsMousePos - scalingCoords) - (lastScaling / scaling) * (scaledCoordsMousePos - scalingCoords);
                    }
                    break;
                }
            case EventType.ScrollWheel:
                {
                    scaling -= e.delta.y * _scale_per_delta;
                    scaling = Mathf.Clamp(scaling, minScaling, maxScaling);
                    Vector2 scaledCoordsMousePos = ScaledCoords(e.mousePosition);
                    scalingCoords += (scaledCoordsMousePos - scalingCoords) - (lastScaling / scaling) * (scaledCoordsMousePos - scalingCoords);
                    scalingPivot = e.mousePosition;
                    break;
                }
            case EventType.MouseDrag:
                {
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
                }

        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / (gridSpacing * scaling));
        int heightDivs = Mathf.CeilToInt(position.height / (gridSpacing * scaling));

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height / scaling, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width / scaling, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Sequential Node"), false, () => OnClickAddSequentialNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Selection Node"), false, () => OnClickAddSelectionNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Choice Node"), false, () => OnClickAddChoiceNode(mousePosition));
        genericMenu.AddItem(new GUIContent("End Node"), false, () => OnClickAddEndNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    Vector2 ScaledCoords(Vector2 screenCoords)
    {
        Vector2 canvasTopLeft = new Vector2(zoomableCanvas.xMin, zoomableCanvas.yMin);
        return (screenCoords - canvasTopLeft) / scaling + scalingPivot;
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    void OnClickAddSelectionNode(Vector2 mousePosition)
    {
        nodes.Add(new SelectionNode(mousePosition, 300, 90, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddChoiceNode(Vector2 mousePosition)
    {
        nodes.Add(new ChoiceNode(mousePosition, 300, 200, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddSequentialNode(Vector2 mousePosition)
    {
        nodes.Add(new SequentialNode(mousePosition, 300, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddEndNode(Vector2 mousePosition)
    {
        nodes.Add(new EndNode(mousePosition, 300, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;
        TryToMakeConnection();
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;
        TryToMakeConnection();
    }

    //TODO: рефактор
    private void TryToMakeConnection()
    {
        if (selectedOutPoint != null && selectedInPoint != null)
        {
            if (selectedOutPoint.node == selectedInPoint.node)
            {
                ClearConnectionSelection();
            }
            if (selectedOutPoint.node is SelectionNode || selectedInPoint.node is ChoiceNode)
            {
                if (selectedOutPoint.node is SelectionNode && selectedInPoint.node is ChoiceNode)
                {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                ClearConnectionSelection();
                return;
            }
            if (connections.Any(x => (x.outPoint.node == selectedOutPoint.node)))
            {
                connections.Remove(connections.First(x => (x.outPoint.node == selectedOutPoint.node)));
            }
            CreateConnection();
            ClearConnectionSelection();
            return;
        }
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void OnClickRemoveNode(StandardNode node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void CreateConnection()
    {
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void NewSave()
    {
        List<Node> unresolved_links = new List<Node>();
        Dictionary<Node, int> result_links = new Dictionary<Node, int>();
        Queue<Node> q = new Queue<Node>();
        List<DialogueNodeObject> result = new List<DialogueNodeObject>();

        q.Enqueue(getChildren(nodes[0]).First());

        while (q.Count != 0)
        {
            Node n = q.Dequeue();

            if (result_links.ContainsKey(n))
            {
                continue;
            }
            result.Add(TransformToNodeObject(n));
            result_links[n] = result.Count() - 1;
            Node[] children = getChildren(n);
            if (n is SelectionNode)
            {
                foreach (var x in children)
                {
                    Node child = getChildren(x).First();
                    unresolved_links.Add(child);
                    result.Last().playerChoice_events.Add((x as ChoiceNode).OnSelect);
                    result.Last().playerChoice_keys.Add((x as ChoiceNode).textValue);
                    result.Last().playerChoice_values.Add(unresolved_links.Count());
                    result.Last().choiceNodes.Add(x.rect);
                    q.Enqueue(child);
                }
            }
            else
            {
                if (children.Count() == 0 && n is EndNode)
                {
                    result.Last().nextOnNextVisit = -1;
                }
                else
                {
                    unresolved_links.Add(children.First());
                    result.Last().next = unresolved_links.Count();
                    result.Last().nextOnNextVisit = unresolved_links.Count();
                }
                q.Enqueue(children.First());
            }
        }

        foreach (var x in result)
        {
            if (x.playerChoice_values != null)
            {
                for (int i = 0; i < x.playerChoice_values.Count(); i++)
                {
                    x.playerChoice_values[i] = result_links[unresolved_links[x.playerChoice_values[i] - 1]];
                }
            }
            if (x.next != 0)
            {
                x.next = result_links[unresolved_links[x.next - 1]];
            }
            if (x.nextOnNextVisit > 0)
            {
                x.nextOnNextVisit = result_links[unresolved_links[x.nextOnNextVisit - 1]];
            }
        }

        DialogueObject asset = ScriptableObject.CreateInstance<DialogueObject>();
        asset.nodes = result;
        string path = EditorUtility.SaveFilePanel("Save dialogue object to folder", "", "dialogue.asset", "asset");
        if (path.Length != 0)
        {
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        }
    }

    private DialogueNodeObject TransformToNodeObject(Node n)
    {
        DialogueNodeObject result = new DialogueNodeObject();
        if (n is SelectionNode)
        {
            result.type = DialogueNodeObject.Type.Selection;
        }
        else if (n is EndNode)
        {
            result.type = DialogueNodeObject.Type.End;
        }
        else
        {
            result.type = DialogueNodeObject.Type.Sequential;
        }

        result.textValue = n.textValue;
        result.rect = n.rect;
        result.isOnCharacter = n.isOnCharacter;
        return result;
    }

    private Node[] getChildren(Node n)
    {
        return connections.FindAll(x => x.outPoint.node == n).Select(x => x.inPoint.node).ToArray();
    }

    private void Load()
    {
        string path = EditorUtility.OpenFilePanel("Load dialogue file", "", "asset");
        if (path.Length == 0)
        { 
            return;
        }
        if (path.StartsWith(Application.dataPath))
        {
            path = "Assets" + path.Substring(Application.dataPath.Length);
        }
        Clear();
        DialogueObject file = AssetDatabase.LoadAssetAtPath<DialogueObject>(path);
        DeserializeFile(file);
    }

    private void Clear()
    {
        nodes.RemoveAll(x => !(x is StartNode ));
        connections.RemoveAll(x => true);
    }

    private void DeserializeFile(DialogueObject file)
    {
        if (!file)
        {
            Debug.LogError("No dialogue file!");
            return;
        }

        Dictionary<int, StandardNode> processed = new Dictionary<int, StandardNode>();
        Queue<(int, Node)> q = new Queue<(int, Node)>();

        for (int i = file.nodes.Count - 1; i >= 0; i--)
        {
            if (file.nodes[i].type == DialogueNodeObject.Type.End)
            {
                nodes.Add(new EndNode(file.nodes[i].rect.position, file.nodes[i].rect.width, file.nodes[i].rect.height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
                OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                processed[i] = nodes.Last() as StandardNode;
                processed[i].textValue = file.nodes[i].textValue;
                processed[i].isOnCharacter = file.nodes[i].isOnCharacter;
                if (file.nodes[i].nextOnNextVisit < 0)
                {
                    continue;
                }
                if (processed.ContainsKey(file.nodes[i].nextOnNextVisit))
                {
                    connections.Add(new Connection(processed[file.nodes[i].nextOnNextVisit].inPoint, processed[i].outPoint, OnClickRemoveConnection));
                }
                else
                {
                    q.Enqueue((file.nodes[i].nextOnNextVisit, processed[i]));
                }
            }
            else if (file.nodes[i].type == DialogueNodeObject.Type.Sequential)
            {
                nodes.Add(new SequentialNode(file.nodes[i].rect.position, file.nodes[i].rect.width, file.nodes[i].rect.height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
                OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                processed[i] = nodes.Last() as StandardNode;
                processed[i].textValue = file.nodes[i].textValue;
                processed[i].isOnCharacter = file.nodes[i].isOnCharacter;
                if (processed.ContainsKey(file.nodes[i].next))
                {
                    connections.Add(new Connection(processed[file.nodes[i].next].inPoint, processed[i].outPoint, OnClickRemoveConnection));
                }
                else
                {
                    q.Enqueue((file.nodes[i].next, processed[i]));
                }
            }
            else if (file.nodes[i].type == DialogueNodeObject.Type.Selection)
            {
                nodes.Add(new SelectionNode(file.nodes[i].rect.position, file.nodes[i].rect.width, file.nodes[i].rect.height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
                 OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                processed[i] = nodes.Last() as StandardNode;
                processed[i].textValue = file.nodes[i].textValue;
                for (int j = 0; j < file.nodes[i].playerChoice_values.Count; j++)
                {
                    nodes.Add(new ChoiceNode(file.nodes[i].choiceNodes[j].position, file.nodes[i].choiceNodes[j].width, file.nodes[i].choiceNodes[j].height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
                 OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                    //TODO: убрать этот момент.
                    processed[-40 * i + j] = nodes.Last() as StandardNode;
                    processed[-40 * i + j].textValue = file.nodes[i].playerChoice_keys[j];
                    (processed[-40 * i + j] as ChoiceNode).OnSelect = file.nodes[i].playerChoice_events[j];

                    connections.Add(new Connection(processed[-40*i + j].inPoint, processed[i].outPoint, OnClickRemoveConnection));

                    q.Enqueue((file.nodes[i].playerChoice_values[j], processed[-40 * i + j]));
                }
            }

            Repaint();
        }

        while (q.Count != 0)
        {
            (int, Node) pair = q.Dequeue();
            StandardNode d_node = pair.Item2 as StandardNode;
            int i = pair.Item1;
            connections.Add(new Connection(processed[i].inPoint, d_node.outPoint, OnClickRemoveConnection));
        }

        foreach (var x in processed)
        {
            (x.Value.this_as_serialized).Update();
            //(x.Value.this_as_serialized).ApplyModifiedProperties();
        }

        connections.Add(new Connection(processed[0].inPoint, nodes[0].outPoint, OnClickRemoveConnection));
    }
}
