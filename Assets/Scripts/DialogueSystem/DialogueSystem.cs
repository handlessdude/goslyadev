using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DialogueElement
{
    [SerializeField]
    public string textValue;
    public bool isOnCharacter;
}

public class DialogueEndElement : DialogueElement
{
    public DialogueElement nextOnNextVisit;

    public DialogueEndElement()
    {
        nextOnNextVisit = this;
    }

    public DialogueEndElement(DialogueEndElement next)
    {
        nextOnNextVisit = next;
    }
}

public class SequentialDialogueElement : DialogueElement
{
    public DialogueElement next;

    public SequentialDialogueElement(string val, bool isOnChar, DialogueElement dialogueElement)
    {
        textValue = val;
        isOnCharacter = isOnChar;
        next = dialogueElement;
    }
}

public class SelectionDialogueElement : DialogueElement
{
    public List<string> playerChoices;
    public List<System.Func<DialogueElement>> next;
    public SelectionDialogueElement(string val, List<string> choices, List<System.Func<DialogueElement>> outcomes)
    {
        textValue = val;
        isOnCharacter = false;
        playerChoices = choices;
        next = outcomes;
    }
}
