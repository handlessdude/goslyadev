using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject PauseVolume;
    public GameObject LightBuble;
    public GameObject FPSCounter;
    public GameObject UIFX;
    public GameObject Inventory;
    public RectTransform HPBarContentMask;

    public bool showHPBar;

    private float hpMaskMaxWidth;
    void Start()
    {
        if (!pauseMenu)
        {
            pauseMenu = transform.Find("PauseMenu").gameObject;
        }

        if (showHPBar)
        {
            if (!HPBarContentMask)
            {
                HPBarContentMask = transform.Find("HPBar").Find("Mask").GetComponent<RectTransform>();
            }
            hpMaskMaxWidth = HPBarContentMask.rect.width;
        }
        
    }

    void Update()
    {
        if (InputManager.GetKeyDown(KeyAction.Pause))
        {
            if (GameplayState.isPaused)
            {
                ExitPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (PauseVolume)
        {
            PauseVolume.SetActive(true);
        }
        if (LightBuble)
        {
            LightBuble.SetActive(false); // Отключает лампочку при открытии меню
            UIFX.SetActive(false); //Отключает свет лампочки при открытии меню
        }
        if (Inventory)
            Inventory.SetActive(false);
        FPSCounter.SetActive(false);  // Отключает счетчик FPS при открытии меню
        pauseMenu.SetActive(true);
        foreach (Transform child in transform.GetChild(2))
        {
            if (child.name == "MainMenu")
                child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
        }
        Cursor.visible = true;
        Time.timeScale = 0.0f;
        GameplayState.isPaused = true;
    }

    public void ExitPause()
    {
        if (PauseVolume)
        {
            PauseVolume.SetActive(false);
        }
        if (LightBuble)
        {
            UIFX.SetActive(true); //Включает свет лампочки при закрытии меню
            LightBuble.SetActive(true); // Включает лампочку при закрытии меню
        }
        if (Inventory)  
            Inventory.SetActive(true);
        FPSCounter.SetActive(true); // Включает счетчик FPS при закрытии меню
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        GameplayState.isPaused = false;
    }

    public void UpdateHP(int hp, int maxhp)
    {
        if (showHPBar)
        {
            float fill_ratio = (float)maxhp / hp;
            float width = hpMaskMaxWidth / fill_ratio;
            HPBarContentMask.sizeDelta = new Vector2(width, HPBarContentMask.rect.height);
        }
        
    }
}
