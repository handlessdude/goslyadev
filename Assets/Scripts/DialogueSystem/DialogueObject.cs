using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueNodeObject
{
    [SerializeField]
    public LocalizedString textValue;
    public bool isOnCharacter;
    public Rect rect;

    public enum Type
    {
        Selection, End, Sequential
    }

    public Type type;

    public int nextOnNextVisit;
    public int next;

    [SerializeField]
    public List<LocalizedString> playerChoice_keys = new List<LocalizedString>();
    [SerializeField]
    public List<UnityEvent> playerChoice_events = new List<UnityEvent>();
    [SerializeField]
    public List<int> playerChoice_values = new List<int>();
}

public class DialogueObject : ScriptableObject
{
    [SerializeField]
    public List<DialogueNodeObject> nodes;
}
