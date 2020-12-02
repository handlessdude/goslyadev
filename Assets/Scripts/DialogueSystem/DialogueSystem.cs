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

    public DialogueEndElement(DialogueElement next)
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
    public string[] playerChoices;
    System.Func<DialogueElement>[] next;
    public SelectionDialogueElement(string val, string[] choices, System.Func<DialogueElement>[] outcomes)
    {
        textValue = val;
        isOnCharacter = false;
        playerChoices = choices;
        next = outcomes;
    }

    public DialogueElement ChooseNext(int i)
    { 
        return next[i]();
    }
}
