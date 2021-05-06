using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMiddleGround : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Boxes"))
            GameplayState.isUnderElevator = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Boxes"))
            GameplayState.isUnderElevator = false;
    }
}