using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSecondLvl : DialogueNPC
{
    

    void Start()
    {
            selfBubbleHeight = 2.3f;
            currentDialogElement = new SequentialDialogueElement("after_repair1", true,
                new SequentialDialogueElement("after_repair2", true,
                new SequentialDialogueElement("after_repair3", true,
                new SequentialDialogueElement("call_from_hosp", true,
                new SequentialDialogueElement("meets_hosp", true,
                new SequentialDialogueElement("news_bout_son1", true,
                new SequentialDialogueElement("news_bout_son2", true,
                new SequentialDialogueElement("news_bout_son3", true,
                new SequentialDialogueElement("hung_up", false,
                new SequentialDialogueElement("have_no_time1", true,
                new SequentialDialogueElement("have_no_time2", true,
                new DialogueEndElement())))))))))));
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
        SceneManager.LoadScene(0);
        Cursor.visible = true;
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
