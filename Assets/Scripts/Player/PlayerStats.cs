using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public int health;

    public int maxHealth;


    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit(GameObject source, int damage)
    {
        int deltaHP = health - damage;
        if (deltaHP > 0)
        {
            health = deltaHP;
        }
        else
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {

    }
}
