using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Doorway : Interactable
{
    public FadeToBlack fade;
    public Transform destination;
    public GameObject cam;
    bool cooldown = false;
    public Plate Plate;
    void Start()
    {
    }

    protected override void Action()
    {
        base.Action();
        if (Plate.isTriggered)
        {
            if (!cooldown)
            {
                CancelInvoke();
                fade.FadeTransition(out float halfTime);
                Invoke("Teleport", halfTime);
                cooldown = true;
            }
        }    
    }

    void Teleport()
    {
        CancelInvoke("Teleport");
        cooldown = false;
        //потому что менять Z-координату у камеры это игрушки дьявола!
        Vector2 destVector = new Vector2(destination.position.x, destination.position.y);
        cam.transform.position = destVector;
        player.transform.position = destVector;
    }
}
