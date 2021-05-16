using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    public GameObject warningMenu;
    public MainMenu mainMenuScript;
    public AudioMixer audioMixer;
    public UnityEngine.UI.Slider SFXSlider;
    public UnityEngine.UI.Slider MusicSlider;
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenDropdown;

    string currentAspectRatio;

    //строка - "16:9" например
    Dictionary<string, List<(int, int)>> standard_resolutions = new Dictionary<string, List<(int, int)>>()
    {
        { "16:9", new List<(int, int)> {(1024, 576), (1280, 720), (1366, 768), (1600, 900), (1920, 1080), (2560, 1440), (3840, 2160) } },
        { "4:3", new List<(int, int)> {(640, 480), (800, 600), (1024, 768), (1280, 960), (1600,1200), (1920, 1440)} },
        { "5:4", new List<(int, int)> {(960, 768), (1280, 1024), (1350, 1080), (1920, 1536)} },
        { "16:10", new List<(int, int)> {(1280, 800), (1440, 900), (1680, 1050), (1920, 1200), (2560, 1600)} },
        { "21:9", new List<(int, int)> {(2560, 1080), (3440, 1440)} }
    };

    Dictionary<string, int> dropdownKeys = new Dictionary<string, int>()
    { { "Auto", 0}, { "16:9", 1}, { "4:3", 2}, { "5:4", 3}, { "16:10", 4}, { "21:9", 5}};


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

    private void OnEnable()
    {
        //инициализуется здесь, потому что Awake->OnEnable->Start
        if (!aspectRatioDropdown)
        {
            var t = transform.Find("AspectRatio");
            if (t)
            {
                aspectRatioDropdown = t.GetComponent<TMP_Dropdown>();
            }
        }

        if (!resolutionDropdown)
        {
            var t = transform.Find("ResolutionDropdown");
            if (t)
            {
                resolutionDropdown = t.GetComponent<TMP_Dropdown>();
            }
        }

        if (!fullscreenDropdown)
        {
            fullscreenDropdown = transform.Find("ScreenDropDown").GetComponent<TMP_Dropdown>();
        }

        if (resolutionDropdown && aspectRatioDropdown)
        {
            InitializeResolutionDropdowns();
        }

        if (fullscreenDropdown)
        {
            InitializeFullscreenDropdown();
        }
    }

    //OPTIONS CONTROLS


    public void ChangeWindowed(int n)
    {
        if (n == 0)
        {
            //Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (n == 1)
        {
            //Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (n == 2)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void ChangeAspectRatio(int n)
    {
        resolutionDropdown.ClearOptions();
        currentAspectRatio = dropdownKeys.First(x => x.Value == n).Key;
        if (currentAspectRatio == "Auto")
        {
            Resolution resolution;
            if (Screen.fullScreen)
            {
                //еще одна причина не уважать Юнити - причина написания этого куска
                //но он все равно не работает так, как надо
                
                //FullScreenMode prev_mod = Screen.fullScreenMode;
                //Screen.fullScreenMode = FullScreenMode.Windowed;
                resolution = new Resolution();
                resolution.width = Display.displays[0].systemWidth;
                resolution.height = Display.displays[0].systemHeight;
                Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, Screen.fullScreen);
                //Screen.fullScreenMode = prev_mod;
            }
            else
            {
                resolution = Screen.currentResolution;
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            }

            resolutionDropdown.AddOptions(new List<string> { resolution.width + "x" + resolution.height });
        }
        else
        {
            resolutionDropdown.AddOptions(standard_resolutions[currentAspectRatio].Select(x => x.Item1 + "x" + x.Item2).ToList());
            (int, int) new_resolution = standard_resolutions[currentAspectRatio].First();
            Screen.SetResolution(new_resolution.Item1, new_resolution.Item2, Screen.fullScreen);
        }
    }

    public void UpdateResolution(int n)
    {
        (int, int) resolution = standard_resolutions[currentAspectRatio][n];
        Screen.SetResolution(resolution.Item1, resolution.Item2, Screen.fullScreen);
    }

    public void InitializeResolutionDropdowns()
    {
        (int, int) resolution = (Screen.width, Screen.height);
        currentAspectRatio = "";
        foreach (var kv in standard_resolutions)
        {
            if (kv.Value.Contains(resolution))
            {
                currentAspectRatio = kv.Key;
                break;
            }
        }
        resolutionDropdown.ClearOptions();
        if (currentAspectRatio == "")
        {
            currentAspectRatio = "Auto";
            resolutionDropdown.AddOptions(new List<string>{ resolution.Item1 + "x" + resolution.Item2});
        }
        else
        {
            resolutionDropdown.AddOptions(standard_resolutions[currentAspectRatio].Select(x => x.Item1 + "x" + x.Item2).ToList());
            resolutionDropdown.SetValueWithoutNotify(standard_resolutions[currentAspectRatio].IndexOf(resolution));
        }

        aspectRatioDropdown.SetValueWithoutNotify(dropdownKeys[currentAspectRatio]);
    }

    public void InitializeFullscreenDropdown()
    {
        
        switch (Screen.fullScreenMode)
        {
            
            case FullScreenMode.FullScreenWindow:
                {
                    
                    fullscreenDropdown.value = 0;
                    break;
                }
            case FullScreenMode.ExclusiveFullScreen:
                {
                    fullscreenDropdown.value = 1;
                    break;
                }
            case FullScreenMode.Windowed:
                {
                    //mainMenuScript.ShowNotImplementedWarning();
                    fullscreenDropdown.value = 2;
                    break;
                }
            default:
                {
                    fullscreenDropdown.value = 0;
                    break;
                }
        }
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
