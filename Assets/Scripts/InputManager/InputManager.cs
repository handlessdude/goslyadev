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
    CameraScale,
    Action,
    Confirm,
    // Сделал для проверки
    Level1,
    Level2,
    Load,
    Save,
    //Test
    CombatAbility1,
    CombatAbility2,
    Stomp
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
            [KeyAction.CameraScale] = new System.Tuple<KeyCode, KeyCode>(KeyCode.V, KeyCode.None),
            [KeyAction.Action] = new System.Tuple<KeyCode, KeyCode>(KeyCode.E, KeyCode.None),
            [KeyAction.Confirm] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Return, KeyCode.E),
            [KeyAction.Level1] = new System.Tuple<KeyCode, KeyCode>(KeyCode.F1, KeyCode.None),
            [KeyAction.Level2] = new System.Tuple<KeyCode, KeyCode>(KeyCode.F2, KeyCode.None),
            [KeyAction.Load] = new System.Tuple<KeyCode, KeyCode>(KeyCode.F4, KeyCode.Mouse3),
            [KeyAction.Save] = new System.Tuple<KeyCode, KeyCode>(KeyCode.F5, KeyCode.Mouse4),
            [KeyAction.CombatAbility1] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Mouse0, KeyCode.None),
            [KeyAction.CombatAbility2] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Mouse1, KeyCode.None),
            [KeyAction.Stomp] = new System.Tuple<KeyCode, KeyCode>(KeyCode.Mouse1, KeyCode.None),
            
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

    public static KeyCode GetPrimaryBinding(KeyAction kA)
    {
        if (!isInit)
        {
            Init();
        }
        if (!bindings.ContainsKey(kA))
        {
            return KeyCode.None;
        }
        else
        {
            return bindings[kA].Item1;
        }
    }

    public static KeyCode GetSecondaryBinding(KeyAction kA)
    {
        if (!isInit)
        {
            Init();
        }
        if (!bindings.ContainsKey(kA))
        {
            return KeyCode.None;
        }
        else
        {
            return bindings[kA].Item2;
        }
    }
}
