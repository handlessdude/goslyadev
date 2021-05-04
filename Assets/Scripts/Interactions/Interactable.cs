using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected int playerCollidersCount = 0;
    protected GameObject player;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision.gameObject;
            playerCollidersCount += 1;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerCollidersCount -= 1;
        }
    }

    protected virtual void Update()
    {
        if (playerCollidersCount != 0 && GameplayState.isPaused == false && ((GameplayState.controllability == PlayerControllability.Full) || (GameplayState.controllability == PlayerControllability.FirstDialog))
            && InputManager.GetKeyDown(KeyAction.Action))
        {
            Action();
        }
    }

    protected virtual void Action()
    {
        
    }
}
