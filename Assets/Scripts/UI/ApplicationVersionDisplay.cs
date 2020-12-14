using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ApplicationVersionDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        if (!text)
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        text.text = Application.version;
    }
}
