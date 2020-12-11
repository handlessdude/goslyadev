﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    static Dictionary<string, string> CurrentLangDict = null;
    static Language CurrentLang = Language.English;
    static bool Init = false;

    static void Initialize()
    {
        Init = true;
        CurrentLangDict = CSVParser.GetLocalizedStrings(GetLanguageCode(CurrentLang));
    }

    public enum Language
    {
        English = 1,
        Russian,
        Deutsch
    }

    public static string GetLanguageCode(Language lang)
    {
        switch(lang)
        {
            case Language.English:
                return "en";
            case Language.Russian:
                return "ru";
            case Language.Deutsch:
                return "de";
            default:
                return "en";
        }
    }

    public static int GetCurrentLanguage()
    {
        return (int)CurrentLang-1;
    }

    public static void ChangeLanguage(Language lang)
    {
        if (lang == CurrentLang)
        {
            return;
        }
        CurrentLang = lang;
        Initialize();
    }

    public static string GetLocalizedString(string key)
    {
        if (!Init)
        {
            Initialize();
        }

        string value;
        if (CurrentLangDict.TryGetValue(key, out value))
        {
            return value;
        }
        return key;
    }
}
