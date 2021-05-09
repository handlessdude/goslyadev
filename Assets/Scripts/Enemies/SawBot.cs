using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBot : Enemy
{
    public Vector2 lastKnownLocation;

    bool isThinkingAboutLife = false;
    int ticksSpentOnPath = 0;

    protected override void Start()
    {
        base.Start();
        goal = Goal.Patrol;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (followPath)
        {
            ticksSpentOnPath++;
        }
    }

    protected override void StartFollowingPath(Vector2 tpos)
    {
        base.StartFollowingPath(tpos);
        ticksSpentOnPath = 0;
    }

    protected override void OnAI()
    {
        
        switch (goal)
        {
            case (Goal.Chase):
                {
                    if (isPlayerInSenseRange.Count == 0)
                    {
                        OnLostTarget();
                    }
                    else if (isPlayerInAttackRange && !isAttackOnCooldown)
                    {
                        Attack();
                    }

                    if (followPath == false)
                    {
                        StartFollowingPath(target.transform.position);
                    }

                    if (isAttackAnimationPlaying || !followPath)
                    {
                        physicalAction = PhysicalAction.Stay;
                    }
                    else
                    {
                        physicalAction = PhysicalAction.Run;
                    }

                    if (ticksSpentOnPath > 60)
                    {
                        StartFollowingPath(target.transform.position);
                    }
                    break;
                }
            case (Goal.Investigate):
                {
                    if (followPath)
                    {
                        physicalAction = PhysicalAction.Walk;
                    }
                    
                    break;
                }
            case (Goal.Patrol):
                {
                    if (followPath == false && !isThinkingAboutLife)
                    {
                        bool should_think_about_life = Random.Range(0, 10) > 4;
                        if (!should_think_about_life)
                        {
                            int _direction = Random.Range(0, 2) * 2 - 1; //либо -1 либо 1
                            float _distance = Random.Range(0f, 5f);
                            int failure_counter = 0;
                            Vector2 tpos = new Vector2(transform.position.x + _direction * _distance * Pathfinder.cellSize, transform.position.y);
                            while (!Pathfinder.IsSpaceWalkable(transform.position, tpos))
                            {
                                _direction = Random.Range(0, 1) * 2 - 1; //либо -1 либо 1
                                _distance = Random.Range(1f, 5f);
                                tpos = new Vector2(transform.position.x + _direction * _distance * Pathfinder.cellSize, transform.position.y);
                                failure_counter++;

                                if (failure_counter > 5)
                                {
                                    return;
                                }
                            }

                            StartFollowingPath(tpos);
                        }
                        else
                        {
                            isThinkingAboutLife = true;
                            Invoke("RealizeLifeIsPointless", Random.Range(1f, 4f));
                        }
                    }
                    
                    if (isThinkingAboutLife)
                    {
                        physicalAction = PhysicalAction.Stay;
                    }
                    else
                    {
                        physicalAction = PhysicalAction.Walk;
                    }
                    
                    break;
                }
            default:
                {
                    physicalAction = PhysicalAction.Stay;
                    break;
                }
        }
    }

    void RealizeLifeIsPointless()
    {
        isThinkingAboutLife = false;
    }

    protected override void OnLostTarget()
    {
        base.OnLostTarget();
        goal = Goal.Investigate;
        
    }

    protected override void OnPathCompleted()
    {
        base.OnPathCompleted();

    }

    public override void OnEnterSenseRange(GameObject player, Sense s)
    {
        base.OnEnterSenseRange(player, s);
        goal = Goal.Chase;
    }
}
