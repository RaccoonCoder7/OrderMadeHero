using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
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
    public int playerDayShopBuyCost;
    public int playerDayChipUseCost;
    public int customerCnt;
    public int playerLastDayCredit;
    public int playerLastDayTend;
    public int playerLastDayFame;
    public string savedDate;
    public int isEventOngoing = 0;
    public int newsDone = 0;
    public PlayerData(string name, int credit, int week, GameMgr.Day day, int tend, int fame, int dayFame, int dayTend, 
        int dayShopBuy, int dayChipUse, int cusCnt, int lastCredit, int lastTend, int lastFame, int dayEvent, int newsCount, string date)
    {
        playerName = name;
        playerCredit = credit;
        weekData = week;
        dayData = day;
        playerTend = tend;
        playerFame = fame;
        playerDayFame = dayFame;
        playerDayTend = dayTend;
        playerDayShopBuyCost = dayShopBuy;
        playerDayChipUseCost = dayChipUse;
        customerCnt = cusCnt;
        playerLastDayCredit = lastCredit;
        playerLastDayTend = lastTend;
        playerLastDayFame = lastFame;
        isEventOngoing = dayEvent;
        newsDone = newsCount;
        savedDate = date;
    }
}

[Serializable]
public class StringBoolEntry
{
    public string key;
    public bool value;
}

[Serializable]
public class UnlockedWeaponInfo 
{
    public List<BluePrintState> bluePrintStatesList = new List<BluePrintState>();
    public List<ChipState> chipStatesList = new List<ChipState>();
    public List<OrderState> orderStatesList = new List<OrderState>();

    [Serializable]
    public class BluePrintState
    {
    public string bluePrintKey;
    public bool createEnable;
    public bool orderEnable;
    public int bpWeaponState;
    }
    [Serializable]
    public class ChipState 
    {
        public string chipKey;
        public bool createEnable;
        public int bpChipState;
    }
    [Serializable]
    public class OrderState 
    {
        public string orderKey;
        public bool orderEnable;
        public List<StringBoolEntry> orderConditionList;
        
        [NonSerialized]
        public StringBool orderConditionDictionary;
        
        public void OnBeforeSerialize()
        {
            orderConditionList = new List<StringBoolEntry>();
            foreach (var kvp in orderConditionDictionary)
            {
                orderConditionList.Add(new StringBoolEntry() { key = kvp.Key, value = kvp.Value });
            }
        }
        
        public void OnAfterDeserialize()
        {
            orderConditionDictionary = new StringBool();
            foreach (var entry in orderConditionList)
            {
                orderConditionDictionary.Add(entry.key, entry.value);
            }
        }
    }
}

public class DataSaveLoad : MonoBehaviour
{
    public bool isLoaded = false;
    private string folderPath;

    public GameObject slots1, slots2, slots3;
    public Button toLeft, toRight;
    public Camera mainCam;
    public Button delete;

    public WeaponDataTable weaponDataTable;
    public ChipTable chipTable;
    public OrderTable orderTable;
    
    private enum SlotState
    {
        First,
        Second,
        Third
    }

    private SlotState state;
    
