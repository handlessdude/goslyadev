using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public bool isTriggered = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Boxes"))
        {
            isTriggered = true;
        }
    }


}
