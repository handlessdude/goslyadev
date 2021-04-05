﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;
public class SaveSerial : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindWithTag("Player");
        }
    }
    
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/mynew.dat", FileMode.Create))
        {
            SaveData data = new SaveData();
            data.savedsceneInd = SceneManager.GetActiveScene().buildIndex;
            //Первый лвл
            data.savedbarrels = GameplayState.barrels;
            data.savedfeededbarrels = GameplayState.feededBarrels;
            data.savepreparationended = GameplayState.isPreparationEnded;
            //TODO
            data.savedisDialogEnded = GameplayState.isDialogEnded;
            //
            data.savedDeletedList = GameplayState.deletedObjectsList;  
            
            (data.savedPlayerPosX, data.savedPlayerPosY, data.savedPlayerPosZ) = (player.transform.position.x,player.transform.position.y,player.transform.position.z);
            
            bf.Serialize(fs, data);
        }
        Debug.Log("Game data saved!");
    }
    //TODO: Сделать сериализацию ящиков.
    public void LoadGame()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(Application.persistentDataPath + "/mynew.dat", FileMode.Open))
            {
                SaveData data = (SaveData)bf.Deserialize(fs);
                Cursor.visible = false;
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(data.savedsceneInd,LoadSceneMode.Single);
                GameplayState.LevelStart();
                GameplayState.feededBarrels = data.savedfeededbarrels;
                GameplayState.barrels = data.savedbarrels;
                GameplayState.isPreparationEnded = data.savepreparationended;
                GameplayState.isDialogEnded = data.savedisDialogEnded;
                GameplayState.deletedObjectsList = data.savedDeletedList;

                GameplayState.NewPositionPlayer = new Vector3(data.savedPlayerPosX, data.savedPlayerPosY, data.savedPlayerPosZ);
                GameplayState.isLoaded = true;
            }
            Debug.Log("Game data loaded!");
            
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
           
    }

}

[Serializable]
public class SaveData
{
    public List<string> savedDeletedList;
    public bool savedisDialogEnded;
    public bool savepreparationended;
    public int savedfeededbarrels;
    public int savedbarrels;
    public int savedsceneInd;
    public float savedPlayerPosX;
    public float savedPlayerPosY;
    public float savedPlayerPosZ;
}

