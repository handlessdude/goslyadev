using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LocalizationEditorAddWindow : EditorWindow
{
    public string key;
    public static void Open(string key)
    {
        LocalizationEditorAddWindow window = new LocalizationEditorAddWindow();
        window.titleContent = new GUIContent("ADD | Localization Editor");
        window.ShowUtility();
        window.key = key;
        window.translations = CSVParser.Languages.ToDictionary(x => x.Key, x => "");
    }

    public Dictionary<string, string> translations;

    public void OnGUI()
    {
        key = EditorGUILayout.TextField("Key: ", key);
        

        EditorStyles.textArea.wordWrap = true;

        foreach (string k in translations.Keys.ToArray())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(k + ": ", GUILayout.MaxWidth(50));

            translations[k] = EditorGUILayout.TextArea(translations[k], EditorStyles.textArea, GUILayout.Height(100), GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
        }
        
        if (GUILayout.Button("Add"))
        {
            string[] keys = CSVParser.GetKeys();
            if (keys.Contains(key))
            {
                if (EditorUtility.DisplayDialog("ERROR", "Key already exists! Use \"Edit\" instead!", "OK"))
                {
                    
                }
            }
            else
            {
                CSVParser.Add(key, translations);
            }
        }

        minSize = new Vector2(460, 360);
        maxSize = minSize;
    }
}

public class LocalizationEditorEditWindow : EditorWindow
{
    public string key;
    public static void Open(string key)
    {
        LocalizationEditorEditWindow window = new LocalizationEditorEditWindow();
        window.titleContent = new GUIContent("EDIT | Localization Editor");
        window.ShowUtility();
        window.key = key;
        window.translations = CSVParser.GetTranslations(key);
    }

    public Dictionary<string, string> translations;

    public void OnGUI()
    {
        key = EditorGUILayout.TextField("Key: ", key);

        EditorStyles.textArea.wordWrap = true;

        foreach (string k in translations.Keys.ToArray())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(k + ": ", GUILayout.MaxWidth(50));

            translations[k] = EditorGUILayout.TextArea(translations[k], EditorStyles.textArea, GUILayout.Height(100), GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Edit"))
        {
            string[] keys = CSVParser.GetKeys();
            if (keys.Contains(key))
            {
                CSVParser.Add(key, translations);
            }
            else
            {
                if (EditorUtility.DisplayDialog("ERROR", "Key doesn't exist! Use \"Add\" instead!", "OK"))
                {

                }
            }
        }

        minSize = new Vector2(460, 360);
        maxSize = minSize;
    }
}

public class LocalizationEditorSearchWindow : EditorWindow
{
    public static void Open(LocalizedStringDrawer ls)
    {
        LocalizationEditorSearchWindow window = new LocalizationEditorSearchWindow();
        window.titleContent = new GUIContent("Localization Search");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);
        window.ShowAsDropDown(r, new Vector2(500, 300));
        window.stringDrawer = ls;
        window.GetSearchResults();
    }

    public string value;
    public Vector2 scroll;
    public Dictionary<string, string> dictionary;
    public LocalizedStringDrawer stringDrawer;

    private void OnEnable()
    {
        dictionary = CSVParser.GetLocalizedStrings("en");
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Search :", EditorStyles.boldLabel);
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();

        GetSearchResults();
    }

    private void GetSearchResults()
    {
        if (value == null)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (KeyValuePair<string, string> kv in dictionary)
        {
            if (kv.Key.ToLower().Contains(value.ToLower()) || kv.Value.ToLower().Contains(value.ToLower()))
            {
                EditorGUILayout.BeginHorizontal("box");

                if (GUILayout.Button("✓", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
                {
                    stringDrawer.modifiedProp = kv.Key;
                    this.Close();
                }

                if (GUILayout.Button("x", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
                {
                    if (EditorUtility.DisplayDialog("Remove " + kv.Key + "?", "THINK ABOUT IT", "REMOVE"))
                    {
                        CSVParser.Remove(kv.Key);
                        AssetDatabase.Refresh();
                    }
                }

                EditorGUILayout.TextField(kv.Key);
                EditorGUILayout.LabelField(kv.Value);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}

