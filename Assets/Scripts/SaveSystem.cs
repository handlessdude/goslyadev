using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;
public class SaveSystem : MonoBehaviour
{
    public GameObject player;
    public MainMenu Menu;
    public GameObject NoSaveWarning;
    public GameObject Hint;
    void Start()
    {
        if (!player)
        {
            player = GameObject.FindWithTag("Player");
        }
        /*if (!Hint)
        {
            Hint = GameObject.Find("GameSaved");
            Hint.SetActive(false);
        }*/
        foreach (var x in GameplayState.killedEnemy)
            StartCoroutine(KillKilledEnemy(x, 0.15f));
    }
    IEnumerator KillKilledEnemy(string x, float time)
    {
        yield return new WaitForSeconds(time);
        if (x != "Boss")
            GameObject.Find(x).GetComponent<SimpleBot>().Die(GameObject.FindWithTag("Player"));
        else
            GameObject.Find(x).GetComponent<SawBot>().Die(GameObject.FindWithTag("Player"));
    }
    public void ShowNoSaveWarning()
    {
        NoSaveWarning.SetActive(true);
        CancelInvoke("HideNoSaveWarning");
        Invoke("HideNoSaveWarning", 1.5f);
    }

    void HideNoSaveWarning()
    {
        NoSaveWarning.SetActive(false);
    }

    public void ShowHint()
    {
        Hint.SetActive(true);
        CancelInvoke("HideHint");
        Invoke("HideHint", 1.5f);
    }

    void HideHint()
    {
        Hint.SetActive(false);
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/saved.dat", FileMode.Create))
        {
            SaveData data = new SaveData();

            data.savedKillsMobs = GameplayState.killedEnemy;
            data.savedsceneInd = SceneManager.GetActiveScene().buildIndex;
            //Первый лвл
            data.savedbarrels = GameplayState.barrels;
            data.savedfeededbarrels = GameplayState.feededBarrels;
            data.savepreparationended = GameplayState.isPreparationEnded;
            
            data.savedisDialogEnded = GameplayState.isDialogEnded;
            //
            data.savedisThiefRobotDialogEnded = GameplayState.isThiefRobotDialogEnded;
            data.savedisMainRobotDialogEnded = GameplayState.isMainRobotDialogEnded;
            data.savedboards = GameplayState.boards;

            foreach (var boxes in GameObject.FindGameObjectsWithTag("Boxes"))
            {
                var p = boxes.transform.position;
                data.savedBoxesPosition.Add(boxes.name, new Tuple<float, float>(p.x, p.y));
            }
            data.savedFeededboards = GameplayState.feededboards;
            data.savedDeletedList = GameplayState.deletedObjectsList;

            data.savedisStartedDialogEnded = GameplayState.isStartedDialogEnded;
            data.savedisFixererDiaglolEnded = GameplayState.isFixererDiaglolEnded;

            (data.savedPlayerPosX, data.savedPlayerPosY, data.savedPlayerPosZ) = (player.transform.position.x, player.transform.position.y, player.transform.position.z);

            //Сохраняет раскладку биндов
            data.savedKeyBindings = InputManager.bindings;
            data.savedPatterns = KeywordsReplacer.patterns;
            bf.Serialize(fs, data);
        }
        ShowHint();
        Debug.Log("Game data saved!");
    }
    //TODO: Сделать сериализацию ящиков.
    public void LoadGame()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/saved.dat", FileMode.Open))
            {
                SaveData data = (SaveData)bf.Deserialize(fs);
                Cursor.visible = false;
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(data.savedsceneInd, LoadSceneMode.Single);
                GameplayState.LevelStart();
                GameplayState.feededBarrels = data.savedfeededbarrels;

                GameplayState.barrels = data.savedbarrels;

                GameplayState.isPreparationEnded = data.savepreparationended;

                GameplayState.isDialogEnded = data.savedisDialogEnded;

                GameplayState.deletedObjectsList = data.savedDeletedList;

                GameplayState.isStartedDialogEnded = data.savedisStartedDialogEnded;

                GameplayState.isThiefRobotDialogEnded = data.savedisThiefRobotDialogEnded;

                GameplayState.isMainRobotDialogEnded = data.savedisMainRobotDialogEnded;

                GameplayState.isFixererDiaglolEnded = data.savedisFixererDiaglolEnded;

                GameplayState.feededboards = data.savedFeededboards;

                GameplayState.killedEnemy = data.savedKillsMobs;

                GameplayState.BoxesPosition = data.savedBoxesPosition;

                GameplayState.boards = data.savedboards;

                GameplayState.NewPositionPlayer = new Vector3(data.savedPlayerPosX, data.savedPlayerPosY, data.savedPlayerPosZ);
                GameplayState.isLoaded = true;
            }
            Debug.Log("Game data loaded!");

        }
        catch
        {
            ShowNoSaveWarning();
        }
    }

    public static void SaveKeys()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/keybindings.dat", FileMode.Create))
        {
            SaveData data = new SaveData();
            data.savedKeyBindings = InputManager.bindings;
            data.savedPatterns = KeywordsReplacer.patterns;
            bf.Serialize(fs, data);
        }
        Debug.Log("Bindings saved");
    }

    public static void LoadKeys()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/keybindings.dat", FileMode.Open))
            {
                SaveData data = (SaveData)bf.Deserialize(fs);
                KeywordsReplacer.patterns = new Dictionary<string, Func<string>>(data.savedPatterns);
                InputManager.bindings = new Dictionary<KeyAction, Tuple<KeyCode, KeyCode>>(data.savedKeyBindings);
            }
            Debug.Log("Bindings loaded");
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

}

[Serializable]
public class SaveData
{
    public HashSet<string> savedKillsMobs;
    public Dictionary<string, Tuple<float, float>> savedBoxesPosition = new Dictionary<string, Tuple<float, float>>();
    public Dictionary<KeyAction, Tuple<KeyCode, KeyCode>> savedKeyBindings = new Dictionary<KeyAction, Tuple<KeyCode, KeyCode>>();
    public Dictionary<string, Func<string>> savedPatterns = new Dictionary<string, Func<string>>();
    public List<string> savedDeletedList;
    public bool savedisDialogEnded;
    public bool savedisFixererDiaglolEnded;
    public bool savepreparationended;
    public int savedfeededbarrels;
    public int savedbarrels;
    public int savedsceneInd;
    public float savedPlayerPosX;
    public float savedPlayerPosY;
    public float savedPlayerPosZ;
    public bool savedisThiefRobotDialogEnded;
    public bool savedisMainRobotDialogEnded;
    public bool savedisStartedDialogEnded;
    public int savedboards;
    public int savedFeededboards;
}

