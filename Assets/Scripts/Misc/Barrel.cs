using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Interactable
{ 
    protected override void Action()
    {
        base.Action();
        GameplayState.barrels += 1;
        GameplayState.deletedObjectsList.Add(gameObject.name);
        Destroy(gameObject);
    }
}
