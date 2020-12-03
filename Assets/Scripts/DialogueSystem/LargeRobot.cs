using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeRobot : DialogueNPC
{

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }


        selfBubbleHeight = 1f;

        currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new SelectionDialogueElement("dialogue_test3",
                new string[] { "dialogue_option_test1", "dialogue_options_test2" },
                new System.Func<DialogueElement>[] {
                    () => currentDialogElement,
                    () => new DialogueEndElement(currentDialogElement)
                })));

    }

    public override void Response(int i)
    {
        base.Response(i);
        anim.Play("surprise");
        Invoke("RestoreIdle", 2.0f);

    }

    void RestoreIdle()
    {
        CancelInvoke("RestoreIdle");
        anim.Play("idle");
    }
}
