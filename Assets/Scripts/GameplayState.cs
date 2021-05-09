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
    public static bool isPaused;
    public static int barrels;
    public static int feededBarrels;
    public static bool isPreparationEnded;

    public static Vector3 NewPositionPlayer;

    public static bool IsOnClick;

    public static bool isOnAction;
    public static bool IsMovingBox;
    public static bool isLoaded;

    public static bool isDialogEnded;
    //SecondLvL
    public static bool isThiefRobotDialogEnded;

    public static bool isMainRobotDialogEnded;

    public static bool isStartedDialogEnded;

    public static bool isFixererDiaglolEnded;

    public static bool isUnderElevator;

    public static int boards;
    public static int feededboards;
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
