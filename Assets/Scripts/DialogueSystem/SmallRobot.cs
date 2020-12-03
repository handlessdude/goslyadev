using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallRobot : DialogueNPC
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

        /*currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new SelectionDialogueElement("dialogue_test3",
                new string[] { "dialogue_option_test1", "dialogue_options_test2" },
                new System.Func<DialogueElement>[] {
                    () => currentDialogElement,
                    () => new DialogueEndElement(currentDialogElement)
                })));*/

        currentDialogElement = new SequentialDialogueElement("smallrobot_eh", false, new DialogueEndElement());
        ((currentDialogElement as SequentialDialogueElement).next as DialogueEndElement).nextOnNextVisit = currentDialogElement;
    }

    public override void Response(int i)
    {
        base.Response(i);
        anim.Play("surprise");
        Invoke("RestoreIdle", 2.0f);

    }

    protected override void OnDialogueDrawn()
    {
        base.OnDialogueDrawn();
        anim.Play("surprise");
        Invoke("RestoreIdle", 2.0f);
    }

    protected override void RotateSelf()
    {
        /*if (inDialogue && transform.position.x > player.transform.position.x)
        {
            transform.localScale = new Vector2(transform.localScale.x * (-1f), 1);
            /*foreach (Transform child in transform)
            {
                child.localScale = new Vector2(child.localScale.x * (-1), child.localScale.y);
            }
        }*/
    }

    void RestoreIdle()
    {
        CancelInvoke("RestoreIdle");
        anim.Play("idle");
    }    
}
