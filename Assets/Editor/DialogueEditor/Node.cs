using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;

public class Node : ScriptableObject
{

    //я и сам не понимаю, почему это здесь, но иначе не работает
    protected SerializedObject this_as_serialized;

    public LocalizedString textValue;
    public bool isOnCharacter;

    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;


    public Node (Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle outPointStyle, 
        Action<ConnectionPoint> OnClickOutPoint)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = defaultStyle;
        defaultNodeStyle = defaultStyle;
        selectedNodeStyle = selectedStyle;
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        //трудно понять, что здесь происходит. 
        //я и сам не понимаю. трудно представить, сколько всего пошло не так,
        //чтобы мне пришлось написать это
        this_as_serialized = new UnityEditor.SerializedObject(this);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public virtual void Draw(bool optimized = false)
    {
        outPoint.Draw();
        GUI.Box(rect, title, style);
        this_as_serialized.ApplyModifiedProperties();
    }

    public virtual bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                            e.Use();
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }
                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                }
            case EventType.MouseUp:
                {
                    isDragged = false;
                    break;
                }

            case EventType.MouseDrag:
                {
                    if (e.button == 0 && rect.Contains(e.mousePosition))
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
                }
        }

        return false;
    }

    protected virtual void ProcessContextMenu()
    {
        
    }

    
}



public class StandardNode : Node
{
    public ConnectionPoint inPoint;
    public Action<StandardNode> OnRemoveNode;
    public StandardNode(Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<StandardNode> OnClickRemoveNode)
        : base(position, width, height, defaultStyle, selectedStyle, outPointStyle, OnClickOutPoint)
    {
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        OnRemoveNode = OnClickRemoveNode;
    }

    protected override void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    protected void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    public override void Draw(bool optimized = false)
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
    }
}

public class StartNode : Node 
{
    public StartNode(Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickOutPoint)
        : base(position, width, height, defaultStyle, selectedStyle, outPointStyle, OnClickOutPoint)
    {

    }

    public override void Draw(bool optimized = false)
    {
        base.Draw();
        if (!optimized)
        {
            Rect r = rect;
            r.position = new Vector2(r.position.x + 15, r.position.y - r.height / 2 + 20);
            EditorGUI.LabelField(r, "Start Node");
        }
    }
}



public class SequentialNode : StandardNode
{
    public SequentialNode (Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<StandardNode> OnClickRemoveNode)
        : base(position, width, height, defaultStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
    }

    public override void Draw(bool optimized = false)
    {
        base.Draw();
        if (!optimized)
        {
            Rect r = rect;
            r.position = new Vector2(r.position.x + 15, r.position.y - r.height / 2 + 20);
            EditorGUI.LabelField(r, "Sequential Node");
            r.position = new Vector2(r.position.x, r.position.y + 50);
            r.width -= 25;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("textValue"));
            r.position = new Vector2(r.position.x, r.position.y + 18);
            r.height = 20;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("isOnCharacter"));
            this_as_serialized.ApplyModifiedProperties();
        }
    }
}

public class SelectionNode : StandardNode
{
    public SelectionNode(Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<StandardNode> OnClickRemoveNode)
        : base(position, width, height, defaultStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
    }

    public override void Draw(bool optimized = false)
    {
        base.Draw();
        if (!optimized)
        {
            Rect r = rect;
            r.position = new Vector2(r.position.x + 15, r.position.y - r.height / 2 + 20);
            EditorGUI.LabelField(r, "Selection Node");
            r.position = new Vector2(r.position.x, r.position.y + 47);
            r.height = 20;
            r.width -= 25;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("textValue"));
            this_as_serialized.ApplyModifiedProperties();
        }
    }
}

public class ChoiceNode : StandardNode
{
    public UnityEvent OnSelect;
    public int priority;
    public ChoiceNode(Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<StandardNode> OnClickRemoveNode)
        : base(position, width, height, defaultStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
    }

    public override void Draw(bool optimized = false)
    {
        base.Draw();
        if (!optimized)
        {
            Rect r = rect;
            r.position = new Vector2(r.position.x + 15, r.position.y - r.height / 2 + 20);
            EditorGUI.LabelField(r, "Choice Node");
            r.position = new Vector2(r.position.x, r.position.y + 100);
            r.width -= 25;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("textValue"));
            r.position = new Vector2(r.position.x, r.position.y + 22);
            r.height = 20;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("priority"));
            r.position = new Vector2(r.position.x, r.position.y + 22);
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("OnSelect"));
            this_as_serialized.ApplyModifiedProperties();
        }
    }
}

public class EndNode : StandardNode
{
    public EndNode(Vector2 position, float width, float height, GUIStyle defaultStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<StandardNode> OnClickRemoveNode)
        : base(position, width, height, defaultStyle, selectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode)
    {
    }

    public override void Draw(bool optimized = false)
    {
        base.Draw();
        if (!optimized)
        {
            Rect r = rect;
            r.position = new Vector2(r.position.x + 15, r.position.y - r.height / 2 + 20);
            EditorGUI.LabelField(r, "End Node");
            r.position = new Vector2(r.position.x, r.position.y + 50);
            r.width -= 25;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("textValue"));
            r.position = new Vector2(r.position.x, r.position.y + 18);
            r.height = 20;
            EditorGUI.PropertyField(r, this_as_serialized.FindProperty("isOnCharacter"));
            this_as_serialized.ApplyModifiedProperties();
        }
    }
}
