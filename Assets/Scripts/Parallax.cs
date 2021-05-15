using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxAmount = 0.0f;

    float spriteLength = 35f;
    float posX;

    void Start()
    {
        posX = transform.position.x;
    }

    void Update()
    {
        float dist = transform.parent.position.x * parallaxAmount;
        float distToCam = transform.parent.position.x * (1 - parallaxAmount);
        transform.position = new Vector2(posX + dist, transform.position.y);

        if (distToCam > posX + spriteLength)
        {
            posX += spriteLength;
        }
        else if (distToCam < posX - spriteLength)
        {
            posX -= spriteLength;
        }
    }
}
