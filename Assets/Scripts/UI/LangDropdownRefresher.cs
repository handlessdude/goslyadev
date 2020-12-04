using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//TODO: интегрировать этот скрипт в скрипт главного меню
public class LangDropdownRefresher : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    void Start()
    {
        if (!dropdown)
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        dropdown.value = Localization.GetCurrentLanguage();
    }
}
