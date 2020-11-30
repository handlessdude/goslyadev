using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintText : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI text;
    void Start()
    {
        if (!text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Color color = text.color;
        color.a = 0.0f;
        text.color = color;
    }

    void Update()
    {
        if (player)
        {
            float dist = Vector2.Distance(gameObject.transform.parent.position, player.transform.position);
            if (dist < 5)
            {
                Color color = text.color;
                color.a = 1 - dist*dist / 25;
                text.color = color;
            }
        }
    }
}
