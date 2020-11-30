using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public Transform left_ground;
    public Transform right_ground;
    public Transform middle_ground;
    public Transform cameraTarget;
    //2^(ИД слоя), поэтому 256
    public int groundLayer = 256;

    public float movementSpeed = 4.0f;

    // -1 — влево, 1 — вправо, 0 — на месте.
    float horizontalDirection = 0.0f;

    float verticalDirection = 0.0f;
    float jumpForce = 600.0f;
    bool isInAir = false;
    bool jumping = false;

    float checkRadius = 0.05f;

    TargetPosition cameraTargetPosition = TargetPosition.OnCharacter;

    //объекты создаются здесь, чтобы потом не создавать их каждый раз
    Vector3 CameraUpPosition = new Vector3(0, 3);
    Vector3 CameraDownPosistion = new Vector3(0, -3);
    bool movementInput = false;

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

        if (!animator)
        {
            animator = GetComponent<Animator>();
        }

        if (!cameraTarget)
        {
            cameraTarget = transform.Find("CameraTarget");
        }
    }

    void Update()
    {
        if (InputManager.GetKey(KeyAction.MoveLeft))
        {
            horizontalDirection = -1.0f;
            animator.SetFloat(475924382, horizontalDirection); //по сути animator.SetFloat("Horizontal", horizontalDirection);
            //TODO: movementInput это костыль. Надо убрать.
            movementInput = true;
            ResetCamera();
        }
        else if (InputManager.GetKey(KeyAction.MoveRight))
        {
            horizontalDirection = 1.0f;
            animator.SetFloat(475924382, horizontalDirection);
            movementInput = true;
            ResetCamera();
        }
        else
        {
            //сделано для того, чтобы не присваивать 0 в поле "Horizontal" в аниматоре,
            //ибо если это делать, то при остановке персонаж будет поворачиваться в дефолтное положение,
            //то есть вправо, что не имеет смысла, если игрок до остановки шёл влево.
            
            if (horizontalDirection != 0)
            {
                animator.SetFloat(475924382, horizontalDirection/100); 
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
        }

        movementInput = false;
    }

    private void FixedUpdate()
    {
        rb.position = new Vector2(rb.position.x + horizontalDirection*movementSpeed * Time.deltaTime, rb.position.y);

        if (jumping)
        {
            if (!IsOnGround())
            {
                isInAir = true;
            }
            else
            {
                if (isInAir && IsOnGround())
                {
                    Grounded();
                }
            }
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
        return (Physics2D.OverlapCircleAll(middle_ground.position, checkRadius, groundLayer).Any() ||
            Physics2D.OverlapCircleAll(left_ground.position, checkRadius, groundLayer).Any() ||
            Physics2D.OverlapCircleAll(right_ground.position, checkRadius, groundLayer).Any()) && rb.velocity.y < 0.0001f;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0.0f, jumpForce));
        jumping = true;
        animator.SetBool(125937960, true);
    }

    void Grounded()
    {
        jumping = false;
        isInAir = false;
        animator.SetBool(125937960, false);
    }
}
