using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class KeywordsReplacer
{
    static Dictionary<string, System.Func<string>> patterns =
        new Dictionary<string, System.Func<string>>()
        {
            ["MoveLeftKey"] = InputManager.GetPrimaryBinding(KeyAction.MoveLeft).ToString,
            ["MoveRightKey"] = InputManager.GetPrimaryBinding(KeyAction.MoveRight).ToString,
            ["LookUpKey"] = InputManager.GetPrimaryBinding(KeyAction.LookUp).ToString,
            ["LookDownKey"] = InputManager.GetPrimaryBinding(KeyAction.LookDown).ToString,
            ["ActionKey"] = InputManager.GetPrimaryBinding(KeyAction.Action).ToString
        };

    static char[] trimChars = new char[] { '<', '>'};

    public static string ReplaceKeywords(string str)
    {
        string pattern = @"<" + string.Join(@">|<", patterns.Keys) + @">";
        return Regex.Replace(str, pattern, x => patterns[x.Value.Trim(trimChars)]());
    }
}