    public static DataSaveLoad dataSave { get; private set; }

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
        state = SlotState.First;
        folderPath = Application.persistentDataPath + "/saves";
    }

    private void Update()
    {
        if (GameSceneMgr.isSavePopupActive || StartSceneMgr.isSavePopupActive)
        {
            SetActiveSlot();
        }
    }
    
    private UnlockedWeaponInfo GenerateUnlockedWeaponInfo()
    {
        var result = new UnlockedWeaponInfo();
        foreach (var category in weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bluePrint in category.bluePrintList)
            {
                if (bluePrint.createEnable || bluePrint.orderEnable)
                {
                    result.bluePrintStatesList.Add(new UnlockedWeaponInfo.BluePrintState() { bluePrintKey = bluePrint.bluePrintKey, createEnable = bluePrint.createEnable, orderEnable = bluePrint.orderEnable, bpWeaponState = bluePrint.weaponState});
                }
            }
        }
        foreach (var chip in chipTable.chipList)
        {
            if(chip.createEnable)
            {
                result.chipStatesList.Add(new UnlockedWeaponInfo.ChipState() { chipKey = chip.chipKey, createEnable = chip.createEnable, bpChipState = chip.chipState} );
            }
        }

        foreach (var order in orderTable.orderList)
        {
                var orderState = new UnlockedWeaponInfo.OrderState()
                {
                    orderKey = order.orderKey,
                    orderEnable = order.orderEnable,
                    orderConditionDictionary = order.orderConditionDictionary
                };
            
                orderState.OnBeforeSerialize();
                result.orderStatesList.Add(orderState);
        }
        return result;
    }
    
    private void ApplyUnlockedWeaponInfo(UnlockedWeaponInfo info)
    {
        foreach (var bluePrintState in info.bluePrintStatesList)
        {
            foreach (var category in weaponDataTable.bluePrintCategoryList)
            {
                foreach (var bluePrint in category.bluePrintList)
                {
                    if (bluePrint.bluePrintKey == bluePrintState.bluePrintKey)
                    {
                        bluePrint.createEnable = bluePrintState.createEnable;
                        bluePrint.orderEnable = bluePrintState.orderEnable;
                        bluePrint.weaponState = bluePrintState.bpWeaponState;
                        break;
                    }
                }
            }
        }

        foreach (var chipState in info.chipStatesList)
        {
            foreach (var chip in chipTable.chipList)
            {
                if (chip.chipKey == chipState.chipKey)
                {
                    chip.createEnable = chipState.createEnable;
                    chip.chipState = chipState.bpChipState;
                    break;
                }
            }
        }

        foreach (var orderState in info.orderStatesList)
        {
            orderState.OnAfterDeserialize();
            
            foreach (var order in orderTable.orderList)
            {
                if (order.orderKey == orderState.orderKey)
                {
                    order.orderEnable = orderState.orderEnable;
                    order.orderConditionDictionary = orderState.orderConditionDictionary;
                }
            }
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

        toLeft.onClick.RemoveListener(ClickToLeft);
        toRight.onClick.RemoveListener(ClickToRight);
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
    
    public Texture2D DeleteSS(string fileName)
    {
        string filePath = folderPath + "/" + fileName + "_screenshot.png";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
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
            GameMgr.In.dayShopBuyCost,
            GameMgr.In.dayChipUseCost,
            GameMgr.In.dayCustomerCnt,
            GameMgr.In.lastDayCredit,
            GameMgr.In.lastDayTend,
            GameMgr.In.lastDayFame,
            GameMgr.In.isEventOn,
            GameMgr.In.newsProgress,
            DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

        string json = JsonUtility.ToJson(data);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string newJson = Convert.ToBase64String(bytes);
        
        Directory.CreateDirectory(folderPath);
        string filePath = folderPath + "/"+ fileName + ".json";
        
        File.WriteAllText(filePath, newJson);
        
        UnlockedWeaponInfo unlockedWeaponInfo = GenerateUnlockedWeaponInfo();
        foreach (var orderState in unlockedWeaponInfo.orderStatesList)
        {
            orderState.OnBeforeSerialize();
        }


        string unlockedWeaponInfoJson = JsonUtility.ToJson(unlockedWeaponInfo);
        bytes = System.Text.Encoding.UTF8.GetBytes(unlockedWeaponInfoJson);
        newJson = Convert.ToBase64String(bytes);

        filePath = folderPath + "/"+ fileName + "_unlocked_info.json";
    
        File.WriteAllText(filePath, newJson);
    }


    public void LoadData(string fileName)
    {
        isLoaded = true;
        
        string filePath = folderPath + "/"+ fileName + ".json";
        string unlockedFilePath = folderPath + "/"+ fileName + "_unlocked_info.json";
        Debug.Log("data loaded");
        if (File.Exists(unlockedFilePath))
        {
            string json = File.ReadAllText(unlockedFilePath);
            byte[] bytes = Convert.FromBase64String(json);
            string newJson = System.Text.Encoding.UTF8.GetString(bytes);
            UnlockedWeaponInfo unlockedWeaponInfo = JsonUtility.FromJson<UnlockedWeaponInfo>(newJson);
            foreach (var orderState in unlockedWeaponInfo.orderStatesList)
            {
                orderState.OnAfterDeserialize();
            }
            
            ApplyUnlockedWeaponInfo(unlockedWeaponInfo);
        }
        else
        {
            Debug.LogError("No unlocked info data");
        } 
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
            GameMgr.In.dayShopBuyCost = data.playerDayShopBuyCost;
            GameMgr.In.dayChipUseCost = data.playerDayChipUseCost;
            GameMgr.In.dayCustomerCnt = data.customerCnt;
            GameMgr.In.lastDayCredit = data.playerLastDayCredit;
            GameMgr.In.lastDayTend = data.playerLastDayTend;
            GameMgr.In.lastDayFame = data.playerLastDayFame;
            GameMgr.In.newsProgress = data.newsDone;
            GameMgr.In.isEventOn = data.isEventOngoing;
            
            StartCoroutine(CommonTool.In.AsyncChangeScene("GameScene"));
        }
        else
        {
            Debug.LogError("no data");
        }
    }
    
    public void DeleteData(string fileName)
    {
        string filePath = folderPath + "/" + fileName + ".json";
        string unlockedFilePath = folderPath + "/" + fileName + "_unlocked_info.json";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save file deleted: " + filePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
        }

        if (File.Exists(unlockedFilePath))
        {
            File.Delete(unlockedFilePath);
            Debug.Log("Unlocked info deleted: " + unlockedFilePath);
        }
        else
        {
            Debug.LogWarning("Unlocked info file not found: " + unlockedFilePath);
        }
    }
}