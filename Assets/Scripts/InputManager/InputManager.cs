using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction
{
    MoveLeft,
    MoveRight,
    Jump,
    Pause,
    LookUp,
    LookDown,
    WorldCyan,
    WorldMagenta,
    WorldGreen,
    CameraScale
}

public static class InputManager
{
    static Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>> bindings;

    static Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>> defaultMapping =
        new Dictionary<KeyAction, System.Tuple<KeyCode, KeyCode>>()
        {
            [KeyAction.MoveLeft] = new System.Tuple<KeyCode, KeyCode>(KeyCode.LeftArrow, KeyCode.A),
            [KeyAction.MoveRight] = new System.Tuple<KeyCode, KeyCode>(KeyCode.RightArrow, KeyCode.D),
            [KeyAction.Jump] = new System.Tuple<KeyCode, KeyCode>(KeyCode.None, KeyCode.Space),
            [KeyAction.Pause] = new System.Tuple<KeyCode, KeyCode>(KeyCode.P, KeyCode.Escape),
            [KeyAction.LookDown] = new System.Tuple<KeyCode, KeyCode>(KeyCode.S, KeyCode.DownArrow),
            [KeyAction.LookUp] = new System.Tuple<KeyCode, KeyCode>(KeyCode.W, KeyCode.UpArrow),
            [KeyAction.WorldCyan] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Alpha1, KeyCode.C),
            [KeyAction.WorldMagenta] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Alpha2, KeyCode.F),
            [KeyAction.WorldGreen] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Alpha3, KeyCode.G),
            [KeyAction.CameraScale] = new System.Tuple<KeyCode, KeyCode>(KeyCode.V, KeyCode.None)
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

    public static bool GetKeyUp(KeyAction kA)
    {
        if (!isInit)
        {
            Init();
        }
        if (!bindings.ContainsKey(kA))
        {
            return false;
        }
        return Input.GetKeyUp(bindings[kA].Item1) || Input.GetKeyUp(bindings[kA].Item2);
    }
}
