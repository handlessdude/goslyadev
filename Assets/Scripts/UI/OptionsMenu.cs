using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    public GameObject warningMenu;
    public MainMenu mainMenuScript;
    public AudioMixer audioMixer;
    public UnityEngine.UI.Slider SFXSlider;
    public UnityEngine.UI.Slider MusicSlider;

    //строка - "16:9" например
    Dictionary<string, List<(int, int)>> standard_resolutions = new Dictionary<string, List<(int, int)>>()
    {
        { "4:3", new List<(int, int)> {(640, 480), (800, 600), (1024, 768), (1280, 960), (1600,1200), (1920, 1440)} },
        { "16:10", new List<(int, int)> {(1280, 800), (1440, 900), (1680, 1050), (1920, 1200), (2560, 1600)} },
        { "16:9", new List<(int, int)> {(1024, 576), (1280, 720), (1366, 768), (1600, 900), (1920, 1080), (2560, 1440), (3840, 2160) } },
        { "21:9", new List<(int, int)> {(2560, 1080), (3440, 1440)} }
    };

    void Start()
    {
        if (!SFXSlider)
        {
            SFXSlider = transform.Find("SFXVolume").GetComponent<UnityEngine.UI.Slider>();
        }
        if (!MusicSlider)
        {
            MusicSlider = transform.Find("MusicVolume").GetComponent<UnityEngine.UI.Slider>();
        }

        if (audioMixer)
        {
            float val;
            audioMixer.GetFloat("SFXVolume", out val);
            SFXSlider.value = Mathf.Pow(10, val/20);
            audioMixer.GetFloat("MusicVolume", out val);
            MusicSlider.value = Mathf.Pow(10, val/20);
        }
    }

    //OPTIONS CONTROLS


    public void ChangeWindowed(int n)
    {
        Debug.Log("HERE");
        //if (n == 0)
        //{
        //    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        //}
        //else if (n == 1)
        //{
        //    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        //}
        //else if (n == 2)
        //{
        //    Screen.fullScreenMode = FullScreenMode.Windowed;
        //}
    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SaveBindings()
    {
        SaveSystem.SaveKeys();
        foreach (var x in GameObject.FindGameObjectsWithTag("Hint"))
            x.GetComponentInChildren<KeywordTextLocalizer>().UpdateLocalization();
        GameplayState.isBindingsSaved = true;
    }

    public void Yes()
    {
        SaveBindings();
        warningMenu.SetActive(false);
        gameObject.SetActive(true);
    }

    public void No()
    {
        SaveSystem.LoadKeys();
        warningMenu.SetActive(false);
        gameObject.SetActive(true);
    }
        
    public void ExitControls()
    {
        controlsMenu.SetActive(false);
        if (GameplayState.isBindingsSaved)
            gameObject.SetActive(true);
        else
            warningMenu.SetActive(true);
    }

    public void ChangeSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        Debug.Log(Mathf.Log10(value) * 20);
        Debug.Log(value);
    }

    public void ChangeMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }


    public void ExitOptions()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
