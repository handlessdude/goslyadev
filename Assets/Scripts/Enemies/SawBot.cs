using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBot : Enemy
{
    public Animator animator;

    protected override void Start()
    {
        base.Start();
        goal = Goal.Patrol;
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected override void OnAI()
    {
        
    }
}
