using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropDownLocalizer : Localizer
{
    public TMP_Dropdown dropdown;
    public List<string> text;
    void Start()
    {
        if (!dropdown)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }
        UpdateLocalization();
    }

    //[!] вообще, это костыль. В редакторе Start() вызывается и при включении объекта, но в билде — нет!
    //TODO: разобраться
    private void OnEnable()
    {
        if (!dropdown)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }
        UpdateLocalization();
    }

    public override void UpdateLocalization()
    {
        //TODO: выглядит плохо
        int ctr = dropdown.options.Count;
        dropdown.options.Clear();
        for (int i = 0; i < ctr; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(Localization.GetLocalizedString(text[i])));
        }
    }
}
