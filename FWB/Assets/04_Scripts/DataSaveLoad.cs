using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int playerCredit;
    public int weekData;
    public GameMgr.Day dayData;
    public int playerTend;
    public int playerFame;
    public int playerDayFame;
    public int playerDayTend;
    public int playerDaySpent;
    public int customerCnt;
    public string savedDate;

    public PlayerData(string name, int credit, int week, GameMgr.Day day, int tend, int fame, int dayFame, int dayTend, int daySpent, int cusCnt, string date)
    {
        playerName = name;
        playerCredit = credit;
        weekData = week;
        dayData = day;
        playerTend = tend;
        playerFame = fame;
        playerDayFame = dayFame;
        playerDayTend = dayTend;
        playerDaySpent = daySpent;
        customerCnt = cusCnt;
        savedDate = date;
    }
}

public class DataSaveLoad : MonoBehaviour
{
    public bool isLoaded = false;
    private string folderPath;

    public GameObject slots1, slots2, slots3;
    public Button toLeft, toRight;
    public Camera mainCam;
    
    private enum SlotState
    {
        First,
        Second,
        Third
    }

    private SlotState state;
    
    public static DataSaveLoad dataSave { get; private set; }

    private DataSaveLoad() {}

    private void Awake()
    {
        if (dataSave == null)
        {
            dataSave = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        toLeft.onClick.AddListener(ClickToLeft);
        toRight.onClick.AddListener(ClickToRight);
        state = SlotState.First;
        folderPath = Application.persistentDataPath + "/saves";
    }

    private void Update()
    {
        if (GameSceneMgr.isSavePopupActive)
        {
            SetActiveSlot();
        }
    }

    private void ClickToLeft()
    {
        state = SwitchToNextState(state, false);
    }

    private void ClickToRight()
    {
        state = SwitchToNextState(state, true);
    }
    
    private SlotState SwitchToNextState(SlotState currentState, bool isRightDirection)
    {
        switch (currentState)
        {
            case SlotState.First:
                return isRightDirection ? SlotState.Second : SlotState.Third;
            case SlotState.Second:
                return isRightDirection ? SlotState.Third : SlotState.First;
            case SlotState.Third:
                return isRightDirection ? SlotState.First : SlotState.Second;
            default:
                throw new InvalidOperationException("invalid state");
        }
    }
    
    private void SetActiveSlot()
    {
        slots1.SetActive(state == SlotState.First);
        slots2.SetActive(state == SlotState.Second);
        slots3.SetActive(state == SlotState.Third);
    }

    public void AssignSceneObjects(GameObject s1, GameObject s2, GameObject s3, Button left, Button right, Camera mCam)
    {
        slots1 = s1;
        slots2 = s2;
        slots3 = s3;
        
        toLeft = left;
        toRight = right;

        mainCam = mCam;

        toLeft.onClick.AddListener(ClickToLeft);
        toRight.onClick.AddListener(ClickToRight);
    }
    
    public Texture2D MakeScreenShot()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mainCam.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        mainCam.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        mainCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        return screenshot;
    }

    public void SaveSS(Texture2D screenshot, string fileName)
    {
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(folderPath + "/" + fileName + "_screenshot.png", bytes);
    }

    public Texture2D LoadSS(string fileName)
    {
        string filePath = folderPath + "/" + fileName + "_screenshot.png";
        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            return texture;
        }
        return null;
    }

    public void SaveData(string fileName)
    {
        Debug.Log("data saved");
        PlayerData data = new PlayerData(CommonTool.In.playerName,
            GameMgr.In.credit,
            GameMgr.In.week,
            GameMgr.In.day,
            GameMgr.In.tendency,
            GameMgr.In.fame,
            GameMgr.In.dayFame,
            GameMgr.In.dayTendency,
            GameMgr.In.daySpendCredit,
            GameMgr.In.dayCustomerCnt,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

        string json = JsonUtility.ToJson(data);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string newJson = Convert.ToBase64String(bytes);
        
        Directory.CreateDirectory(folderPath);
        string filePath = folderPath + "/"+ fileName + ".json";
        
        File.WriteAllText(filePath, newJson);
    }

    public void LoadData(string fileName)
    {
        isLoaded = true;
        
        string filePath = folderPath + "/"+ fileName + ".json";
        Debug.Log("data loaded");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            byte[] bytes = Convert.FromBase64String(json);
            string newJson = System.Text.Encoding.UTF8.GetString(bytes);
            PlayerData data = JsonUtility.FromJson<PlayerData>(newJson);
        
            CommonTool.In.playerName = data.playerName;
            GameMgr.In.credit = data.playerCredit;
            GameMgr.In.week = data.weekData;
            GameMgr.In.day = data.dayData;
            GameMgr.In.tendency = data.playerTend;
            GameMgr.In.fame = data.playerFame;
            GameMgr.In.dayFame = data.playerDayFame;
            GameMgr.In.dayTendency = data.playerDayTend;
            GameMgr.In.daySpendCredit = data.playerDaySpent;
            GameMgr.In.dayCustomerCnt = data.customerCnt;
            
            StartCoroutine(CommonTool.In.AsyncChangeScene("GameScene"));
        }
        else
        {
            Debug.LogError("no data");
        }
    }
}