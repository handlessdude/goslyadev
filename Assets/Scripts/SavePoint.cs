using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : SaveSystem
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveGame();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
