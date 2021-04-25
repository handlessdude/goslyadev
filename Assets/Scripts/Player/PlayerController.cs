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
    //2^(ИД слоя), поэтому 256
    public int groundLayer = 256;

    public float movementSpeed = 6.0f;
    public float jumpForce = 700.0f;



    // -1 — влево, 1 — вправо, 0 — на месте.
    float horizontalDirection = 0.0f;

    float verticalDirection = 0.0f;
    
    bool isInAir = false;
    bool jumping = false;

    bool stepCooldown = false;
    float stepAudioLength = 0.33f;

    float checkRadius = 0.05f;

    TargetPosition cameraTargetPosition = TargetPosition.OnCharacter;

    //объекты создаются здесь, чтобы потом не создавать их каждый раз
    Vector3 CameraUpPosition = new Vector3(0, 3);
    Vector3 CameraDownPosistion = new Vector3(0, -3);
    Vector2 _currentVelocity;
    bool movementInput = false;
    bool flag = false;

    enum TargetPosition
    {
        Up,
        OnCharacter,
        Down
    }

    void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
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
    }

    //TODO: Доделать анимацию, когда персонаж стоит на маленьком ящике, нет анимации передвижения

    public void OnTriggerStay2D(Collider2D collision)
    {
        
        
        if ((collision.gameObject.tag == "Boxes") && (collision.gameObject.transform.GetChild(0).position.y > middle_ground.position.y))
        {
            print(middle_ground.position.y);
            print(collision.gameObject.transform.GetChild(0).localPosition.y);
            flag = true;
        }
            
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boxes")
        {
            print(middle_ground.position.y);
            print(collision.gameObject.transform.GetChild(0).localPosition.y);
            flag = false;
        }
            
    }


   
    void Update()
    {
        if (GameplayState.controllability == PlayerControllability.Full)
        {
            if (InputManager.GetKey(KeyAction.MoveLeft))
            {
                if (flag)
                    horizontalDirection = -0.6f;
                else
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
                if (flag)
                    horizontalDirection = 0.6f;
                else
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

            if (InputManager.GetKey(KeyAction.Jump) && !jumping && IsOnGround())
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

    //TODO: навести порядок во всех системах, которые затрагивает FixedUpdate, они все сделаны плохо
    private void FixedUpdate()
    {

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

    bool IsOnGround()
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
}

