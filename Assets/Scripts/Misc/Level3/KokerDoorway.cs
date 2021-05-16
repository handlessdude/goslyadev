using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KokerDoorway : Doorway
{

    // Update is called once per frame
    protected override void Update()
    {
        if (GameplayState.isAccessToFatherGranted)
        {
            base.Update();
        }
    }
}
