using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Interactable
{ 
    protected override void Action()
    {
        base.Action();
        GameplayState.boards += 1;
        GameplayState.deletedObjectsList.Add(gameObject.name);
        Destroy(gameObject);
    }
}
