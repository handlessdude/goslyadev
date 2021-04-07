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
    public static List<string> deletedObjectsList = new List<string>();
    public static bool isPaused = false;
    public static int barrels = 0;
    public static int feededBarrels = 0;
    public static bool isPreparationEnded = false;
    public static Vector3 NewPositionPlayer;
    public static bool isLoaded;
    public static bool isDialogEnded = false;
    //SecondLvl
    public static bool isThiefRobotDialogEnded = false;

    public static bool isMainRobotDialogEnded = false;


    public static bool isStartedDialogEnded = false;
    public static int boards = 0;
    public static Dictionary<string, Tuple<float, float>> BoxesPosition = new Dictionary<string, Tuple<float, float>>();
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
    FirstDialog
}
