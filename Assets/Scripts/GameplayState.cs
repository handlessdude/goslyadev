using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;

public static class GameplayState
{
    public static PlayerControllability controllability = PlayerControllability.Full;
    public static bool isPaused = false;
    public static int barrels = 0;
    public static int feededBarrels = 0;
    public static Vector3 NewPositionPlayer;
    public static bool isLoaded = false;
    public static bool isDialogEnded = false;
    public static bool isPreparationEnded = false; 
    public static List<string> deletedObjectsList = new List<string>();
    
    public static void LevelStart()
    {
        isPaused = false;
        controllability = PlayerControllability.Full;
    }

}

public enum PlayerControllability
{
    Full,
    InDialogue,
}
