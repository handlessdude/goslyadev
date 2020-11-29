using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public GameObject pauseMenu;

    bool isPaused = false;

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
            if (isPaused)
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
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    void ExitPause()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        isPaused = false;
    }
}
