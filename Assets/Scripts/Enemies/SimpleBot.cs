using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBot : Enemy
{
    public Vector2 lastKnownLocation;

    bool isThinkingAboutLife = false;
    int ticksSpentOnPath = 0;

    new Collider2D collider;

    protected override void Start()
    {

        animator = transform.Find("Sprite").GetComponent<Animator>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Start();
        directionX = 1f;
        goal = Goal.Patrol;
        collider = GetComponent<Collider2D>();
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

    protected override void Attack()
    {
        base.Attack();
        animator.Play("mobster_hit");
    }

    protected override void AbandonPath()
    {
        base.AbandonPath();
        ticksSpentOnPath = 0;
    }

    protected override void OnAI()
    {
        
        switch (goal)
        {
            case (Goal.Chase):
                {
                    if (!jumpSteer)
                    {
                        if (isPlayerInSenseRange.Count == 0)
                        {
                            OnLostTarget();
                        }
                        else if (isPlayerInAttackRange && !isAttackOnCooldown)
                        {
                            Attack();
                        }

                        if (ticksSpentOnPath > 60)
                        {
                            StartFollowingPath(target.transform.position);
                        }
                    }

                    if (followPath == false)
                    {
                        StartFollowingPath(target.transform.position);
                        if (followPath == false)
                        {
                            bool isSignificantX = Mathf.Abs(target.transform.position.x - transform.position.x) > 0.2f;
                            UpdateDirectionX(target.transform.position.x - transform.position.x > 0f ? 1f : -1f, isSignificantX);
                            physicalAction = PhysicalAction.Run;
                            animator.Play("mobster_run");
                            return;
                        }
                    }

                    if (isAttackAnimationPlaying || !followPath)
                    {
                        physicalAction = PhysicalAction.Stay;
                        //animator.Play("mobster_idle");
                    }
                    else
                    {
                        physicalAction = PhysicalAction.Run;
                        animator.Play("mobster_run");
                    }

                    break;
                }
            case (Goal.Investigate):
                {
                    if (followPath)
                    {
                        physicalAction = PhysicalAction.Walk;
                        animator.Play("mobster_run");
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
                        animator.Play("mobster_idle");
                    }
                    else
                    {
                        physicalAction = PhysicalAction.Walk;
                        animator.Play("mobster_run");
                    }
                    
                    break;
                }
            default:
                {
                    physicalAction = PhysicalAction.Stay;
                    animator.Play("mobster_idle");
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(collision.collider, collider);
        }
    }

    public override void OnEnterSenseRange(GameObject player, Sense s)
    {
        base.OnEnterSenseRange(player, s);
        goal = Goal.Chase;
    }
}
