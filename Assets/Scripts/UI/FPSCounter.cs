using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    public TextMeshProUGUI text;

    private void Start()
    {
        if (!text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        text.SetText(avgFrameRate.ToString());
    }
}
