using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    Rigidbody2D rb;
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
    }


    public void OnTriggerEnter2D(Collider2D collision)
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
            if (transform.position.y > targetTop.position.y)
            {
                rb.velocity = new Vector2(0, 0);
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
            if (transform.position.y < targetBottom.position.y)
            {
                rb.bodyType = RigidbodyType2D.Static;
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = new Vector2(rb.velocity.x, -moveSpeed * 2 * Time.fixedDeltaTime);
            }
        }
    }
}
