using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialog : DialogueNPC
{
    public GameObject Hint;
    BoxCollider2D Box;
    void Start()
    {
        Box = gameObject.GetComponentInParent<BoxCollider2D>();
        if (!GameplayState.isStartedDialogEnded)
        {
            selfBubbleHeight = 2.3f;
            currentDialogElement = new SequentialDialogueElement("misterk_wake_up1", true,
                new SequentialDialogueElement("misterk_wake_up2", true,
                new SequentialDialogueElement("q_son", true,
                new SequentialDialogueElement("son_asked_about_sleep", true,
                new SequentialDialogueElement("son_know", true,
                new SequentialDialogueElement("had_dream1", true,
                new SequentialDialogueElement("had_dream2", true,
                new SequentialDialogueElement("again_body1", true,
                new SequentialDialogueElement("again_body2", true,
                new SequentialDialogueElement("will_be_healthy1", true,
                new SequentialDialogueElement("will_be_healthy2", true,
                new SequentialDialogueElement("go_work", true,
                new SequentialDialogueElement("call_later", true, 
                new DialogueEndElement())))))))))))));
        }
        else
            Box.enabled = false;
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
        Hint.SetActive(false);
        GameplayState.isStartedDialogEnded = true;
        Box.enabled = false;
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
