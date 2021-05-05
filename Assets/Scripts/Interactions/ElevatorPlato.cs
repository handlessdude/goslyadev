using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlato : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Boxes"))
            GameplayState.UnderElevator = true;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Boxes"))
            GameplayState.UnderElevator = false;
    }
}