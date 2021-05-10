using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public int health;

    public int maxHealth;

    [HideInInspector]
    public InGameUIController ingameUI;

    void Start()
    {
        health = maxHealth;

        if (!ingameUI)
        {
            ingameUI = FindObjectOfType<InGameUIController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnHit(GameObject source, int damage)
    {
        Debug.Log("HIT");
        int deltaHP = health - damage;
        ingameUI.UpdateHP(deltaHP, maxHealth);
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
