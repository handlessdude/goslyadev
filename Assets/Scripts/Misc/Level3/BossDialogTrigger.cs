using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDialogTrigger : MonoBehaviour
{
    [HideInInspector]
    public BossDialog parent;
    private void Start()
    {
        if (!parent)
        {
            parent = GetComponentInParent<BossDialog>();
            if (!parent)
            {
                Debug.LogError("Boss Trigger couldn't find its parent's Enemy Script!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            parent.OnEnterDialogTrigger(collision.gameObject);
        }
    }
}
