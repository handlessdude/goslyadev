using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBot : Enemy
{
    public Vector2 lastKnownLocation;

    public static int specialAttackDamage = 20;
    public static float specialAttackCooldown = 1f;
    public static float specialAttackAnimationDuration = 0.5f;
    public static float specialAttackTelegraphDuration = 0.3f;

    bool isPlayerInSpecialAttackRange = false;
    bool isSpecialAttackOnCooldown = false;
    bool isSpecialAttackAnimationPlaying = false;

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
                    if (isAttackAnimationPlaying || !isSpecialAttackAnimationPlaying)
                    {
                        physicalAction = PhysicalAction.Stay;
                        return;
                    }
                    else
                    {
                        physicalAction = PhysicalAction.Run;
                    }

                    if (isPlayerInSenseRange.Count == 0)
                    {
                        OnLostTarget();
                    }
                    else if (isPlayerInAttackRange && !isAttackOnCooldown)
                    {
                        Attack();
                    }
                    else if (isPlayerInSpecialAttackRange && !isSpecialAttackOnCooldown)
                    {
                        SpecialAttack();
                    }

                    if (target.transform.position.y - transform.position.y > 4f && 
                        Mathf.Abs(target.transform.position.x - transform.position.x) < 4f)
                    {
                        //если игрок слишком высоко, то глупо пытаться стать прямо под него
                        physicalAction = PhysicalAction.Stay;
                        return;
                    }

                    bool isSignificantX = Mathf.Abs(target.transform.position.x - transform.position.x) > 0.2f;
                    UpdateDirectionX(target.transform.position.x - transform.position.x > 0f ? 1f : -1f, isSignificantX);
                    physicalAction = PhysicalAction.Run;

                    break;
                }
            case (Goal.Investigate):
                {
                    if (followPath)
                    {
                        physicalAction = PhysicalAction.Walk;
                    }
                    else
                    {
                        goal = Goal.Patrol;
                    }
                    
                    break;
                }
            case (Goal.Patrol):
                {
                    if (followPath == false && !isThinkingAboutLife)
                    {
                        bool should_think_about_life = Random.Range(0, 10) > 6;
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

    protected virtual void SpecialAttack()
    {
        isSpecialAttackOnCooldown = true;
        isSpecialAttackAnimationPlaying = true;
        Invoke("ResetSpecialAttackCooldown", specialAttackCooldown);
        Invoke("DealSpecialDamage", specialAttackTelegraphDuration);
        Invoke("OnSpecialAttackAnimationEnd", specialAttackAnimationDuration);
    }

    protected virtual void DealSpecialDamage()
    {
        if (isPlayerInSpecialAttackRange)
        {
            target.GetComponent<PlayerStats>().OnHit(gameObject, specialAttackDamage);
        }
    }

    protected virtual void OnSpecialAttackAnimationEnd()
    {
        isSpecialAttackAnimationPlaying = false;
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

    protected override void AbandonPath()
    {
        base.AbandonPath();
        ticksSpentOnPath = 0;
    }

    public override void OnEnterSenseRange(GameObject player, Sense s)
    {
        base.OnEnterSenseRange(player, s);
        goal = Goal.Chase;
    }

    public virtual void OnEnterSpecialAttackRange(GameObject player)
    {
        Debug.Log("PLAYER IN ATTACK RANGE");
        isPlayerInSpecialAttackRange = true;
        target = player;
    }

    public virtual void OnLeaveSpecialAttackRange(GameObject player)
    {
        isPlayerInSpecialAttackRange = false;
    }
}