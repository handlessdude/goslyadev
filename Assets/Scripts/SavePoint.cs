using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public SaveSystem SaveSystem;
    private void Start()
    {
        if (!SaveSystem)
        {
            SaveSystem = GameObject.Find("Main Camera").GetComponent<SaveSystem>();
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveSystem.SaveGame();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
