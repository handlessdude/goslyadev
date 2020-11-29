using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction
{
    MoveLeft,
    MoveRight,
    Jump,
}

public static class InputManager
{
    static Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>> bindings;

    static Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>> defaultMapping =
        new Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>>()
        {
            [KeyAction.MoveLeft] = new System.Tuple<KeyCode, KeyCode>(KeyCode.LeftArrow, KeyCode.A),
            [KeyAction.MoveRight] = new System.Tuple<KeyCode, KeyCode>(KeyCode.RightArrow, KeyCode.D),
            [KeyAction.Jump] = new System.Tuple<KeyCode, KeyCode>(KeyCode.UpArrow, KeyCode.Space)
        };

    static bool isInit = false;

    static void Init()
    {
        bindings = defaultMapping;
        isInit = true;
    }

    public static bool GetKey(KeyAction kA)
    {
        // низя так делать! но пока можно
        if (!isInit)
        {
            Init();
        }
        if (!bindings.ContainsKey(kA))
        {
            return false;
        }
        return Input.GetKey(bindings[kA].Item1) || Input.GetKey(bindings[kA].Item2);
    }

    public static bool GetKeyDown(KeyAction kA)
    {
        if (!isInit)
        {
            Init();
        }
        if (!bindings.ContainsKey(kA))
        {
            return false;
        }
        return Input.GetKeyDown(bindings[kA].Item1) || Input.GetKeyDown(bindings[kA].Item2);
    }
}
