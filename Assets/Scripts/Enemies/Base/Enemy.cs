using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public enum Sense {
		Sight,
		Hearing
	}

	public enum Goal
	{
		Idle,
		Patrol,
		Investigate,
		Intimidate,
		Attack,
		Chase,
		Die
	}

    public enum PhysicalAction
    {
		Stay,
		Run,
		Walk,
		JumpSteer
	}

	public enum Modifier
	{
		Discombobulated,
		Occupied
	}

	//СТАТЫ
	
	public List<Sense> senses = new List<Sense>();

	public int maxHealth = 50;
	public int damage = 5;
	public float walkingSpeed = 3f;
	public float runningSpeed = 6f;
	public float maxJumpForce = 700f;

	//ПЕРЕМЕННЫЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ ИИ

	protected int health;
	protected bool isDead = false;
	protected GameObject target;
	protected Goal goal = Goal.Idle;
	protected List<Modifier> modifiers = new List<Modifier>();
	protected bool isPlayerInAttackRange = false;
	protected Dictionary<Sense, bool> isPlayerInSenseRange = new Dictionary<Sense, bool>();
	protected PhysicalAction physicalAction;

	//ПЕРЕМЕННЫЕ ДЛЯ КОНТРОЛЛЕРА
	protected float directionX;
	protected float jumpSteerVelocityX;
	protected Rigidbody2D rb;
	protected LinkedListNode<PathNode> currentPathNode;
	protected bool followPath = false;
	protected int ticksSpentOnNode = 0;

    protected virtual void Start()
    {
		health = maxHealth;

		if (!rb)
        {
			rb = GetComponent<Rigidbody2D>();
		}
    }

    protected virtual void Update()
	{
		OnAI();
	}

	protected virtual void FixedUpdate()
    {
        switch (physicalAction)
        {
			case PhysicalAction.Run:
                {
					rb.velocity = new Vector2(runningSpeed, rb.velocity.y);
					break;
                }
			case PhysicalAction.Walk:
				{
					rb.velocity = new Vector2(walkingSpeed, rb.velocity.y);
					break;
				}
			case PhysicalAction.JumpSteer:
				{
					rb.velocity = new Vector2(jumpSteerVelocityX * directionX, rb.velocity.y);
					break;
				}
			case PhysicalAction.Stay:
				{
					rb.velocity = rb.velocity = new Vector2(0f, rb.velocity.y);
					break;
				}
		}

    }

	protected virtual void OnAI()
	{

	}

	public virtual void Die(GameObject killer)
    {
		health = 0;
		isDead = true;
		OnDeath(killer);
	}

	public bool IsDead()
    {
		return isDead;
    }

	public virtual void OnDeath(GameObject killer)
	{

	}

	public virtual void Attack()
    {
		target.GetComponent<PlayerStats>().OnHit(gameObject, damage);
    }

	public virtual void OnHit(GameObject player, int damage)
	{
		int deltaHP = health - damage;
		if (deltaHP > 0) {
			health = deltaHP;
		}
		else
		{
			Die(player);
		}
	}

	public virtual void OnFailedToFollowPath()
    {
		followPath = false;
    }

	public virtual void OnEnterAttackRange(GameObject player)
	{
		isPlayerInAttackRange = true;
		target = player;
	}

	public virtual void OnLeaveAttackRange(GameObject player)
	{
		isPlayerInAttackRange = false;
	}

	public virtual void OnEnterSenseRange(GameObject player, Sense s)
    {
		if (senses.Contains(s))
        {
			isPlayerInSenseRange[s] = true;
			target = player;
        }
    }

	public virtual void OnLeaveSenseRange(GameObject player, Sense s)
	{
		if (senses.Contains(s))
		{
			isPlayerInSenseRange[s] = false;
		}
	}

}
