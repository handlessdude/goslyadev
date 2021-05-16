using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDialog : DialogueNPC
{
    SawBot sawBot;
    bool greetings_played = false;
    bool after_boss_played = false;
    public DialogueObject secondDialogFile;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (!sawBot)
        {
            sawBot = GetComponent<SawBot>();
            sawBot.enabled = false;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (sawBot.IsDead() && !after_boss_played)
        {
            after_boss_played = true;
            GameplayState.isAccessToFatherGranted = true;
            dialogueFile = secondDialogFile;
            DeserializeDialogueFile();
            Action();
        }
    }

    protected override void DialogueExit()
    {
        base.DialogueExit();
        sawBot.enabled = true;

        Invoke("Agro", 0.5f);
        
    }

    void Agro()
    {
        sawBot.OnEnterSenseRange(player, Enemy.Sense.Sight);
        sawBot.goal = Enemy.Goal.Chase;
    }

    public void OnEnterDialogTrigger(GameObject p)
    {
        if (!sawBot.IsDead())
        {
            if (!greetings_played)
            {
                greetings_played = true;
                player = p;
                Action();
            }
        }
        
    }
}
