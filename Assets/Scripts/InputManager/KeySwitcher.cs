using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class KeySwitcher : MonoBehaviour
{
    public KeywordTextLocalizer localizer;
    private GameObject currentKey;
    private RectTransform rectTransform;
    private Image Image;
    public Sprite largeButton;
    Sprite smallButton;

    private static Dictionary<Keys, Tuple<KeyAction, string>> KeyActions = new Dictionary<Keys, Tuple<KeyAction, string>>()
    {
        [Keys.Left] = new Tuple<KeyAction, string>(KeyAction.MoveLeft, "MoveLeftKey"),
        [Keys.Right] = new Tuple<KeyAction, string>(KeyAction.MoveRight, "MoveRightKey"),
        [Keys.LookUp] = new Tuple<KeyAction, string>(KeyAction.LookUp, "LookUpKey"),
        [Keys.LookDown] = new Tuple<KeyAction, string>(KeyAction.LookDown, "LookDownKey"),
        [Keys.Action] = new Tuple<KeyAction, string>(KeyAction.Action, "ActionKey"),
        [Keys.Jump] = new Tuple<KeyAction, string>(KeyAction.Jump, "JumpKey"),
        [Keys.CameraScale] = new Tuple<KeyAction, string>(KeyAction.CameraScale, "CameraScaleKey"),
        [Keys.Magenta] = new Tuple<KeyAction, string>(KeyAction.WorldMagenta, "MagentaKey"),
        [Keys.Cyan] = new Tuple<KeyAction, string>(KeyAction.WorldCyan, "CyanKey"),
        [Keys.Green] = new Tuple<KeyAction, string>(KeyAction.WorldGreen, "GreenKey"),
        [Keys.Dash] = new Tuple<KeyAction, string>(KeyAction.Dash, "DashKey"),
        [Keys.Hit] = new Tuple<KeyAction, string>(KeyAction.Hit, "HitKey"),
        [Keys.Stomp] = new Tuple<KeyAction, string>(KeyAction.Stomp, "StompKey"),
    };

    /// Выпадающее меню в инспекторе
    public Keys Key;
    public enum Keys
    {
        Left,
        Right,
        LookUp,
        LookDown,
        Action,
        Jump,
        CameraScale,
        Magenta,
        Cyan,
        Green,
        Dash,
        Hit,
        Stomp,
    };
    
    //Кнопки, на которые нельзя биндить
    List<KeyCode> DeprecatedKeys = new List<KeyCode>() {
        KeyCode.Escape, 
        KeyCode.Return, 
        KeyCode.LeftCommand,
        KeyCode.None
    };


    List<KeyCode> LargeButtons = new List<KeyCode>()
    {
        KeyCode.Space,
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.Tab,
        KeyCode.CapsLock,
        KeyCode.RightAlt,
        KeyCode.LeftAlt,
        KeyCode.Backspace,
        KeyCode.Menu,
        KeyCode.Pause,
        KeyCode.ScrollLock,
        KeyCode.Home,
        KeyCode.Delete,
        KeyCode.PageDown,
        KeyCode.PageUp,
        KeyCode.SysReq,
        KeyCode.Insert,
        KeyCode.End,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.DownArrow,
        KeyCode.UpArrow,
        KeyCode.Mouse0,
        KeyCode.Mouse1,
    };

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        Image = gameObject.GetComponent<Image>();
        smallButton = Image.sprite;
        //Проверяет нужна ли большая подложка для кнопки
        if (LargeButtons.Contains(InputManager.bindings[KeyActions[Key].Item1].Item1))
            LargeButton();
    }

    public void OnGUI()
    {
        Event Event = Event.current;
        if (currentKey != null && GameplayState.IsOnClick)
        {
            if (Event.isKey)
            {
                if (!DeprecatedKeys.Contains(Event.keyCode))
                {
                    if (LargeButtons.Contains(Event.keyCode))
                        LargeButton();
                    else
                        SmallButton();
                    //Находит все кнопки, которые имею такой же бинд как и нажатая кнопка
                    foreach (var x in InputManager.bindings.Where(x => x.Value.Item1 == Event.keyCode && x.Key != KeyActions[Key].Item1).ToArray())
                    {
                        //Присваивает к ннопке бинд None
                        InputManager.bindings[x.Key] = new Tuple<KeyCode, KeyCode>(KeyCode.None, x.Value.Item2);
                        var NoneKey = KeyActions.Where(k => k.Value.Item1 == x.Key).First();
                        KeywordsReplacer.patterns[NoneKey.Value.Item2] = InputManager.GetPrimaryBinding(x.Key).ToString;
                        //Ищет объект(кнопку с биндом None), далее изменяет подложку и размеры и обновляет текст на кнопке. 
                        var NoneButton = GameObject.Find(NoneKey.Value.Item2);
                        NoneButton.transform.GetChild(0).GetComponent<KeywordTextLocalizer>().UpdateLocalization();
                        NoneButton.GetComponent<Image>().sprite = largeButton;
                        NoneButton.GetComponent<RectTransform>().sizeDelta = new Vector2(312f, 100f);
                    }
                    InputManager.bindings[KeyActions[Key].Item1] = new Tuple<KeyCode, KeyCode>(Event.keyCode, InputManager.bindings[KeyActions[Key].Item1].Item2);
                    KeywordsReplacer.patterns[KeyActions[Key].Item2] = InputManager.GetPrimaryBinding(KeyActions[Key].Item1).ToString;
                    localizer.UpdateLocalization();
                    Deselect();
                }
            }
        }
    }


    /// <summary>
    /// Делает подложку для кнопки маленького размера (одиночной клавишы)
    /// </summary>
    public void SmallButton()
    {
        rectTransform.sizeDelta = new Vector2(100f, 100f);
        Image.sprite = smallButton;
    }


    /// <summary>
    /// Делает подложку для большой клавишы (для клавишы пробел)
    /// </summary>
    public void LargeButton()
    {
        rectTransform.sizeDelta = new Vector2(350f, 100f);
        Image.sprite = largeButton;
    }


    /// <summary>
    /// Убирает выделение кнопки
    /// </summary>
    public void Deselect()
    {
        currentKey = null;
        GameplayState.IsOnClick = false;
        Image.color = new Color(1f, 1f, 1f);
    }
    public void ChangeKey(GameObject clicked)
    { 
        currentKey = clicked;
        Image.color = new Color(0.6f, 0.6f, 0.6f);
        GameplayState.IsOnClick = true;
    }

}
