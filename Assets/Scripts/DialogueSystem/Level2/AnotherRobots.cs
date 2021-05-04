
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherRobots : DialogueNPC
{
    void Start()
    {
        selfBubbleHeight = 2.3f;
        var Diaglogs = new string[6] { "random_rep1", "random_rep2", "random_rep3", "random_rep4", "random_rep5", "random_rep6" };
        currentDialogElement = new SequentialDialogueElement(Diaglogs[new System.Random().Next(Diaglogs.Length - 1)], false, new DialogueEndElement());
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
    }

    public override void Response(int i)
    {
        base.Response(i);
        Invoke("RestoreIdle", 2.0f);
    }
    void RestoreIdle()
    {
        CancelInvoke("RestoreIdle");
    }
}
