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
    public Image saveScreen;

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
            var savedSS = DataSaveLoad.dataSave.LoadSS(fileName);
            saveScreen.sprite = Sprite.Create(savedSS, new Rect(0, 0, savedSS.width, savedSS.height), new Vector2(0.5f, 0.5f));
            string json = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(json);
            string newJson = System.Text.Encoding.UTF8.GetString(bytes);
            PlayerData data = JsonUtility.FromJson<PlayerData>(newJson);
        
            playerName.text = data.playerName;
            progress.text = "Week: " + data.weekData + "  " + "Day: " + data.dayData;
            saveDate.text = data.savedDate;
            chapter.text = "CH.0" + data.weekData;
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
