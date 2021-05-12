using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    public enum AttackSide
    {
        Left,
        Right
    }

    public AttackSide side;
    PlayerController parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            parent.OnEnterAttackRange(collision.gameObject.GetComponent<Enemy>(), side);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            parent.OnLeaveAttackRange(collision.gameObject.GetComponent<Enemy>(), side);
        }
    }
}
