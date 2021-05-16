using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleBot : Enemy
{
    public Vector2 lastKnownLocation;

    bool isThinkingAboutLife = false;
    int ticksSpentOnPath = 0;

    bool isInAir = false;
    bool isLandingAnimationPlaying = false;

    new Collider2D collider;

    Transform middleGround;

    protected override void Start()
    {

        animator = transform.Find("Sprite").GetComponent<Animator>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Start();
        directionX = 1f;
        goal = Goal.Patrol;
        collider = GetComponent<Collider2D>();

        if (!middleGround)
        {
            middleGround = transform.Find("MiddleGround");
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (followPath)
        {
            ticksSpentOnPath++;
        }

        if (isInAir)
        {
            //как в PlayerController
            isInAir = !Physics2D.OverlapCircleAll(middleGround.position, 0.05f, 256).Any();
            if (!isInAir)
            {
                animator.speed = 1f;
                isLandingAnimationPlaying = true;
                Invoke("StopLandingAnimation", 0.25f);
            }
        }
    }

    void StopLandingAnimation()
    {
        isLandingAnimationPlaying = false;
    }

    protected override void StartFollowingPath(Vector2 tpos)
    {
        base.StartFollowingPath(tpos);
        CancelInvoke("StopLandingAnimation");
        isInAir = false;
        isLandingAnimationPlaying = false;
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
        isLandingAnimationPlaying = false;
        isInAir = false;
        ticksSpentOnPath = 0;
    }

    //проигрывание анимаций (и куча флагов типа isInAir и isLandingAnimationPlaying) 
    //запутывает сильно, надо рефакторить
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
                            if (!isLandingAnimationPlaying)
                            {
                                animator.Play("mobster_run");
                            }
                            
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
                        if (jumpSteer)
                        {
                            
                        }
                        else
                        {
                            if (!isLandingAnimationPlaying)
                            {
                                animator.Play("mobster_run");
                            }
                        }
                        
                    }

                    break;
                }
            case (Goal.Investigate):
                {
                    if (followPath)
                    {
                        physicalAction = PhysicalAction.Walk;
                        if (isLandingAnimationPlaying)
                        {
                            return;
                        }
                        if (jumpSteer)
                        {
                            animator.Play("mobster_jump");
                        }
                        else
                        {
                            animator.Play("mobster_run");
                        }
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

    protected override void OnDeath(GameObject killer)
    {
        base.OnDeath(killer);
        collider.attachedRigidbody.isKinematic = true;
        collider.isTrigger = true;
        animator.Play("mobster_death");
    }

    void RealizeLifeIsPointless()
    {
        isThinkingAboutLife = false;
    }

    protected override void DealDamage()
    {
        base.DealDamage();
        target.GetComponent<PlayerController>().rb.AddForce(new Vector2(directionX * 800f, 0f));
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

    protected override void ProcessPathInstructions()
    {
        (float, float) instructions = Pathfinder.GetPathInstructions(currentPathNode);
        float new_directionX = instructions.Item1 > 0 ? 1f : -1f;
        UpdateDirectionX(new_directionX);

        if (instructions.Item2 != 0f)
        {
            jumpSteerVelocityX = Mathf.Abs(instructions.Item1);
            jumpSteer = true;
            animator.speed = 1f;
            isLandingAnimationPlaying = false;
            animator.Play("mobster_jump");
            StartCoroutine(Jump(instructions.Item2));
        }
        else
        {
            jumpSteer = false;
        }
    }

    private IEnumerator Jump(float jumpForce)
    {
        yield return new WaitForSeconds(0.1f);
        Debug.LogWarning("Coroutine Started");
        Invoke("StopAnimator", 8*Time.fixedDeltaTime);
        rb.AddForce(new Vector2(0.0f, jumpForce));
    }

    private void StopAnimator()
    {
        isInAir = true;
        animator.speed = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "CyanWorld" || collision.gameObject.tag == "MagentaWorld" ||
            collision.gameObject.tag == "GreenWorld")
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
