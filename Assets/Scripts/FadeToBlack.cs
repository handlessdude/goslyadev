using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlack : MonoBehaviour
{
    UnityEngine.UI.Image image;

    float fadeStep = 0.10f;
    float fadeTimeOnOneStep = 0.05f;
    public float blackScreenDuration = 0.2f;

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
        halfTime = 1 / fadeStep * fadeTimeOnOneStep + blackScreenDuration/2;
        FadeTransition();
    }

    IEnumerator FadeTransitionAnimation()
    {
        while (image.color.a < 1.0f)
        {
            image.color = new Color(0, 0, 0, image.color.a + fadeStep);
            yield return new WaitForSeconds(fadeTimeOnOneStep);
        }

        yield return new WaitForSeconds(0.2f);

        while (image.color.a > 0f)
        {
            image.color = new Color(0, 0, 0, image.color.a - fadeStep);
            yield return new WaitForSeconds(fadeTimeOnOneStep);
        }
    }
}
