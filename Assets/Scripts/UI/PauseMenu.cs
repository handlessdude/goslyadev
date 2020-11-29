using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO?: вынести опции в отедльный скрипт, общий для главного меню и этого меню
public class PauseMenu : MonoBehaviour
{
    public GameObject notImplementedWarning;
    public GameObject mainMenu;
    public GameObject optionsMenu;

    private void Start()
    {
        if (!notImplementedWarning)
        {
            notImplementedWarning = transform.Find("NotImplemented").gameObject;
        }

        if (!mainMenu)
        {
            mainMenu = transform.Find("MainMenu").gameObject;
        }

        if (!optionsMenu)
        {
            optionsMenu = gameObject.FindObject("Settings").gameObject;
        }
    }

    //PAUSE MENU BUTTONS
    public void SaveGame()
    {
        Debug.Log("SAVING THE GAME");
        ShowNotImplementedWarning();
    }

    public void LoadGame()
    {
        Debug.Log("OPENING LOAD GAME MENU");
        ShowNotImplementedWarning();
    }

    public void OpenOptions()
    {
        Debug.Log("OPENING OPTIONS");
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("EXITING GAME");
        Application.Quit();
    }

    public void MainMenu()
    {
        Debug.Log("OPENING MAIN MENU");
        SceneManager.LoadScene(0);
    }



    //OPTIONS CONTROLS

    public void ChangeWindowed(int n)
    {
        if (n == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (n == 1)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }

    public void Controls()
    {
        Debug.Log("OPENING CONTROLS");
        ShowNotImplementedWarning();
    }


    public void ExitOptions()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowNotImplementedWarning()
    {
        notImplementedWarning.SetActive(true);
        CancelInvoke("HideNotImplementedWarning");
        Invoke("HideNotImplementedWarning", 1.5f);
    }

    void HideNotImplementedWarning()
    {
        notImplementedWarning.SetActive(false);
    }
}
