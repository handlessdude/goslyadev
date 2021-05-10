using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefRobot : DialogueNPC
{
    public GameObject board;
    void Start()
    {
        if (!GameplayState.isThiefRobotDialogEnded)
        {
            selfBubbleHeight = 2.3f;
            currentDialogElement = new SequentialDialogueElement("ask_rob_bout_board1", true,
                new SequentialDialogueElement("ask_rob_bout_board2", true,
                new SequentialDialogueElement("ask_rob_bout_board3", true,
                new SequentialDialogueElement("ask_rob_bout_board4", true,
                new SequentialDialogueElement("ask_rob_bout_board5", true,
                new SequentialDialogueElement("ask_rob_bout_board6", true,
                new SequentialDialogueElement("robot_panic", false,
                new SequentialDialogueElement("k_demands_plate", true,
                new SequentialDialogueElement("rob_gives_board", false, 
                new DialogueEndElement())))))))));
        }
                
    }

    protected override void DialogueExit()
    {
        board.SetActive(true);
        base.DialogueExit();
        GameplayState.isThiefRobotDialogEnded = true;
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
