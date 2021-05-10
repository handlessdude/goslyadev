using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixererBot : DialogueNPC
{
    public GameObject board;
    void Start()
    {
        if (!GameplayState.isFixererDiaglolEnded)
        {
            selfBubbleHeight = 2f;
            currentDialogElement = new SequentialDialogueElement("meets_fixer", true,
                new SequentialDialogueElement("fixer_meets_k", false,
                new SequentialDialogueElement("remember_why1", true,
                new SequentialDialogueElement("remember_why2", true,
                new SequentialDialogueElement("insanity_of_ designer", false,
                new SequentialDialogueElement("ask_fixer_board", true,
                new SequentialDialogueElement("know_bout_board", false,
                new SequentialDialogueElement("fixer_knows_everyth", true,
                new SequentialDialogueElement("fixer_prep_board", false,
                new SequentialDialogueElement("thanks_to_fix", true,
                new SequentialDialogueElement("and_sir", false,
                new SequentialDialogueElement("what_fixer", true,
                new SequentialDialogueElement("good_luck", false,
                new DialogueEndElement())))))))))))));
        }
    }

    protected override void DialogueExit()
    {
        board.SetActive(true);
        base.DialogueExit();
        GameplayState.isFixererDiaglolEnded = true;
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
