using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    SpriteRenderer sr;
    BoxCollider2D rigidCollider;
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
        if (!rigidCollider)
        {
            rigidCollider = GetComponents<BoxCollider2D>().First(x => !x.isTrigger);
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
        if (!rigidCollider)
        {
            rigidCollider = GetComponents<BoxCollider2D>().First(x => !x.isTrigger);
        }

        StartCoroutine(PlayerGlitchFixer());
        SetFullOpacity();
    }

    //решает проблему с проталкиванием игрока в текстуры, когда платформа спавнится в нём
    IEnumerator PlayerGlitchFixer()
    {
        rigidCollider.enabled = false;
        yield return new WaitForFixedUpdate();
        if (playerColliders < 1)
        {
            rigidCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (playerColliders == 0)
            {
                SetTransparent();
                rigidCollider.enabled = false;
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
                rigidCollider.enabled = true;
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
