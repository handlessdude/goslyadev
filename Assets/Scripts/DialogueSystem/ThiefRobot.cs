using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefRobot : DialogueNPC
{
    void Start()
    {
        selfBubbleHeight = 2.3f;
        currentDialogElement = new SequentialDialogueElement("ask_rob_bout_board", false,
            new SequentialDialogueElement("ask_rob_bout_board2",false,
            new SequentialDialogueElement("robot_panic", false, new SequentialDialogueElement("k_demands_plate", true,
            new SequentialDialogueElement("rob_gives_board", false, new DialogueEndElement()))
            )));
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
