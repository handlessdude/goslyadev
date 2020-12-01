using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO?: вынести опции в отедльный скрипт, общий для главного меню и этого меню
public class PauseMenu : MainMenu
{

    //PAUSE MENU BUTTONS
    public void SaveGame()
    {
        Debug.Log("SAVING THE GAME");
        ShowNotImplementedWarning();
    }

    public void MainMenu()
    {
        Debug.Log("OPENING MAIN MENU");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
