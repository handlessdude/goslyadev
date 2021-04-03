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

        nodes.Add(new StartNode(new Vector2(windowSize.x/4, windowSize.y/4), 150, 80, nodeStyle, selectedNodeStyle, outPointStyle, OnClickOutPoint));
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
            Save();
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
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height/scaling, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width/scaling, gridSpacing * j, 0f) + newOffset);
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
        return (screenCoords - canvasTopLeft)/scaling + scalingPivot;
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
        nodes.Add(new SelectionNode(mousePosition, 400, 70, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, 
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddChoiceNode(Vector2 mousePosition)
    {
        nodes.Add(new ChoiceNode(mousePosition, 400, 180, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddSequentialNode(Vector2 mousePosition)
    {
        nodes.Add(new SequentialNode(mousePosition, 400, 80, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, 
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    void OnClickAddEndNode(Vector2 mousePosition)
    {
        nodes.Add(new EndNode(mousePosition, 400, 80, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
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
    
    //TODO: да, здесь сложность можно сократить на порядок
    private void Save()
    {
        //(parent_ind, node)
        Stack<(int, Node)> s = new Stack<(int, Node)>();
        List<DialogueNodeObject> result = new List<DialogueNodeObject>();
        Dictionary<Node, int> enumerated_nodes = new Dictionary<Node, int>();

        s.Push((-1, getChildren(nodes[0]).First()));

        int i = -1;
        while (s.Count != 0)
        {
            i++;
            (int, Node) pair = s.Pop();
            UnityEvent e = null;
            if (pair.Item2 is ChoiceNode)
            {
                e = (pair.Item2 as ChoiceNode).OnSelect;
                Node n1 = getChildren(pair.Item2).First();

                result[pair.Item1].playerChoice_keys.Add(pair.Item2.textValue);
                result[pair.Item1].playerChoice_events.Add(e);
                if (enumerated_nodes.ContainsKey(n1))
                {
                    Debug.Log("Here!");
                    result[pair.Item1].playerChoice_values.Add(enumerated_nodes[n1]);
                    continue;
                }
                result[pair.Item1].playerChoice_values.Add(i);

                pair = (pair.Item1, n1);
            }
            result.Add(TransformToNodeObject(pair.Item2));
            enumerated_nodes[pair.Item2] = i;
            if (pair.Item1 != -1)
            { //обновляем родительскую ссылку
                if (result[pair.Item1].type == DialogueNodeObject.Type.End)
                {
                    result[pair.Item1].nextOnNextVisit = i;
                }
                else if (result[pair.Item1].type == DialogueNodeObject.Type.Sequential)
                {
                    result[pair.Item1].next = i;
                }
            }

            Node[] children = getChildren(pair.Item2);
            foreach (var c in children)
            {
                if (enumerated_nodes.ContainsKey(c))
                {
                    if (pair.Item2 is EndNode)
                    {
                        result[i].nextOnNextVisit = enumerated_nodes[c];
                    }
                    else if (pair.Item2 is SequentialNode)
                    {
                        result[i].next = enumerated_nodes[c];
                    }
                }
                else
                {
                    s.Push((i, c));
                }
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
}
