using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    int health;

    public int Health
    { get { return health; }
        set {
            if (value > maxHealth)
            {
                health = maxHealth;
            }
            else if (value <= 0)
            {
                health = 0;
                OnDeath();
            }
            else
            {
                health = value;
            }
            ingameUI.UpdateHP(health, maxHealth);
        } }



    public int maxHealth;

    [HideInInspector]
    SpriteRenderer spriteRenderer;

    [HideInInspector]
    public InGameUIController ingameUI;

    Material default_material;
    Material hit_material;

    void Start()
    {
        health = maxHealth;

        if (!ingameUI)
        {
            ingameUI = FindObjectOfType<InGameUIController>();
        }

        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        default_material = spriteRenderer.material;
        hit_material = Resources.Load<Material>("HitMaterial");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnHit(GameObject source, int damage)
    {
        Debug.Log("HIT");
        if (GameplayState.controllability == PlayerControllability.InDialogue)
            return;
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
        CancelInvoke("RestoreDefaultMaterial");
        spriteRenderer.material = hit_material;
        Invoke("RestoreDefaultMaterial", 0.1f);
    }

    void RestoreDefaultMaterial()
    {
        spriteRenderer.material = default_material;
    }

    public void OnDeath()
    {
        ingameUI.DisplayDeathScreen();
    }
}
