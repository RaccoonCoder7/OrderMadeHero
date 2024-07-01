using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private string folderPath;
    private string fileName;
    public Text playerName;
    public Text saveDate;
    public Text progress;
    public Text chapter;

    private void Start()
    {
        folderPath = Application.persistentDataPath + "/saves";
        fileName = gameObject.name;
        CallSlotInfo();
    }

    public void CallSlotInfo()
    {
        string filePath = folderPath + "/"+ fileName + ".json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(json);
            string newJson = System.Text.Encoding.UTF8.GetString(bytes);
            PlayerData data = JsonUtility.FromJson<PlayerData>(newJson);
        
            playerName.text = data.playerName;
            progress.text = "Week: " + data.weekData + "  " + "Day: " + data.dayData;
            saveDate.text = data.savedDate;
        }
        else
        {
            playerName.text = "No Data";
            progress.text = "No Data";
            saveDate.text = "No Data";
            chapter.text = "No Data";
        }
    }
}
