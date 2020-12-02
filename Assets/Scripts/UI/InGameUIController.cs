using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject PauseVolume;


    void Start()
    {
        if (!pauseMenu)
        {
            pauseMenu = transform.Find("PauseMenu").gameObject;
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

    void Pause()
    {
        if (PauseVolume)
        {
            PauseVolume.SetActive(true);
        }
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0.0f;
        GameplayState.isPaused = true;
    }

    void ExitPause()
    {
        if (PauseVolume)
        {
            PauseVolume.SetActive(false);
        }
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        GameplayState.isPaused = false;
    }
}
