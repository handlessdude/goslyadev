using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompRange : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.isTrigger)
            PlayerController.isStopmAllowed = false;   
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
            PlayerController.isStopmAllowed = true; 
    }
}
