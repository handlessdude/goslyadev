using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRobot : DialogueNPC
{
    void Start()
    {

        if (!GameplayState.isMainRobotDialogEnded)
        {
            currentDialogElement = new SequentialDialogueElement("meets_informer", false,
                new SequentialDialogueElement("what_happend", true,
                new SequentialDialogueElement("have_problem", false,
                new SequentialDialogueElement("fix_it", true,
                new SequentialDialogueElement("i_cannot", false,
                new SequentialDialogueElement("didnt_wake_up", true,
                new SequentialDialogueElement("no_words", false,
                new SequentialDialogueElement("error", false,
                new SequentialDialogueElement("tin_can", true,
                new SequentialDialogueElement("lets_fix_mech", false,
                new SequentialDialogueElement("how_fix", true,
                new SequentialDialogueElement("find_boards", false,
                new SequentialDialogueElement("clear", true,
                new DialogueEndElement())))))))))))));
        }
           
    }


    protected override void DialogueExit()
    {
        base.DialogueExit();
        GameplayState.isMainRobotDialogEnded = true;
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
