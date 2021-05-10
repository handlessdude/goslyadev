using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//TODO: трешовый скрипт вышел, без поллитра не резберёшься
public class WorldSwitcher : MonoBehaviour
{
    public VolumeProfile aberrationProfile;

    bool aberrationFadeIn = false;
    bool aberrationFadeOut = false;

    float fadeSpeed = 0.1f;
    float aberrationAmount = 0.8f;

    ChromaticAberration aberration;

    public static World currentWorld = World.none;

    public GameObject cyanGlow;
    public GameObject magentaGlow;
    public GameObject greenGlow;
    GameObject currentGlow;

    Dictionary<World, GameObject[]> objects;

    public enum World
    {
        none,
        cyan,
        green,
        magenta
    }

    void Start()
    {
        if (!aberrationProfile)
        {
            aberrationProfile = FindObjectsOfType<Volume>().First(x => x.name == "AberrationVolume").profile;
        }

        if (!aberrationProfile.TryGet<ChromaticAberration>(out aberration))
        {
            throw new System.Exception("Chromatic Aberration not found in WorldSwitcher!");
        }

        if (!cyanGlow || !magentaGlow || !greenGlow)
        {
            throw new System.Exception("Glowing objects for light bulb are not set in WorldSwitcher!");
        }

        objects = new Dictionary<World, GameObject[]>();
        objects[World.cyan] = GameObject.FindGameObjectsWithTag("CyanWorld");
        objects[World.magenta] = GameObject.FindGameObjectsWithTag("MagentaWorld");
        objects[World.green] = GameObject.FindGameObjectsWithTag("GreenWorld");
        foreach (KeyValuePair<World, GameObject[]> kv in objects)
        {
            foreach (GameObject go in kv.Value)
            {
                go.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!GameplayState.isPaused)
        {
            if (InputManager.GetKeyDown(KeyAction.WorldGreen))
            {
                if (currentWorld == World.green)
                {
                    DefaultWorld();
                }
                else
                {
                    GreenWorld();
                }
            }
            else if (InputManager.GetKeyDown(KeyAction.WorldCyan))
            {
                if (currentWorld == World.cyan)
                {
                    DefaultWorld();
                }
                else
                {
                    CyanWorld();
                }
            }
            else if (InputManager.GetKeyDown(KeyAction.WorldMagenta))
            {
                if (currentWorld == World.magenta)
                {
                    DefaultWorld();
                }
                else
                {
                    MagentaWorld();
                }
            }
        }
       
    }

    void FixedUpdate()
    {
        if (aberrationFadeIn)
        {
            if (aberration.intensity.value < aberrationAmount)
            {
                aberration.intensity.value += fadeSpeed;
            }
            else
            {
                aberrationFadeIn = false;
            }
        }
        else if (aberrationFadeOut)
        {
            if (aberration.intensity.value > 0.0f)
            {
                aberration.intensity.value -= fadeSpeed;
            }
            else
            {
                aberrationFadeOut = false;
            }
        }
    }

    public void DefaultWorld()
    {
        if (currentWorld != World.none)
        {
            DisableAberration();
            foreach (GameObject go in objects[currentWorld])
            {
                go.SetActive(false);
            }
            currentWorld = World.none;
            currentGlow.SetActive(false);
        }
    }

    void CyanWorld()
    {
        StartWorldChange();
        currentWorld = World.cyan;
        cyanGlow.SetActive(true);
        currentGlow = cyanGlow;
        foreach (GameObject go in objects[currentWorld])
        {
            go.SetActive(true);
        }
    }

    void MagentaWorld()
    {
        StartWorldChange();
        currentWorld = World.magenta;
        magentaGlow.SetActive(true);
        currentGlow = magentaGlow;
        foreach (GameObject go in objects[currentWorld])
        {
            go.SetActive(true);
        }
    }

    void GreenWorld()
    {
        StartWorldChange();
        currentWorld = World.green;
        greenGlow.SetActive(true);
        currentGlow = greenGlow;
        foreach (GameObject go in objects[currentWorld])
        {
            go.SetActive(true);
        }
    }

    void StartWorldChange()
    {
        if (currentWorld == World.none)
        {
            EnableAberration();
        }
        else
        {
            foreach (GameObject go in objects[currentWorld])
            {
                go.SetActive(false);
            }
            currentGlow.SetActive(false);
        }
    }

    public void EnableAberration()
    {
        aberrationFadeIn = true;
    }

    public void DisableAberration()
    {
        aberrationFadeOut = true;
    }
}
