using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVParser
{
    public static string localizationFilename = "localization";
    static readonly char lineSeparator = '\n';
    static readonly char surround = '\"';
    static readonly string[] fieldSeparator = { "\",\""};

    static TextAsset CSVfile;

    static void LoadCSV()
    {
        CSVfile = Resources.Load<TextAsset>(localizationFilename);
    }

    /// <summary>
    /// Ищет переведённые слова одного языка (язык задаётся через строку-сокращение, например, "en")
    /// </summary>
    public static Dictionary<string, string> GetLocalizedStrings(string language)
    {
        //если ни разу не запускали (~~гуся) методы этого класса
        if (!CSVfile)
        {
            LoadCSV();
        }
        string[] lines = CSVfile.text.Split(lineSeparator);
        string[] lang_keys = lines[0].Split(fieldSeparator, System.StringSplitOptions.None);
        lang_keys[0] = lang_keys[0].Trim(surround);
        lang_keys[lang_keys.Length-1] = lang_keys[lang_keys.Length - 1].Trim(surround);

        //ищем, какой по порядку в CSV-файле наш язык
        int searched_key_number = -1;
        for (int i = 0; i < lang_keys.Length; i++)
        {
            if (lang_keys[i] == language)
            {
                searched_key_number = i;
                break;
            }
        }

        if (searched_key_number == -1)
        {
            throw new System.Exception("Введённое сокращение для языка неверно! (" + language + "). Используйте en, ru или подобные");
        }

        //забиваем на другие языки, читаем только ключ и нужный язык
        Dictionary<string, string> result = new Dictionary<string, string>();

        string key_pattern = surround + @".*?" + surround;
        //читаемые регексы это удел слабых
        //TODO: в один enumerable перед Concat всё запихнуть было бы лучше
        string value_pattern = "(?<=(" + surround +  string.Concat(Enumerable.Repeat(@".*?" + fieldSeparator[0], searched_key_number)) + ")).*?" + surround;
        for (int i = 0; i < searched_key_number; i++)
        {

        }
        //это конечно вполне эффективно, но можно сделать ещё эффективнее
        //TODO: понять как и надо ли оно.
        for (int i = 1; i < lines.Length; i++)
        {
            string key = Regex.Match(lines[i], key_pattern).Value.Trim(surround);
            string value = Regex.Match(lines[i], value_pattern).Value.Trim(surround);

            if (!result.ContainsKey(key))
            {
                result[key] = value;
            }

        }
        return result;
    }
}

