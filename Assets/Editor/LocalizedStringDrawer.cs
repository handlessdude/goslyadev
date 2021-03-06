﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(LocalizedString))]
public class LocalizedStringDrawer : PropertyDrawer
{
    bool dropdown;
    float height;
    public Dictionary<string, string> modifiedProp = new Dictionary<string, string>();
    bool isInit = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (dropdown)
        {
            return height + 47;
        }
        return 42;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        position.width -= 34;
        position.height = 18;

        Rect valueRect = new Rect(position);
        valueRect.x += 15;
        valueRect.width -= 15;

        Rect foldButtonRect = new Rect(position);
        foldButtonRect.width = 15;

        dropdown = EditorGUI.Foldout(foldButtonRect, dropdown, "");

        position.width += 34;

        SerializedProperty key = property.FindPropertyRelative("key");
        key.stringValue = EditorGUI.TextField(position, key.stringValue);

        if (!modifiedProp.ContainsKey(property.propertyPath))
        {
            modifiedProp[property.propertyPath] = default(string);
        }

        //Debug.Log(string.Join(" ", modifiedProp.Keys.Select(x => x.ToString())));

        if (modifiedProp[property.propertyPath] != default(string))
        {
            key.stringValue = modifiedProp[property.propertyPath];
            modifiedProp[property.propertyPath] = default(string);
        }

        
        //position.x += position.width + 2;
        position.y += 20;
        position.width = position.width/7*3;
        position.height = 17;


        if (GUI.Button(position, "Search"))
        {
            LocalizationEditorSearchWindow.Open(this, property.propertyPath);
        }

        position.x += position.width + 2;
        position.width = position.width/3*2;

        if (GUI.Button(position, "New"))
        {
            LocalizationEditorAddWindow.Open(key.stringValue);
        }

        position.x += position.width + 2;

        if (GUI.Button(position, "Edit"))
        {
            LocalizationEditorEditWindow.Open(key.stringValue);
        }

        if (dropdown)
        {
            var value = "\n" + CSVParser.GetTranslations(key.stringValue).Select(x => x.Key + ": " + x.Value + "\n\n").Aggregate((res, x) => res + x);
            GUIStyle style = GUI.skin.box;
            height = style.CalcHeight(new GUIContent(value), valueRect.width) + 40;

            valueRect.height = height;
            valueRect.y += 21;

            EditorGUI.LabelField(valueRect, value, EditorStyles.wordWrappedLabel);
        }

        EditorGUI.EndProperty();
    }
}
