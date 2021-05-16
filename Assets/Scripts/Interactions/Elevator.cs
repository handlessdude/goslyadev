using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource ElevatorSound;
    public AudioSource music;
    public float moveSpeed = 2f;
    public Transform targetTop;
    public Transform targetBottom;

    bool onLift = false;


    private void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (!ElevatorSound)
        {
            ElevatorSound = GetComponent<AudioSource>();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            onLift = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            onLift = false;
    }

    private void FixedUpdate()
    {
        if (onLift)
        {
            music.mute = true;
            ElevatorSound.mute = false;
            if (transform.position.y > targetTop.position.y || GameplayState.isUnderElevator) 
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * 2 * Time.fixedDeltaTime);
            }
        }
        else
        {
            music.mute = false;
            ElevatorSound.mute = true;
            if (transform.position.y < targetBottom.position.y || GameplayState.isUnderElevator)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = new Vector2(rb.velocity.x, -moveSpeed * 2 * Time.fixedDeltaTime);
            }
        }
    }
}
