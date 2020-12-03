using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameRobot : DialogueNPC
{

    public Animator anim;
    public GameObject pressToUnderstandHint;

    bool isHintShown = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }


        selfBubbleHeight = 2.3f;

        currentDialogElement = new SequentialDialogueElement("endgame", false,
            new SequentialDialogueElement("big_robot_emptiness", true,
            new DialogueEndElement()));

        /*currentDialogElement = new SequentialDialogueElement(
        "dialogue_test1", true, new SequentialDialogueElement(
            "dialogue_test2", false, new SelectionDialogueElement("dialogue_test3",
                new string[] { "dialogue_option_test1", "dialogue_options_test2" },
                new System.Func<DialogueElement>[] {
                    () => currentDialogElement,
                    () => new DialogueEndElement(currentDialogElement)
                })));*/

    }

    public override void Response(int i)
    {
        base.Response(i);
        anim.Play("udivl");
        Invoke("RestoreIdle", 2.0f);

    }

    void RestoreIdle()
    {
        CancelInvoke("RestoreIdle");
        anim.Play("idle");
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
        SceneManager.LoadScene(0);
    }
}
