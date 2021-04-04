using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;
public class SaveSerial : MonoBehaviour
{
    //GameObject gameobjectToSave;
    public GameObject player;

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/mynew.dat", FileMode.Create))
        {
            SaveData data = new SaveData();
            data.savedsceneInd = SceneManager.GetActiveScene().buildIndex;
            
            data.savedPlayerPosX = player.transform.position.x;
            data.savedPlayerPosY = player.transform.position.y;
            data.savedPlayerPosZ = player.transform.position.z;
            bf.Serialize(fs, data);
        }
        Debug.Log("Game data saved!");
    }

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
    public int savedsceneInd;
    public float savedPlayerPosX;
    public float savedPlayerPosY;
    public float savedPlayerPosZ;
}

