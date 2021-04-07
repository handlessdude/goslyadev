using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeRobot : DialogueNPC
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

       
            var make_generator_work = new SequentialDialogueElement("big_robot_make_generator_work", false,
            new SequentialDialogueElement("big_robot_hero_what", true, new DialogueEndElement()));

            var tried_to_do_end = new SequentialDialogueElement("big_robot_hero_tried_to_do", true,
                new SequentialDialogueElement("big_robot_not_touch_it", false,
                new SequentialDialogueElement("big_robot_its_name", false,
                new SequentialDialogueElement("big_robot_hero_dont_want_to_know", true,
                make_generator_work))));

            var why_are_you_end = new SequentialDialogueElement("big_robot_hero_why_are_you", true,
                new SequentialDialogueElement("big_robot_performance", false,
                new SequentialDialogueElement("big_robot_hero_whats_name", true,
                new SequentialDialogueElement("big_robot_not_working", false,
                make_generator_work))));

            var why_are_you = new SequentialDialogueElement("big_robot_hero_why_are_you", true,
                new SequentialDialogueElement("big_robot_performance", false,
                new SequentialDialogueElement("big_robot_hero_whats_name", true,
                new SelectionDialogueElement("big_robot_not_working", new string[] { "big_robot_choice_tried_to_do" },
                new System.Func<DialogueElement>[] {
                () => tried_to_do_end
                }))));

            var tried_to_do = new SequentialDialogueElement("big_robot_hero_tried_to_do", true,
                new SequentialDialogueElement("big_robot_not_touch_it", false,
                new SequentialDialogueElement("big_robot_its_name", false,
                new SequentialDialogueElement("big_robot_hero_dont_want_to_know", true,
                new SelectionDialogueElement("big_robot_emptiness", new string[] { "big_robot_choice_why_are_you" },
                new System.Func<DialogueElement>[] {
                () => why_are_you_end
                })))));

            currentDialogElement = new SelectionDialogueElement("big_robot_emptiness", new string[]
            { "big_robot_choice_why_are_you" , "big_robot_choice_tried_to_do"
            }, new System.Func<DialogueElement>[] {
                () => why_are_you,
                () => tried_to_do
            });

            ((make_generator_work.next as SequentialDialogueElement).next
             as DialogueEndElement).nextOnNextVisit = make_generator_work;
      
       


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
        if (!isHintShown)
        {
            GameObject instantiated = Instantiate(pressToUnderstandHint, player.transform);
            instantiated.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 1.0f);
            isHintShown = true;
        }
        //GameplayState.isDialogEnded = true;
    }
}
