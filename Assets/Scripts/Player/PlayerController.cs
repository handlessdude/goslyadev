﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public Transform left_ground;
    public Transform right_ground;
    public Transform middle_ground;
    public Transform middle_ground1;
    public Transform middle_ground2;
    public Transform cameraTarget;
    public Footsteps footsteps;
    public AbilitySound abilitySound;
    public GameObject wall;
    public InGameUIController ingameUI;
    PlayerStats playerStats;
    GameObject wallClone;
    
    //2^(ИД слоя), поэтому 256
    public int groundLayer = 256;
    public float movementSpeed = 6.0f;
    public float jumpForce = 700.0f;

    int damage = 20;

    public bool areAbilitiesAllowed = false;

    //характеристики Dash
    float dashForce = 5000.0f;
    bool isDashAllowed = true;


    //характеристики Stopm
    bool isStompAllowed = true;
    float stompDuration = 1f;
    float stompTimeLeft = 0f; //в секундах
    float stompCooldown = 4f;
    //Характеристики Hit
    bool isHitAllowed = true;

    WorldSwitcher worldSwitcher;

    // -1 — влево, 1 — вправо, 0 — на месте.
    float horizontalDirection = 0.0f;

    float verticalDirection = 0.0f;
    
    bool isInAir = false;
    bool jumping = false;

    bool stepCooldown = false;
    float stepAudioLength = 0.33f;

    float checkRadius = 0.05f;

    TargetPosition cameraTargetPosition = TargetPosition.OnCharacter;

    Vector3 CameraUpPosition = new Vector3(0, 3);
    Vector3 CameraDownPosistion = new Vector3(0, -3);
    Vector2 _currentVelocity;
    bool movementInput = false;

    //надо бы на одно нормальную структуру данных заменить
    List<Enemy> enemies_on_the_left = new List<Enemy>();
    List<Enemy> enemies_on_the_right = new List<Enemy>();

    float nonzero_horizontal_direction = 0f;

    enum TargetPosition
    {
        Up,
        OnCharacter,
        Down
    }

    void Start()
    {
        if (!ingameUI)
        {
            ingameUI = FindObjectOfType<InGameUIController>();
        }
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (!worldSwitcher)
        {
            worldSwitcher = GetComponent<WorldSwitcher>();
        }

        if (!left_ground)
        {
            left_ground = transform.Find("LeftGround");
        }

        if (!right_ground)
        {
            right_ground = transform.Find("RightGround");
        }

        if (!middle_ground)
        {
            middle_ground = transform.Find("MiddleGround");
        }

        if (!middle_ground1)
        {
            middle_ground1 = transform.Find("MiddleGround1");
        }

        if (!middle_ground2)
        {
            middle_ground2 = transform.Find("MiddleGround2");
        }

        if (!animator)
        {
            animator = GetComponent<Animator>();
        }

        if (!cameraTarget)
        {
            cameraTarget = transform.Find("CameraTarget");
        }
        
        if (!footsteps)
        {
            footsteps = GetComponent<Footsteps>();
        }

        if (!abilitySound)
        {
            abilitySound = GetComponent<AbilitySound>();
        }

        if (!playerStats)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    //Реализация абилки Stopm
    void Stomp()
    {
        abilitySound.Sound("Stomp");
        animator.SetBool("Stomp", true);
        Invoke("CreateClone", 0.2f);
        GameplayState.controllability = PlayerControllability.InDialogue;
        Invoke("DeleteClone", stompDuration);
        isStompAllowed = false;
        stompTimeLeft = stompCooldown;
        ingameUI.UpdateStompCooldownBar(1f);
    }

    void StompCoolDown() => isStompAllowed = true;
    void StompCooledDown()
    {
        isStompAllowed = true;
        ingameUI.UpdateStompCooldownBar(0f);
    }

    void DeleteClone()
    {
        Destroy(wallClone);
        GameplayState.controllability = PlayerControllability.Full;
        animator.SetBool("Stomp", false);
    }

    void CreateClone()
    {
        var playerPos = gameObject.transform.position;
        //0.5 добавил из-за кривого спрайта
        wallClone = Instantiate(wall, new Vector3(playerPos.x, playerPos.y + 0.5f, 0), Quaternion.identity);
        wallClone.transform.parent = transform;
    }
    //

    //Реализация абилки Dash
    void Dash()
    {
        if (horizontalDirection != 0.0f)
        {
            abilitySound.Sound("Dash");
            animator.SetBool("Dash", true);
            if (WorldSwitcher.currentWorld == WorldSwitcher.World.cyan)
                dashForce = 10000;
            if (nonzero_horizontal_direction > 0)
                rb.AddForce(new Vector2(dashForce, 0));
            if (nonzero_horizontal_direction < 0)
                rb.AddForce(new Vector2(-dashForce, 0));
            Invoke("StopDash", 0.5f);
            isDashAllowed = false;
            Invoke("DashCoolDown", 0.5f);
            dashForce = 5000;
        }
    }

    void DashCoolDown() => isDashAllowed = true;
    void StopDash() => animator.SetBool("Dash", false);
    //

    //Реализация абилки Hit
    
    void Hit()
    {
        abilitySound.Sound("Hit");
        animator.SetBool("Hit", true);
        Invoke("StopHit", 0.5f);
        isHitAllowed = false;
        Invoke("DealDamage", 1 / 6f);
        Invoke("HitCoolDown", 0.5f);
    }

    void HitCoolDown() => isHitAllowed = true;
    void StopHit() => animator.SetBool("Hit", false);

    //
    void Update()
    {
        nonzero_horizontal_direction = horizontalDirection == 0f ? nonzero_horizontal_direction : horizontalDirection;
        if ((GameplayState.controllability == PlayerControllability.Full || GameplayState.controllability == PlayerControllability.FirstDialog) && !GameplayState.isPaused)
        {
            if (InputManager.GetKey(KeyAction.MoveLeft))
            {
                horizontalDirection = -1.0f;
                animator.SetFloat(475924382, horizontalDirection); //по сути animator.SetFloat("Horizontal", horizontalDirection);
                                                                  //TODO: movementInput это костыль. Надо убрать.
                movementInput = true;
                if (IsOnGround())
                {
                    PlayStepSound();
                }
                ResetCamera();
            }
            else if (InputManager.GetKey(KeyAction.MoveRight))
            {
                horizontalDirection = 1.0f;
                animator.SetFloat(475924382, horizontalDirection);
                movementInput = true;
                if (IsOnGround())
                {
                    PlayStepSound();
                }
                ResetCamera();
            }
            else
            {
                //сделано для того, чтобы не присваивать 0 в поле "Horizontal" в аниматоре,
                //ибо если это делать, то при остановке персонаж будет поворачиваться в дефолтное положение,
                //то есть вправо, что не имеет смысла, если игрок до остановки шёл влево.

                if (horizontalDirection != 0)
                {
                    animator.SetFloat(475924382, horizontalDirection / 100);
                }
                horizontalDirection = 0.0f;
            }

            if (SceneManager.GetSceneByBuildIndex(2) != SceneManager.GetActiveScene())
            {
                if (areAbilitiesAllowed)
                {
                    if (InputManager.GetKeyDown(KeyAction.Stomp) && !jumping && isStompAllowed)
                    {
                        Stomp();
                    }

                    if (InputManager.GetKeyDown(KeyAction.Dash) && isDashAllowed)
                    {
                        Dash();
                    }

                    if (InputManager.GetKeyDown(KeyAction.Hit) && isHitAllowed)
                    {
                        Hit();
                    }
                }
            }
            
            if (InputManager.GetKey(KeyAction.LookUp) && !jumping && !movementInput)
            {
                LookUp();
            }

            if (InputManager.GetKey(KeyAction.LookDown) && !jumping && !movementInput)
            {
                LookDown();
            }

            if (InputManager.GetKeyUp(KeyAction.LookUp))
            {
                ResetCamera(TargetPosition.Up);
            }
           
            if (InputManager.GetKeyUp(KeyAction.LookDown))
            {
                ResetCamera(TargetPosition.Down);
            }

            if (InputManager.GetKey(KeyAction.Jump) && !jumping && IsOnGround() && !GameplayState.IsMovingBox)
            {
                Jump();
                ResetCamera();
                ForcePlayStepSound();
            }
            
            
        } //ветка else здесь костыль
        else
        {
            if (horizontalDirection != 0)
            {
                animator.SetFloat(475924382, horizontalDirection / 100);
            }
            horizontalDirection = 0.0f;
        }

        movementInput = false;
    }

    public void RotateToVector(Vector2 position)
    {
        animator.SetFloat(475924382, (position.x - transform.position.x)/1000f);
    }

    void ProcessStompCooldown()
    {
        if (!isStompAllowed)
        {
            stompTimeLeft -= Time.fixedDeltaTime;
            ingameUI.UpdateStompCooldownBar(stompTimeLeft / stompCooldown);

            float stompTimePassed = stompCooldown - stompTimeLeft;

            if (WorldSwitcher.currentWorld == WorldSwitcher.World.green)
            {
                if (stompTimePassed < stompDuration)
                {
                    //постепенно восстанавливает 10 ХП или около того
                    if (stompTimePassed % (1f/stompDuration/20f) < Time.fixedDeltaTime)
                    {
                        playerStats.Health += 1;
                    }
                }
            }

            if (stompTimeLeft < Time.fixedDeltaTime)
            {
                StompCooledDown();
            }
        }
    }

    //TODO: навести порядок во всех системах, которые затрагивает FixedUpdate, они все сделаны плохо
    private void FixedUpdate()
    {

        ProcessStompCooldown();

        //float targetPos = rb.position.x + horizontalDirection * movementSpeed * Time.deltaTime;

        //rb.position = Vector2.SmoothDamp(rb.position, new Vector2(targetPos, rb.position.y), ref _currentVelocity, 0.05f);
        //rb.position = new Vector2(rb.position.x + horizontalDirection*movementSpeed * Time.deltaTime, rb.position.y);
        rb.velocity = new Vector2(horizontalDirection * movementSpeed, rb.velocity.y);
        bool isOnGround = IsOnGround();
        /*if (jumping)
        {
            if (!isOnGround)
            {
                isInAir = true;
            }
            else
            {
                if (isInAir && isOnGround)
                {
                    
                }
            }
        }*/

        /*if (isInAir && isOnGround)
        {
            ForcePlayStepSound();
            isInAir = false;
            //animator.SetBool(125937960, false);
        }*/

        
        //временное решение
        if (isOnGround && jumping && isInAir)
        {
            Grounded();
            animator.SetBool(125937960, false);
        }

        if (GameplayState.isLoaded)
        {
            transform.position = GameplayState.NewPositionPlayer;


            foreach (var boxes in GameplayState.BoxesPosition)
            {
                GameObject.Find(boxes.Key).transform.position = new Vector3(boxes.Value.Item1, boxes.Value.Item2, 0);
            }

            foreach (var item in GameplayState.deletedObjectsList)
            {
                Destroy(GameObject.Find(item));
            }
            GameplayState.isLoaded = false;
        }
    }

    void ResetStep()
    {
        CancelInvoke("ResetStep");
        stepCooldown = false;
    }

    void InAir()
    {
        CancelInvoke("InAir");
        isInAir = true;
        Debug.Log("IN AIR " + Random.Range(1, 100));
    }

    void PlayStepSound()
    {
        if (!stepCooldown && footsteps)
        {
            footsteps.Step();
            stepCooldown = true;
            Invoke("ResetStep", stepAudioLength);
        }
    }

    void ForcePlayStepSound()
    {
        if (footsteps)
        {
            footsteps.Step();
        }
    }

    void LookUp()
    {
        cameraTargetPosition = TargetPosition.Up;
        cameraTarget.localPosition = CameraUpPosition;
    }

    void LookDown()
    {
        cameraTargetPosition = TargetPosition.Down;
        cameraTarget.localPosition = CameraDownPosistion;
    }

    void ResetCamera(TargetPosition tp)
    {
        if (tp == cameraTargetPosition)
        {
            cameraTargetPosition = TargetPosition.OnCharacter;
            cameraTarget.localPosition = Vector3.zero;
        }
    }

    void ResetCamera()
    {
        if (cameraTargetPosition != TargetPosition.OnCharacter)
        {
            cameraTargetPosition = TargetPosition.OnCharacter;
            cameraTarget.localPosition = Vector3.zero;
        }
    }
    public bool IsOnGround()
    {
        if (isInAir)
        {
            return Physics2D.OverlapAreaAll(middle_ground1.position, middle_ground2.position, groundLayer).Any(x => !x.isTrigger);
        }
        return (Physics2D.OverlapCircleAll(middle_ground.position, checkRadius, groundLayer).Any() ||
            Physics2D.OverlapCircleAll(left_ground.position, checkRadius, groundLayer).Any() ||
            Physics2D.OverlapCircleAll(right_ground.position, checkRadius, groundLayer).Any());
    }

    void Jump()
    {
        Invoke("InAir", 0.1f);
        rb.AddForce(new Vector2(0.0f, jumpForce));
        jumping = true;
        animator.SetBool(125937960, true);
    }

    void Grounded()
    {
        Debug.Log("GROUNDED " + Random.Range(1, 100));
        //ForcePlayStepSound();
        rb.velocity = new Vector2(rb.velocity.x, 0);
        jumping = false;
        isInAir = false; 
    }


    public void OnEnterAttackRange(Enemy enemy, PlayerAttackRange.AttackSide side)
    {
        if (side == PlayerAttackRange.AttackSide.Left)
        {
            enemies_on_the_left.Add(enemy);
        }
        else
        {
            enemies_on_the_right.Add(enemy);
        }
    }

    public void OnLeaveAttackRange(Enemy enemy, PlayerAttackRange.AttackSide side)
    {
        if (side == PlayerAttackRange.AttackSide.Left)
        {
            enemies_on_the_left.Remove(enemy);
        }
        else
        {
            enemies_on_the_right.Remove(enemy);
        }
    }

    void DealDamage()
    {
        int _damage = damage;
        if (WorldSwitcher.currentWorld == WorldSwitcher.World.magenta)
        {
            _damage = Mathf.FloorToInt((float)damage * 1.5f);
        }

        if (nonzero_horizontal_direction < 0f)
        {
            foreach(Enemy e in enemies_on_the_left)
            {
                e.OnHit(gameObject, _damage);
            }
        }
        else
        {
            foreach (Enemy e in enemies_on_the_right)
            {
                e.OnHit(gameObject, _damage);
            }
        }
    }
}

