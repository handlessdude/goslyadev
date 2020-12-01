using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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

        if (!optionsMenu.GetComponent<OptionsMenu>().mainMenu)
        {
            optionsMenu.GetComponent<OptionsMenu>().mainMenu = mainMenu;
            optionsMenu.GetComponent<OptionsMenu>().mainMenuScript = this;
        }
    }

    //MAIN MENU BUTTONS
    public void NewGame()
    {
        Debug.Log("STARTING NEW GAME");
        SceneManager.LoadScene(1);
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

    public void OnLangChange(int lang)
    {
        Localization.Language language = (Localization.Language)(lang+1);
        Localization.ChangeLanguage(language);
        //если кому-то хочется меня язык — он будет готов к тому, что сейчас будет что-то долгое
        foreach (Localizer t in FindObjectsOfType<Localizer>())
        {
            t.UpdateLocalization();
        }
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
