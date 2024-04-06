using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int playerCredit;
    public int weekData;
    public GameMgr.Day dayData;
    public int playerTend;
    public int playerFame;

    public PlayerData(string name, int credit, int week, GameMgr.Day day, int tend, int fame)
    {
        playerName = name;
        playerCredit = credit;
        weekData = week;
        dayData = day;
        playerTend = tend;
        playerFame = fame;
    }
}

public class DataSaveLoad : MonoBehaviour
{
    public static DataSaveLoad Current { get; private set; }

    private DataSaveLoad() {}

    private void Awake()
    {
        if (Current == this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        Current = this;
    }

    public void SaveData()
    {
        Debug.Log("data saved");
        PlayerData data = new PlayerData(CommonTool.In.playerName,
            GameMgr.In.credit,
            GameMgr.In.week,
            GameMgr.In.day,
            GameMgr.In.tendency,
            GameMgr.In.fame);

        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/save.json";
        
        File.WriteAllText(path, json);
    }
}