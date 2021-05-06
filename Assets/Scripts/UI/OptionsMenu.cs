using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    public MainMenu mainMenuScript;
    public AudioMixer audioMixer;
    public UnityEngine.UI.Slider SFXSlider;
    public UnityEngine.UI.Slider MusicSlider;

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

    public void OnEnable()
    {
        
    }

    //OPTIONS CONTROLS

    public void ChangeWindowed(int n)
    {
        mainMenuScript.ShowNotImplementedWarning();
        Debug.Log("HERE");
        /*if (n == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (n == 1)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }*/
    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SaveBindings() => SaveSystem.SaveKeys();
    public void ExitControls()
    {
        controlsMenu.SetActive(false);
        gameObject.SetActive(true);
        SaveSystem.SaveKeys();
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
