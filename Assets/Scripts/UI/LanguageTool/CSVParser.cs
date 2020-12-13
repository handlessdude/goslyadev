using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

public static class CSVParser
{
    public static string localizationFilename = "localization";
    static readonly char lineSeparator = '\n';
    static readonly char surround = '\"';
    static readonly string[] fieldSeparator = { "\",\""};
    public const int NUMBER_OF_LANGUAGES = 3;

    static TextAsset CSVfile;
    //индексация языков с единицы, т.к. нулевой в файле это key
    static Dictionary<string, int> languages;

    static void LoadCSV()
    {
        CSVfile = Resources.Load<TextAsset>(localizationFilename);

        if (!CSVfile)
        {
            Debug.LogError("LOCALIZATION FILE IS MISSING!");
        }

        //приписываем каждому строковому идентификатору языка его порядковый номер в файле

        int firstLineEndIndex = CSVfile.text.IndexOfAny(new char[] { '\r', '\n' });
        string tableHeader = CSVfile.text.Substring(0, firstLineEndIndex);
        string[] lang_keys = tableHeader.Split(fieldSeparator, System.StringSplitOptions.None);
        lang_keys[0] = lang_keys[0].Trim(surround);
        lang_keys[lang_keys.Length - 1] = lang_keys[lang_keys.Length - 1].Trim(surround);
        languages = new Dictionary<string, int>(NUMBER_OF_LANGUAGES);

        for (int i = 1; i < lang_keys.Length; i++)
        {
            languages.Add(lang_keys[i], i);
        }
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

        //ищем, какой по порядку в CSV-файле наш язык
        int searched_key_number = -1;

        if (!languages.TryGetValue(language, out searched_key_number))
        {
            throw new System.Exception("Введённое сокращение для языка неверно! (" + language + "). Используйте en, ru или подобные");
        }

        //забиваем на другие языки, читаем только ключ и нужный язык
        Dictionary<string, string> result = new Dictionary<string, string>();

        string key_pattern = surround + @".*?" + surround;
        //читаемые регексы это удел слабых
        //TODO: в один enumerable перед Concat всё запихнуть было бы лучше
        string value_pattern = "(?<=(" + surround +  string.Concat(Enumerable.Repeat(@".*?" + fieldSeparator[0], searched_key_number)) + ")).*?" + surround;

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


    //Операции ниже не должны работать в билде (а зачем вам менять файл локализации прямо в игре?),
    //поэтому эффективность не важна
#if UNITY_EDITOR
    public static void Add(string key, Dictionary<string, string> translations)
    {
        if (!CSVfile)
        {
            LoadCSV();
        }

        if (translations.Count != NUMBER_OF_LANGUAGES)
        {
            Debug.LogError("NOT ENOUGH LANGUAGE TRANSLATIONS PASSED IN 'ADD' FUNC!");
        }

        string[] keys = GetKeys();
        if (keys.Contains(key))
        {
            using (StreamWriter sw = new StreamWriter("Assets/Resources/" + localizationFilename + ".csv", false))
            {
                string result = JoinTranslations(key, translations);
                string[] lines = CSVfile.text.Split(lineSeparator);
                sw.Write(lines[0]);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("\"" + key + "\""))
                    {
                        sw.Write(lineSeparator + result);
                    }
                    else
                    {
                        sw.Write(lineSeparator + lines[i]);
                    }
                }
            }
        }
        else
        {
            string result = JoinTranslations(key, translations);
            File.AppendAllText("Assets/Resources/" + localizationFilename + ".csv", lineSeparator + result);
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    

    /*public static void Add(string key, string language, string value)
    {
        if (!CSVfile)
        {
            LoadCSV();
        }

        string[] keys = GetKeys();
        if (keys.Contains(key))
        {
            Dictionary<string, string> translations = GetTranslations(key);
            translations[language] = value;
            string result = JoinTranslations(key, translations);
            string[] lines = CSVfile.text.Split(lineSeparator);
            //File.WriteAllText("Assets/Resources/" + localizationFilename + ".csv", string.Empty);
            using (StreamWriter sw = new StreamWriter("Assets/Resources/" + localizationFilename + ".csv", false))
            {
                sw.Write(lines[0]);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("\"" + key + "\""))
                    {
                        sw.Write(lineSeparator + result);
                    }
                    else
                    {
                        sw.Write(lineSeparator + lines[i]);
                    }
                }
            }
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();
            translations.Add(language, value);
            foreach (string k in languages.Keys)
            {
                if (k != language)
                {
                    translations.Add(k, string.Empty);
                }
            }

            string result = JoinTranslations(key, translations);
            Add(key, translations);
        }
    }*/

    public static Dictionary<string, int> Languages
    {
        get 
        { 
            if (!CSVfile)
            {
                LoadCSV();
            }

            return languages;
        }
    }
    

    public static void Remove(string key)
    {
        if (!CSVfile)
        {
            LoadCSV();
        }

        string[] lines = CSVfile.text.Split(lineSeparator);

        using (StreamWriter sw = new StreamWriter("Assets/Resources/" + localizationFilename + ".csv", false))
        {
            sw.Write(lines[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("\"" + key + "\""))
                { }
                else
                {
                    sw.Write(lineSeparator + lines[i]);
                }
            }
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    static string JoinTranslations(string key, Dictionary<string, string> translations, string surround = "\"", string separator = "\",\"")
    {
        if (!CSVfile)
        {
            LoadCSV();
        }

        string result = surround + key + separator;

        IEnumerable<string> translationsInOrder =
            translations.Select(x => new KeyValuePair<int, string>(languages[x.Key], x.Value)).OrderBy(x => x.Key).Select(x => x.Value);

        result += string.Join(separator, translationsInOrder) + surround;

        return result;
    }

    public static Dictionary<string, string> GetTranslations(string key)
    {
        if (!CSVfile)
        {
            LoadCSV();
        }

        Dictionary<string, string> result = new Dictionary<string, string>(NUMBER_OF_LANGUAGES);
        string str = CSVfile.text.Split(lineSeparator).FirstOrDefault(x => x.StartsWith("\"" + key + "\""));
        if (str == default(string))
        {
            return languages.ToDictionary(x => x.Key, x => default(string));
        }

        string[] translations = str.Split(fieldSeparator, System.StringSplitOptions.None);
        translations[0] = translations[0].Trim(surround);
        translations[translations.Length - 1] = translations[translations.Length - 1].Trim(surround);
        for (int i = 1; i < translations.Length; i++)
        {
            result.Add(languages.FirstOrDefault(x => x.Value == i).Key, translations[i]);
        }

        return result;
    }

    public static string[] GetKeys()
    {
        if (!CSVfile)
        {
            LoadCSV();
        }
        return CSVfile.text.Split(lineSeparator).Select
            (x => x.Split(fieldSeparator, System.StringSplitOptions.None)[0].Trim(surround)).Where(x => x != "key").ToArray();
    }

#endif
}

