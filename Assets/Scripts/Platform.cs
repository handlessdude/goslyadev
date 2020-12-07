using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    SpriteRenderer sr;
    int playerColliders;
    static Color solidColor = new Color(1f, 1f, 1f, 1f);
    static Color transparentColor = new Color(1f, 1f, 1f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        if (!sr)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (!sr)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        SetFullOpacity();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (playerColliders == 0)
            {
                SetTransparent();
            }
            playerColliders += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerColliders -= 1;
            if (playerColliders == 0)
            {
                SetFullOpacity();
            }
            
        }
    }

    void SetFullOpacity()
    {
        sr.color = solidColor;
    }

    void SetTransparent()
    {
        sr.color = transparentColor;
    }
}
