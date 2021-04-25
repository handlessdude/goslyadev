using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public abstract class DialogueElement
{
    [SerializeField]
    public LocalizedString textValue;
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

    public SequentialDialogueElement()
    {

    }
        public SequentialDialogueElement(string val, bool isOnChar, DialogueElement dialogueElement)
    {
        textValue = val;
        isOnCharacter = isOnChar;
        next = dialogueElement;
    }
}

public class SelectionDialogueElement : DialogueElement
{
    public LocalizedString[] playerChoices;
    public List<System.Func<DialogueElement>> next;
    public SelectionDialogueElement()
    {
        isOnCharacter = false;
    }

    public SelectionDialogueElement(string val, LocalizedString[] choices, System.Func<DialogueElement>[] outcomes)
    {
        textValue = val;
        isOnCharacter = false;
        playerChoices = choices;
        //TODO: это временно. эта сигнатура нам не нужна.
        next = outcomes.ToList();
    }

    public DialogueElement ChooseNext(int i)
    { 
        return next[i]();
    }
}
