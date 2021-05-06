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
		Run,
		Walk,
		Stay
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

	//ПЕРЕМЕННЫЕ ДЛЯ КОНТРОЛЛЕРА
	protected float directionX;

    protected virtual void Start()
    {
		health = maxHealth;
    }

    protected virtual void Update()
	{
		OnAI();
	}

	protected virtual void FixedUpdate()
    {

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

	public virtual void OnEnterAttackRange(GameObject player)
	{
		isPlayerInAttackRange = true;
		target = player;
		Debug.Log("Attack Range");
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
