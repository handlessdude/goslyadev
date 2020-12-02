using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlack : MonoBehaviour
{
    UnityEngine.UI.Image image;

    float fadeStep = 0.10f;
    float fadeTimeOnOneStep = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        if (!image)
        {
            image = transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        }
        image.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeTransition()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTransitionAnimation());
    }

    public void FadeTransition(out float halfTime)
    {
        halfTime = 1 / fadeStep * fadeTimeOnOneStep;
        FadeTransition();
    }

    IEnumerator FadeTransitionAnimation()
    {
        while (image.color.a < 1.0f)
        {
            image.color = new Color(0, 0, 0, image.color.a + fadeStep);
            yield return new WaitForSeconds(fadeTimeOnOneStep);
        }

        while (image.color.a > 0f)
        {
            image.color = new Color(0, 0, 0, image.color.a - fadeStep);
            yield return new WaitForSeconds(fadeTimeOnOneStep);
        }
    }
}
