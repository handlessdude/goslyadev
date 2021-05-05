using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class KeywordsReplacer
{
    private static Dictionary<string, string> Replace =
        new Dictionary<string, string>()
        {
            ["Alpha1"] = "1",
            ["Alpha2"] = "2",
            ["Alpha3"] = "3",
            ["Alpha4"] = "4",
            ["Alpha5"] = "5",
            ["Alpha6"] = "6",
            ["Alpha7"] = "7",
            ["Alpha8"] = "8",
            ["Alpha9"] = "9",
            ["Alpha0"] = "0",
            ["LeftShift"] = "LShift",
            ["RightShift"] = "RShift",
            ["LeftControl"] = "LCtrl",
            ["RightControl"] = "RCtrl",
            ["LeftAlt"] = "LAlt",
            ["RightAlt"] = "RAlt",
            ["LeftBracket"] = "[",
            ["RightBracket"] = "]",
            ["Semicolon"] = ";",
            ["Quote"] = "'",
            ["Comma"] = ",",
            ["Period"] = ".",
            ["Slash"] = "/",
            ["Backslash"] = @"\",
            ["Minus"] = "-",
            ["Equals"] = "=",
            ["BackQuote"] = "`",
            ["PageDown"] = "PgDown",
            ["SysReq"] = "PrtSc",
            ["Mouse0"] = "LClick",
            ["Mouse1"] = "RClick",
        };

    public static Dictionary<string, System.Func<string>> patterns =
        new Dictionary<string, System.Func<string>>()
        {
            ["MoveLeftKey"] = InputManager.GetPrimaryBinding(KeyAction.MoveLeft).ToString,
            ["MoveRightKey"] = InputManager.GetPrimaryBinding(KeyAction.MoveRight).ToString,
            ["LookUpKey"] = InputManager.GetPrimaryBinding(KeyAction.LookUp).ToString,
            ["LookDownKey"] = InputManager.GetPrimaryBinding(KeyAction.LookDown).ToString,
            ["ActionKey"] = InputManager.GetPrimaryBinding(KeyAction.Action).ToString,
            ["JumpKey"] = InputManager.GetPrimaryBinding(KeyAction.Jump).ToString,
            ["CameraScaleKey"] = InputManager.GetPrimaryBinding(KeyAction.CameraScale).ToString,
            ["MagentaKey"] = InputManager.GetPrimaryBinding(KeyAction.WorldMagenta).ToString,
            ["CyanKey"] = InputManager.GetPrimaryBinding(KeyAction.WorldCyan).ToString,
            ["GreenKey"] = InputManager.GetPrimaryBinding(KeyAction.WorldGreen).ToString,
            ["DashKey"] = InputManager.GetPrimaryBinding(KeyAction.Dash).ToString,
            ["HitKey"] = InputManager.GetPrimaryBinding(KeyAction.Hit).ToString,
            ["StompKey"] = InputManager.GetPrimaryBinding(KeyAction.Stomp).ToString,
        };

    static char[] trimChars = new char[] { '<', '>'};

    public static string ReplaceKeywords(string str)
    {
        string pattern = @"<" + string.Join(@">|<", patterns.Keys) + @">";
        var s = Regex.Replace(str, pattern, x => patterns[x.Value.Trim(trimChars)]());
        return Replace.ContainsKey(s) ? Replace[s] : s;
    }
}
