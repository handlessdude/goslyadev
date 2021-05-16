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
		ReturnToSpawn,
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
		Walk
	}

	public enum Modifier
	{
		Discombobulated,
		Occupied
	}

	//СТАТЫ
	
	public List<Sense> senses = new List<Sense>() { Sense.Hearing, Sense.Sight };

	public int maxHealth = 50;
	public int damage = 5;
	public float walkingSpeed = 1f;
	public float runningSpeed = 4f;
	public float maxJumpForce = 800f;
	public float attackCooldown = 1f; //в секундах
	public float attackAnimationDuration = 1f;
	public float attackTelegraphDuration = 0.3f;

	//ПЕРЕМЕННЫЕ ДЛЯ КОРРЕКТНОЙ РАБОТЫ ИИ

	protected int health;
	protected bool isDead = false;
	protected bool isAttackOnCooldown = false;
	protected bool isAttackAnimationPlaying = false;
	protected GameObject target;
	protected Goal goal = Goal.Idle;
	protected List<Modifier> modifiers = new List<Modifier>();
	protected bool isPlayerInAttackRange = false;
	protected Dictionary<Sense, bool> isPlayerInSenseRange = new Dictionary<Sense, bool>();
	protected PhysicalAction physicalAction;

	//ПЕРЕМЕННЫЕ ДЛЯ КОНТРОЛЛЕРА
	protected float directionX = -1f;
	protected float jumpSteerVelocityX;
	protected bool jumpSteer = false;
	protected LinkedListNode<PathNode> currentPathNode;
	protected bool followPath = false;
	protected int ticksSpentOnNode = 0;
	protected float sqrMagnitudeToNode = float.MaxValue;

	//КОМПОНЕНТЫ
	protected Animator animator;
	protected Rigidbody2D rb;
	protected SpriteRenderer spriteRenderer;

	//РЕСУРСЫ
	Material default_material;
	Material hit_material;

	protected virtual void Start()
    {
		health = maxHealth;

		if (!rb)
        {
			rb = GetComponent<Rigidbody2D>();
		}

		if (!animator)
		{
			animator = GetComponent<Animator>();
		}

		if (!spriteRenderer)
        {
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		default_material = spriteRenderer.material;
		hit_material = Resources.Load<Material>("HitMaterial");
	}

    protected virtual void Update()
	{
		if (!isDead)
        {
			OnAI();
		}
	}

	protected virtual void FixedUpdate()
    {
		if (followPath)
        {
			bool isSignificantX = Mathf.Abs(currentPathNode.Value.pos.x - transform.position.x) > 0.2f;
			UpdateDirectionX(currentPathNode.Value.pos.x - transform.position.x > 0f ? 1f : -1f, isSignificantX);
			if (Pathfinder.IsNodeReached(transform.position, currentPathNode.Value) && Mathf.Abs(rb.velocity.y) < 0.01f)
			{
				currentPathNode = currentPathNode.Next;
				if (currentPathNode == null)
				{
					OnPathCompleted();
					return;
				}
				sqrMagnitudeToNode = ((Vector2)transform.position - currentPathNode.Value.pos).sqrMagnitude;
				ProcessPathInstructions();
			}
			else
			{
				ticksSpentOnNode += 1; //предотвращает застревание
				if (ticksSpentOnNode > 80)
				{
					ticksSpentOnNode = 0;
					OnFailedToFollowPath();
					return;
				}

				//костыль!
				//исправляет ситуацию, когда моб промахнулся с прыжком и пытается
				//горизонтальным движением дойти до нужной ноды, но не может
				if (!isSignificantX && Mathf.Abs(currentPathNode.Value.pos.y - transform.position.y) > 2.5f)
                {
					OnFailedToFollowPath();
                }

				//заставляет пересчитать путь, если перескочили ноду
				/*if (((Vector2)transform.position - currentPathNode.Value.pos).sqrMagnitude > sqrMagnitudeToNode * 8)
                {
					OnFailedToFollowPath();
					return;
                }*/
			}
		}

		if (jumpSteer)
        {
			rb.velocity = new Vector2(jumpSteerVelocityX * directionX, rb.velocity.y);
		}
		else
        {
			switch (physicalAction)
			{
				case PhysicalAction.Run:
					{
						rb.velocity = new Vector2(runningSpeed * directionX, rb.velocity.y);
						break;
					}
				case PhysicalAction.Walk:
					{
						rb.velocity = new Vector2(walkingSpeed * directionX, rb.velocity.y);
						break;
					}
				case PhysicalAction.Stay:
					{
						rb.velocity = rb.velocity = new Vector2(0f, rb.velocity.y);
						break;
					}
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
		physicalAction = PhysicalAction.Stay;
		OnDeath(killer);
	}

	public bool IsDead()
    {
		return isDead;
    }

	protected virtual void OnDeath(GameObject killer)
	{
		
	}

	protected virtual void Attack()
    {
		isAttackOnCooldown = true;
		isAttackAnimationPlaying = true;
		Invoke("ResetAttackCooldown", attackCooldown);
		Invoke("DealDamage", attackTelegraphDuration);
		Invoke("OnAttackAnimationEnd", attackAnimationDuration);
	}

	protected virtual void DealDamage()
    {
		if (isPlayerInAttackRange && !isDead)
        {
			target.GetComponent<PlayerStats>().OnHit(gameObject, damage);
		}
	}

	protected virtual void OnAttackAnimationEnd()
    {
		isAttackAnimationPlaying = false;
    }

	protected virtual void OnLostTarget()
    {

    }

	protected virtual void OnInvestigationFailed()
	{

	}


	protected virtual void StartFollowingPath(Vector2 tpos)
    {
		LinkedList<PathNode> path = Pathfinder.FindPath(transform.position, tpos, rb, maxJumpForce, runningSpeed);
		if (path == null)
        {
			OnFailedToFollowPath();
			return;
        }
		currentPathNode = path.First.Next;
		followPath = true;
		ProcessPathInstructions();
	}

	protected virtual void ProcessPathInstructions()
    {
		(float, float) instructions = Pathfinder.GetPathInstructions(currentPathNode);
		float new_directionX = instructions.Item1 > 0 ? 1f : -1f;
		UpdateDirectionX(new_directionX);

		if (instructions.Item2 != 0f)
		{
			jumpSteerVelocityX = Mathf.Abs(instructions.Item1);
			jumpSteer = true;
			rb.AddForce(new Vector2(0.0f, instructions.Item2));
		}
		else
        {
			jumpSteer = false;
        }
	}

	protected virtual void UpdateDirectionX(float new_directionX, bool isSignificant = true)
    {
		if (new_directionX != directionX)
		{
			directionX = new_directionX;
		}
		//это можно было бы делать через spriteRenderer.flipX,
		//но тогда были бы беды с триггерами
		if (isSignificant)
		{
			transform.localScale = new Vector3(-directionX*Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		}
	}

	protected virtual void AbandonPath()
    {
		followPath = false;
		jumpSteer = false;
		ticksSpentOnNode = 0;
		currentPathNode = null;
		physicalAction = PhysicalAction.Stay;
	}

	protected virtual void OnPathCompleted()
    {
		AbandonPath();
    }

	protected virtual void OnFailedToFollowPath()
	{
		AbandonPath();
	}

	protected virtual void ResetAttackCooldown()
    {
		isAttackOnCooldown = false;
	}

	public virtual void OnHit(GameObject player, int damage)
	{
		if (!isDead)
        {
			int deltaHP = health - damage;
			if (deltaHP > 0)
			{
				health = deltaHP;
			}
			else
			{
				Die(player);
			}

			CancelInvoke("RestoreDefaultMaterial");
			spriteRenderer.material = hit_material;
			Invoke("RestoreDefaultMaterial", 0.1f);
		}
	}

	void RestoreDefaultMaterial()
	{
		spriteRenderer.material = default_material;
	}

	public virtual void OnEnterAttackRange(GameObject player)
	{
		Debug.Log("PLAYER IN ATTACK RANGE");
		isPlayerInAttackRange = true;
		target = player;
	}

	public virtual void OnLeaveAttackRange(GameObject player)
	{
		isPlayerInAttackRange = false;
	}

	public virtual void OnEnterSenseRange(GameObject player, Sense s)
    {
		target = player;
		if (senses.Contains(s))
        {
			isPlayerInSenseRange[s] = true;
			
        }
    }

	public virtual void OnLeaveSenseRange(GameObject player, Sense s)
	{
		if (senses.Contains(s))
		{
			isPlayerInSenseRange[s] = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (currentPathNode != null)
		{
			LinkedListNode<PathNode> node = currentPathNode;
			while (node != null)
			{
				if (node.Value.jumpNode)
				{
					Gizmos.DrawIcon(node.Value.pos, "sv_icon_dot11_pix16_gizmo", true);
				}
				else
				{
					Gizmos.DrawIcon(node.Value.pos, "sv_icon_dot13_pix16_gizmo", true);
				}
				node = node.Next;
			}
		}
	}
}
