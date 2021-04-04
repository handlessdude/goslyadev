using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MainMenu
{
    public InGameUIController Controller;
  
    //PAUSE MENU BUTTONS
    public void MainMenu()
    {
        Debug.Log("OPENING MAIN MENU");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        Controller.ExitPause();
    }
}
