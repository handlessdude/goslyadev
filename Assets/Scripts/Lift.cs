using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed = 100f;
    bool onLift = false;
    private void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
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

    private void Update()
    {
        if (onLift)
           transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * 2 * Time.deltaTime);
        
    }

}
