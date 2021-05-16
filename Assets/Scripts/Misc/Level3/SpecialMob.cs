using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMob : DialogueNPC
{
    SimpleBot self;

    bool death_processed = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (!self)
        {
            self = GetComponent<SimpleBot>();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (self.IsDead() && !death_processed)
        {
            Debug.LogWarning("died");
            death_processed = true;
            SpecialMobStats.bots_killed += 1;
            if (SpecialMobStats.bots_killed == 2)
            {
                
                player = self.target;
                Debug.LogWarning(player);
                Action();
            }
        }
    }
}

public static class SpecialMobStats
{
    public static SimpleBot lastMobKilled;
    public static int bots_killed = 0;
}

