using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLocalizer : Localizer
{

    public TextMeshProUGUI textMeshProText;
    public string text = "UNLOCALIZED";
    void Start()
    {
        if (!textMeshProText)
        {
            textMeshProText = GetComponent<TextMeshProUGUI>();
        }
        UpdateLocalization();
    }

    //[!] вообще, это костыль. В редакторе Start() вызывается и при включении объекта, но в билде — нет!
    //TODO: разобраться
    private void OnEnable()
    {
        if (!textMeshProText)
        {
            textMeshProText = GetComponent<TextMeshProUGUI>();
        }
        UpdateLocalization();
    }

    public override void UpdateLocalization()
    {
        textMeshProText.SetText(Localization.GetLocalizedString(text));
    }
}
