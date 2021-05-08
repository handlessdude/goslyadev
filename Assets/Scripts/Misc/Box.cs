using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    Rigidbody2D rb;
    HingeJoint2D hinge;
    Animator animator;
    GameObject Player;
    int countAction = 0;
    PlayerController playerController;

    void Start()
    { 
        if (!rb)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        if (!hinge)
        {
            hinge = gameObject.GetComponent<HingeJoint2D>();
        }

        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        if (!animator)
        {
            animator = Player.GetComponent<Animator>();
            print(animator);
        }

        if (!playerController)
        {
            playerController = Player.GetComponent<PlayerController>();
        }

        hinge.connectedBody = Player.GetComponent<Rigidbody2D>();
    }

    void Freeze()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    protected override void Action()
    {
        base.Action();
        CancelInvoke("Freeze");
        if (countAction == 0 && playerController.IsOnGround())
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            countAction += 1;
            hinge.enabled = true;
            GameplayState.IsMovingBox = true;
            if (player.transform.position.x < gameObject.transform.position.x)
                animator.SetBool("MovingBoxLeft", true);
            else
                animator.SetBool("MovingBoxRight", true);
        }
        else
        {
            countAction = 0;
            hinge.enabled = false;
            GameplayState.IsMovingBox = false;
            Invoke("Freeze", 0.5f);
            //Можно надо будет исправить
            animator.SetBool("MovingBoxLeft", false);
            animator.SetBool("MovingBoxRight", false);
        }
    }
}
