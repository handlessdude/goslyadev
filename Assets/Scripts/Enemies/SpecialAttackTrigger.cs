using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackTrigger : MonoBehaviour
{
    [HideInInspector]
    public SawBot parent;


    private void Start()
    {
        if (!parent)
        {
            parent = GetComponentInParent<SawBot>();
            if (!parent)
            {
                Debug.LogError("Enemy Trigger couldn't find its parent's Enemy Script!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            parent.OnEnterSpecialAttackRange(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            parent.OnLeaveSpecialAttackRange(collision.gameObject);
        }
    }
}
