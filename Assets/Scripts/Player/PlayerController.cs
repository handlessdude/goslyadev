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
    }

    void Update()
    {
        //TODO: сделать нормальную систему назначения клавиш

        if (Input.GetKey("a"))
        {
            horizontalDirection = -1.0f;
            animator.SetFloat(475924382, horizontalDirection); //по сути animator.SetFloat("Horizontal", horizontalDirection);
        }
        else if (Input.GetKey("d"))
        {
            horizontalDirection = 1.0f;
            animator.SetFloat(475924382, horizontalDirection);
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

        if (Input.GetButtonDown("Jump") && !jumping && IsOnGround())
        {
            Jump();
        }
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
