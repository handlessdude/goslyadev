using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMoverWhilePressed : MonoBehaviour
{
    public float MovementAmount = 2.0f;
    RectTransform rt;
    private void Start()
    {
        if (!rt)
        {
            rt = transform.Find("Text").GetComponent<RectTransform>();
        }
    }
    public void MoveTextUp()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + MovementAmount);
    }

    public void MoveTextDown()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - MovementAmount);
    }
}
