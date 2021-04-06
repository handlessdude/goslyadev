using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Plate : MonoBehaviour
{
    public bool isTriggered = false;
    public Light2D Light;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Boxes"))
        {
            isTriggered = true;
            Light.color = new Color(0.3f,1f,0.45f,1f);
        }
    }


}
