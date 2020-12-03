using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV : MonoBehaviour
{
    [SerializeField]
    public Sprite spriteOff;
    [SerializeField]
    public Sprite spriteOn;

    public GameObject light;
    public SpriteRenderer spriteRenderer;

    int playerColliders;

    // Start is called before the first frame update
    void Start()
    {
        if (!light)
        {
            light = transform.Find("Light").gameObject;
        }

        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }    
        light.SetActive(false);
        spriteRenderer.sprite = spriteOff;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerColliders += 1;
            spriteRenderer.sprite = spriteOn;
            light.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerColliders -= 1;
            if (playerColliders < 1)
            {
                spriteRenderer.sprite = spriteOff;
                light.SetActive(false);
            }
        }
    }
}
