using System.Text;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Drawing;
using static SpriteChange;
using DG.Tweening;
using static WeaponDataTable;
using static GameMgr;
using System.IO;
using UnityEditorInternal;
using Debug = UnityEngine.Debug;

/// <summary>
/// 게임 씬의 UI와 동작(메인 게임 플로우)를 관리
/// </summary>
public class GameSceneMgr : MonoBehaviour, IDialogue
{

    [Header("UI")]
    public Button pc;
    public Button popupDodge;
    public Button save;
    public Button dataSave;
    public Button load;
    public Button ok;
    public Button yes;
    public Button no;
    public Button eventBtn1;
    public Button eventBtn2;
    public Button returnBtn;
    //public Button gotoMain;
    public List<Button> popupYes = new List<Button>();
    public List<Button> popupNo = new List<Button>();
    public Button setting;
    public Button skip;
    public Button shop;
    public Button index;
    public Button news;
    public Button weaponLeft;
    public Button weaponRight;
    public Button weaponCreate;
    public Button shopBlueprintTab;
    public Button shopChipsetTab;
    public Button shopDodge;
    public Button shopPageUp;
    public Button shopPageDown;
    public List<Button> saveLoadButtons = new List<Button>();
    public Button alertDodge;
    public Button creditDodge;
    public Button bankruptDodge;
    public Button gotoMain;
    public Button history;
    public Button historyDodge;
    public Text chatName;
    public GameObject mainChatPanel;
    public GameObject pcChatPanel;
    public GameObject popupChatPanel;
    public GameObject popupPanel;
    public GameObject fullPanel;
    public GameObject saveLoadPanel;
    public GameObject savePopup;
    public GameObject loadPopup;
    public GameObject popUpDim;
    public GameObject saveOverPopup;
    public GameObject noDataPopup;
    public GameObject newsPanel;
    public static bool isSavePopupActive = false;
    public Sprite selectedSaveSlot;
    public Sprite defaultSaveSlot;
    public GameObject slots1, slots2, slots3;
    public Button toLeft, toRight;
    public Camera mainCam;
    public GameObject day;
    public GameObject renom;
    public GameObject renomMask;
    public GameObject tendency;
    public GameObject tendPoint;
    public GameObject gold;
    public GameObject gamePanel;
    // public GameObject cursor;
    public GameObject alertPanel;
    public GameObject historyPanel;
    private Image alertPanelImg;
    public Sprite chipsetAlertImg;
    public GameObject getItemImg;
    public GameObject getItemText;
    public List<Sprite> getItemSprites = new List<Sprite>();
    public List<Sprite> emojiSprites = new List<Sprite>();
    public List<String> getItemTexts = new List<String>();
    public GameObject creditPanel;
    public GameObject bankruptPanel;
    public GameObject shopUiSlotNoItemPrefab;
    public GameObject shopDrMadChat;
    public GameObject shopControlBlockingPanel;
    public GameObject deskNavi;
    public GameObject shopPopupPanel;
    public GameObject weaponDatasBlock;
    public GameObject renomUIBlock;
    public GameObject tendencyUIBlock;
    public List<GameObject> mobAvatarList = new List<GameObject>();
    public ShopUISlot shopUiSlotPrefab;
    public ShopUISlot shopUiSlotSoldOutPrefab;
    public UISlot uiSlotPrefab;
    public ShopFollowUI shopFollowUI;
    public ShopPopupUI shopPopupUI;
    public BlueprintImgChanger blueprintImgChanger;
    public CustomScrollBar scrollBar;
    public Text mainChatText;
    public Text mascotChatText;
    public Text popupChatText;
    public Text fullChatName;
    public Text fullChatText;
    public GameObject newsChatPanel;
    public Text newsChatName;
    public Text newsChatText;
    public Sprite newsLeftBox;
    public Sprite newsRightBox;
    public bool inNews = false;
    public Text yesText;
    public Text noText;
    public Text eventBtntext1;
    public Text eventBtntext2;
    public Text dateText;
    public Text weekText;
    public Text dateMessage;
    public Text goldText;
    public Text creditTitle;
    public Text creditRevenue;
    public Text creditBonusRevenue;
    public Text creditShopBuyCost;
    public Text creditChipUseCost;
    public Text creditRentCost;
    public Text creditTotalRevenue;
    public Text creditCustomerCnt;
    public Text creditRenom;
    public Text creditTendency;
    public Text creditRevenueResult;
    public Text weaponName;
    public Text popupOrderText;
    public Text comment;
    public Text essentialCondition;
    public Text specialGimmick;
    public Text weaponCategory;
    public Text howToGet;
    public Text historyText;
    public Image blueprintImg;
    public Image emoji;
    public List<IntroSceneMgr.ImageData> imageList = new List<IntroSceneMgr.ImageData>();
    public List<UISlot> bluePrintSlotList = new List<UISlot>();
    [Header("For Test")]
    public int startDay = 1;
    public Transform weaponCategoryParentTr;
    public Transform deskTr;
    public Transform shopPanelTr;
    public Transform shopItemParentTr;
    public RectTransform popupChatPanelRect;
    public Sprite blankSlotSprite;
    public List<Sprite> shopTabSpriteList;
    public SpriteAnimation shopSpriteAnim;
    [HideInInspector]
    public Text chatTargetText;
    public float textDelayTime { get; set; }
    [HideInInspector]
    public bool isEventFlowing;
    [HideInInspector]
    public bool isNormalOrdering;
    [HideInInspector]
    public OrderState orderState = OrderState.None;
    [HideInInspector]
    public ChatTarget chatTarget = ChatTarget.Main;
    [HideInInspector]
    public ChatTarget prevChatTarget = ChatTarget.None;
    [HideInInspector]
    public List<UISlot> bluePrintCategorySlotList = new List<UISlot>();
    [HideInInspector]
    public List<ShopUISlot> shopUISlotList = new List<ShopUISlot>();

    [HideInInspector]
    public PuzzleMgr puzzleMgr;

    private int fadeSpeed = 1;
    [SerializeField]
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private bool isWaitingForText;
    private bool isFeverModeTutorialDone;
    private bool isFeverMode;
    private bool isFeverModeConfirmed;
    public bool autoTextSkip { get; set; }
    private List<string> lines = new List<string>();
    private int lineCnt = 0;
    private string prevOrderKey = "";
    private int normalOrderLineIndex = 0;
    private int normalOrderPrevLineIndex = 0;
    private int currentSelectedWeaponIndex = -1;
    private int currentShopPage = 0;
    private float textSkipWaitTime = 1f;
    private string prevText;
    private string currentSelectedWeaponCategoryKey;
    private Action onEndText;
    private Action onSkip;
    private Coroutine textFlowCoroutine;
    private Coroutine drMadChatRoutine;
    private Coroutine emojiRoutine;
    private Point cursorPos = new Point();
    private bool visible;
    private bool isShopAnimating;
    private bool isShopTutorial = true;
    private Image shopBlueprintTabImg;
    private Image shopChipsetTabImg;
    private List<EventFlow> eventFlowList = new List<EventFlow>();
    private ShopTab currentShopTab = ShopTab.None;
    private List<GameObject> activatedObjList = new List<GameObject>();
    private RectTransform blueprintImgRectTr;
    private SpriteChange indexSC;
    private int previousMobAvatarIndex = 0;
    private string saveSlot;
    private bool isSaving;
    private Texture2D currentScreen;
    private GameObject lastSelectedSlot = null;
    private Image lastSelectedSlotImage = null;
    public bool gameoverTest = false;
    private bool dailyRoutineEndFlag = false;

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);

    public enum ChatTarget
    {
        None,
        Main,
        Mascot,
        Popup,
        Full,
        News
    }

    public enum OrderState
    {
        None,
        Ordering,
        Accepted,
        Rejected,
        Succeed,
        Failed,
        Finished
    }

    public enum ShopTab
    {
        None,
        Blueprint,
        Chipset
    }


    private void FixedUpdate()
    {
        if (visible)
        {
            SetCursorPos(cursorPos.X, cursorPos.Y);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnClickChatBox();
        }
    }

    private IEnumerator Start()
    {
        CommonTool.In.canvas.worldCamera = Camera.main;

        blueprintImgRectTr = blueprintImg.GetComponent<RectTransform>();
        puzzleMgr = gamePanel.GetComponent<PuzzleMgr>();
        shopBlueprintTabImg = (Image)shopBlueprintTab.targetGraphic;
        shopChipsetTabImg = (Image)shopChipsetTab.targetGraphic;
        CommonTool.In.shopFollowUI = shopFollowUI;
        alertPanelImg = alertPanel.GetComponentInChildren<Image>();
        indexSC = index.GetComponent<SpriteChange>();

        DataSaveLoad.dataSave.AssignSceneObjects(slots1, slots2, slots3, toLeft, toRight, mainCam);

        popupDodge.onClick.AddListener(OnClickDodgePopup);
        weaponLeft.onClick.AddListener(OnClickWeaponLeft);
        weaponRight.onClick.AddListener(OnClickWeaponRight);
        weaponCreate.onClick.AddListener(OnClickWeaponCreate);
        save.onClick.AddListener(OnClickSave);
        dataSave.onClick.AddListener(OnClickDataSave);
        load.onClick.AddListener(OnClickDataLoad);
        returnBtn.onClick.AddListener(OnClickReturn);
        //gotoMain.onClick.AddListener(OnClickGoToMain);
        setting.onClick.AddListener(OnClickSetting);
        skip.onClick.AddListener(OnClickSkip);
        shopBlueprintTab.onClick.AddListener(OnClickShopBlueprintTab);
        shopChipsetTab.onClick.AddListener(OnClickShopChipsetTab);
        shopPageUp.onClick.AddListener(OnClickShopPageUp);
        shopPageDown.onClick.AddListener(OnClickShopPageDown);
        history.onClick.AddListener(OnClickHistory);
        historyDodge.onClick.AddListener(OnClickHistory);

        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.AddListener(() => { OnClickSlot(btn); });
        }

        foreach (var btn in popupYes)
        {
            btn.onClick.AddListener(OnClickPopupYes);
        }

        foreach (var btn in popupNo)
        {
            btn.onClick.AddListener(OnClickPopupNo);
        }

        eventFlowList = GetComponents<EventFlow>().ToList();
        foreach (var eventflow in eventFlowList)
        {
            eventflow.mgr = this;
        }

        RefreshWeaponCategoryButtons();

        yield return StartCoroutine(CommonTool.In.FadeIn());

        // TODO: day limit 추가
        //for test - 정발시 startday 기능 삭제할때 조건문도 삭제
        if (startDay != 1 && !GameMgr.In.isBankrupt && !gameoverTest)
        {
            GameMgr.In.isEventOn = 1;
            for (int i = startDay; i <= GameMgr.In.endDay; i++)
            {
                string eventKey = "day" + i;
                var targetEvent = eventFlowList.Find(x => x.eventKey.Equals(eventKey));
                isEventFlowing = true;
                if (targetEvent)
                {
                    yield return StartCoroutine(StartEventFlow(targetEvent));
                }
                else
                {
                    yield return StartCoroutine(StartNormalRoutine(5, EndNormalOrderRoutine));
                }

                while (isEventFlowing)
                {
                    yield return null;
                }
                yield return StartCoroutine(NextDay());
            }
        }
        else
        {
            if (!DataSaveLoad.dataSave.isLoaded && !GameMgr.In.isBankrupt)
            {
                OnBasicUI(startDay);
                StartCoroutine(TestDoNormalJob(startDay));
            }
            else if (GameMgr.In.isBankrupt)
            {
                OnBasicUI((int)GameMgr.In.day);
                Debug.Log("Bankrupt refresh");
                GameMgr.In.isBankrupt = false;
                isEventFlowing = false;
                StartCoroutine(TestDoNormalJob((int)GameMgr.In.day));
            }
            else
            {
                OnBasicUI((int)GameMgr.In.day);

                if (GameMgr.In.dayCustomerCnt <= 0 && GameMgr.In.isEventOn == 0)
                {
                    isNormalOrdering = false;

                    pc.image.raycastTarget = true;
                    var coroutine = StartCoroutine(BlinkNavi());
                    pc.onClick.RemoveAllListeners();
                    pc.onClick.AddListener(() =>
                    {
                        StopCoroutine(coroutine);
                        deskNavi.SetActive(true);
                        RefreshCreditPanel();
                        creditPanel.SetActive(true);
                        creditDodge.onClick.RemoveAllListeners();
                        creditDodge.onClick.AddListener(() =>
                        {
                            if (GameMgr.In.isBankrupt)
                            {
                                Bankrupt();
                            }
                            else
                            {
                                StartCoroutine(FadeToNextDay());
                                StartCoroutine(TestDoNormalJob((int)GameMgr.In.day));
                            }
                        });
                        pc.onClick.RemoveAllListeners();
                        pc.image.raycastTarget = false;
                        UpdateDayEndMessage();
                        FameUIFill();
                        TendUIMove();
                    });
                }
                else
                {
                    StartCoroutine(TestDoNormalJob((int)GameMgr.In.day));
                }
            }
        }
    }

    private void OnBasicUI(int gameStartDay)
    {
        gold.SetActive(true);
        goldText.text = GameMgr.In.credit.ToString();
        day.SetActive(true);
        dateText.text = GameMgr.In.day.ToString();
        weekText.text = GameMgr.In.week + "주차";
        if (GameMgr.In.week == 1)
        {
            if (gameStartDay > 2)
            {
                renom.SetActive(true);
                FameUIFill();
            }
            else if (gameStartDay == 2 && GameMgr.In.isEventOn == 0)
            {
                renom.SetActive(true);
                FameUIFill();
            }

            if (gameStartDay > 5)
            {
                tendency.SetActive(true);
                TendUIMove();
                FameUIFill();
            }
            else if (gameStartDay == 5 && GameMgr.In.isEventOn == 0)
            {
                tendency.SetActive(true);
                TendUIMove();
                FameUIFill();
            }
        }
        else
        {
            renom.SetActive(true);
            tendency.SetActive(true);
            TendUIMove();
            FameUIFill();
        }
    }

    private IEnumerator TestDoNormalJob(int eventStartDay)
    {
        Debug.Log("Start Event Sequence");
        for (int i = eventStartDay; i <= GameMgr.In.endDay; i++)
        {
            string eventKey = "day" + i;
            string weeklyEventKey = "week" + (GameMgr.In.week - 1);
            var targetEvent = eventFlowList.Find(x => x.eventKey.Equals(eventKey));
            var weeklyTargetEvent = eventFlowList.Find(x => x.eventKey.Equals(weeklyEventKey));
            isEventFlowing = true;

            if (weeklyTargetEvent != null && GameMgr.In.week > 1)
            {
                yield return StartCoroutine(StartEventFlow(weeklyTargetEvent));
            }
            if (targetEvent != null && GameMgr.In.week == 1)
            {
                yield return StartCoroutine(StartEventFlow(targetEvent));
            }
            if (!GameMgr.In.isBankrupt && !DataSaveLoad.dataSave.isLoaded)
            {
                Debug.Log("Start Order");
                yield return StartCoroutine(StartNormalRoutine(5, EndNormalOrderRoutine));
            }
            else if (DataSaveLoad.dataSave.isLoaded)
            {
                Debug.Log("Start Loaded Order");
                yield return StartCoroutine(StartNormalRoutine(GameMgr.In.dayCustomerCnt, EndNormalOrderRoutine));
            }
            while (isEventFlowing)
            {
                yield return null;
            }
            if (i < GameMgr.In.endDay && GameMgr.In.isBankrupt == false)
            {
                yield return StartCoroutine(NextDay());
            }
        }
    }

    public void GetChipset(int a)
    {
        getItemImg.GetComponent<Image>().sprite = getItemSprites[a];
        getItemText.GetComponent<Text>().text = getItemTexts[a];

        alertPanel.SetActive(true);
        getItemImg.SetActive(true);
        getItemText.SetActive(true);
        alertPanelImg.sprite = chipsetAlertImg;

        if (a == 1)
        {
            RectTransform rectTransform = getItemImg.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(150, 150);
        }
    }

    public void OnClickDodgePopup()
    {
        popupPanel.SetActive(false);
        indexSC.type = SpriteChangeType.Auto;
        indexSC.secPerAutoChange = 0.5f;
        indexSC.StartAutoMove();
        index.onClick.AddListener(OnClickIndex);
    }

    public void OnClickWeaponLeft()
    {
        for (int i = currentSelectedWeaponIndex - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(bluePrintSlotList[i].key))
            {
                continue;
            }

            bluePrintSlotList[i].button.onClick.Invoke();
            return;
        }
    }

    public void OnClickWeaponRight()
    {
        for (int i = currentSelectedWeaponIndex + 1; i < bluePrintSlotList.Count; i++)
        {
            if (string.IsNullOrEmpty(bluePrintSlotList[i].key))
            {
                continue;
            }

            bluePrintSlotList[i].button.onClick.Invoke();
            return;
        }
    }

    public void OnClickWeaponCreate()
    {
        CommonTool.In.OpenConfirmPanel("이 청사진으로 제작 하시겠습니까?",
        () =>
        {
            popupPanel.SetActive(false);
            gamePanel.SetActive(true);
            puzzleMgr.OnMakingDone += (result) =>
            {
                gamePanel.SetActive(false);
            };
            var key = bluePrintSlotList[currentSelectedWeaponIndex].key;
            GameMgr.In.currentBluePrint = GameMgr.In.GetWeapon(currentSelectedWeaponCategoryKey, key);
            puzzleMgr.StartPuzzle();
        });
    }

    public void ObjectBlinker(GameObject gameObject, int blinkTimes, float duration)
    {
        StartCoroutine(ObjectBlink(gameObject, blinkTimes, duration));
    }

    IEnumerator ObjectBlink(GameObject gameObject, int numberOfBlinks, float duration)
    {
        var blinkDuration = 0.2f;
        int counter = 0;
        float startTime = Time.time;

        while (counter < numberOfBlinks && Time.time - startTime < duration)
        {
            gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkDuration);
            gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkDuration);

            counter++;
        }
        gameObject.SetActive(true);
    }

    public void MobSpriteRandomChange()
    {
        int randomIndex = 0;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, mobAvatarList.Count);
        } while (previousMobAvatarIndex == randomIndex);

        mobAvatarList[previousMobAvatarIndex].SetActive(false);
        mobAvatarList[randomIndex].SetActive(true);

        previousMobAvatarIndex = randomIndex;

    }
    public void SetIndexBlueprintImgAspect(Sprite targetSprite)
    {
        var xMax = 450f;
        var yMax = 300f;
        var additionalRario = xMax / yMax;

        var ratio = targetSprite.rect.size.x / targetSprite.rect.size.y;
        Vector2 size = Vector2.zero;
        if (targetSprite.rect.size.x <= targetSprite.rect.size.y * additionalRario)
        {
            size = new Vector2(yMax * ratio, yMax);
        }
        else
        {
            size = new Vector2(xMax, xMax / ratio);
        }
        blueprintImgRectTr.sizeDelta = size;
    }

    public void OnClickSave()
    {
        currentScreen = DataSaveLoad.dataSave.MakeScreenShot();
        saveLoadPanel.SetActive(true);
        isSavePopupActive = true;
    }

    public void OnClickReturn()
    {
        saveLoadPanel.SetActive(false);
        isSavePopupActive = false;
    }

    //public void OnClickGoToMain()
    //{
    //    StartCoroutine(CommonTool.In.AsyncChangeScene("StartScene"));
    //}

    public void OnClickSlot(Button btn)
    {
        if (lastSelectedSlotImage != null)
        {
            lastSelectedSlotImage.sprite = defaultSaveSlot;
        }
        lastSelectedSlotImage = btn.GetComponent<Image>();
        lastSelectedSlotImage.sprite = selectedSaveSlot;

        saveSlot = btn.name;
        Debug.Log(saveSlot);
    }

    public void OnClickDataSave()
    {
        isSaving = true;
        if (File.Exists(Application.persistentDataPath + "/saves" + "/" + saveSlot + ".json"))
        {
            popUpDim.SetActive(true);
            saveOverPopup.SetActive(true);
        }
        else
        {
            popUpDim.SetActive(true);
            savePopup.SetActive(true);
        }
    }

    public void OnClickDataLoad()
    {
        if (File.Exists(Application.persistentDataPath + "/saves" + "/" + saveSlot + ".json"))
        {
            isSaving = false;
            popUpDim.SetActive(true);
            loadPopup.SetActive(true);
        }
        else
        {
            popUpDim.SetActive(true);
            StartCoroutine(NoDataBlinker());
        }
    }

    private IEnumerator NoDataBlinker()
    {
        noDataPopup.SetActive(true);
        float elapsedTime = 0f;
        float blinkInterval = 1f / 2f;
        bool isVisible = true;

        while (elapsedTime < 2f)
        {
            isVisible = !isVisible;
            noDataPopup.SetActive(isVisible);
            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }
        popUpDim.SetActive(false);
        noDataPopup.SetActive(false);
    }

    public void OnClickPopupYes()
    {
        if (isSaving)
        {
            DataSaveLoad.dataSave.SaveData(saveSlot);
            DataSaveLoad.dataSave.SaveSS(currentScreen, saveSlot);
            savePopup.SetActive(false);
            saveOverPopup.SetActive(false);
        }
        else
        {
            DataSaveLoad.dataSave.LoadData(saveSlot);
            loadPopup.SetActive(false);
        }
        popUpDim.SetActive(false);
        isSavePopupActive = false;
        GameObject slotObj = GameObject.Find(saveSlot);
        slotObj.GetComponent<SaveSlot>().CallSlotInfo();
    }

    public void OnClickSetting()
    {
        // 필요시 사용
        // GetCursorPos(out cursorPos);
        // Cursor.visible = visible;
        // visible = !visible;
        // cursor.SetActive(visible);
    }

    public void OnClickSkip()
    {
        if (onSkip != null)
        {
            var tempOnSkip = onSkip;
            tempOnSkip.Invoke();
            if (tempOnSkip == onSkip)
            {
                onSkip = null;
            }
        }
    }

    public void OnClickPopupNo()
    {
        popUpDim.SetActive(false);
        savePopup.SetActive(false);
        loadPopup.SetActive(false);
        saveOverPopup.SetActive(false);
    }

    public void OnClickChatBox()
    {
        if (!isOnConversation) return;
        if (isWaitingForText) return;

        if (isTextFlowing)
        {
            // SkipCurrLine();
            // StartNextLine();
            return;
        }

        if (lines.Count <= lineCnt + 1)
        {
            isOnConversation = false;
            onEndText.Invoke();
            return;
        }

        StartNextLine();
    }

    public void OnClickShopBlueprintTab()
    {
        if (currentShopTab == ShopTab.Blueprint) return;
        shopBlueprintTabImg.sprite = shopTabSpriteList[1];
        shopChipsetTabImg.sprite = shopTabSpriteList[2];
        currentShopTab = ShopTab.Blueprint;
        currentShopPage = 0;
        RefreshShopUI();
    }

    public void OnClickShopChipsetTab()
    {
        if (currentShopTab == ShopTab.Chipset) return;
        shopChipsetTabImg.sprite = shopTabSpriteList[3];
        shopBlueprintTabImg.sprite = shopTabSpriteList[0];
        currentShopTab = ShopTab.Chipset;
        currentShopPage = 0;
        RefreshShopUI();
    }

    public void OnClickShopPageUp()
    {
        currentShopPage--;
        RefreshShopUI();
    }

    public void OnClickShopPageDown()
    {
        currentShopPage++;
        RefreshShopUI();
    }

    public void OnClickShopDodge()
    {
        if (isShopAnimating) return;
        isShopAnimating = true;
        StartCoroutine(StartShopOutAnim());
    }

    public void OnClickHistory()
    {
        StartCoroutine(scrollBar.DelayScroll());
        historyPanel.SetActive(!historyPanel.activeSelf);
    }

    public void ActiveYesNoButton(bool isActive)
    {
        yes.gameObject.SetActive(isActive);
        no.gameObject.SetActive(isActive);
    }

    public void ActiveEventButton(bool isActive)
    {
        eventBtn1.gameObject.SetActive(isActive);
        eventBtn2.gameObject.SetActive(isActive);
    }

    public void StartText(string textName, Action onEndText, Action onSkip = null)
    {
        lineCnt = -1;
        lines = CommonTool.In.GetText(textName);
        isOnConversation = true;
        this.onEndText = onEndText;
        this.onSkip = onSkip;
        textFlowCoroutine = StartCoroutine(StartTextFlow());
        StartNextLine();
    }

    public void EndText(bool clearText = true)
    {
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;
        prevChatTarget = ChatTarget.None;
        if (clearText)
        {
            chatTargetText.text = string.Empty;
        }
    }

    public void RefreshCreditPanel()
    {
        creditTitle.text = GameMgr.In.week + "주차 " + GameMgr.In.day + "요일";
        creditRevenue.text = "무기판매 +" + GameMgr.In.dayRevenue;
        creditBonusRevenue.text = "완성보너스 +" + GameMgr.In.dayBonusRevenue;
        creditShopBuyCost.text = "상점구매비 " + GameMgr.In.dayShopBuyCost;
        creditChipUseCost.text = "칩셋사용비 " + GameMgr.In.dayChipUseCost;

        int week = GameMgr.In.week;
        switch (week)
        {
            case 1:
                goldText.text = GameMgr.In.credit.ToString();
                break;
            case 2:
                GameMgr.In.dayRentCost = -100;
                break;
            case 3:
                GameMgr.In.dayRentCost = -250;
                break;
            case 4:
            case 5:
                GameMgr.In.dayRentCost = -500;
                break;
            default:
                if (week > 5)
                {
                    GameMgr.In.dayRentCost = -1000;
                }
                break;
        }
        if (week >= 2)
        {
            var money = GameMgr.In.credit + GameMgr.In.dayRentCost;
            if (money <= 0)
            {
                GameMgr.In.isBankrupt = true;
            }
            else
            {
                GameMgr.In.credit += GameMgr.In.dayRentCost;
                GameMgr.In.lastDayCredit = GameMgr.In.credit;
                GameMgr.In.lastDayFame = GameMgr.In.fame;
                GameMgr.In.lastDayTend = GameMgr.In.tendency;
            }

            goldText.text = money.ToString();
        }

        /*
        if (GameMgr.In.day == Day.금)
        {
            GameMgr.In.dayRentCost = -100;
            var money = GameMgr.In.credit + GameMgr.In.dayRentCost;
            if (money <= 0)
            {
                GameMgr.In.isBankrupt = true;
                goldText.text = GameMgr.In.credit.ToString();
            }
            else
            {
                goldText.text = GameMgr.In.credit.ToString();
                GameMgr.In.lastWeekCredit = GameMgr.In.credit;
                GameMgr.In.lastWeekFame = GameMgr.In.fame;
                GameMgr.In.lastWeekTend = GameMgr.In.tendency;
            }
        }
        else
        {
            GameMgr.In.dayRentCost = 0;
        }
        */
        creditRentCost.text = "임대료 " + GameMgr.In.dayRentCost;
        var totalRevenue = GameMgr.In.dayRevenue + GameMgr.In.dayBonusRevenue + GameMgr.In.dayRentCost
                         + GameMgr.In.dayShopBuyCost + GameMgr.In.dayChipUseCost;
        creditTotalRevenue.text = totalRevenue + " 크레딧";
        creditCustomerCnt.text = GameMgr.In.dayCustomerCnt.ToString();
        if (renom.activeInHierarchy)
        {
            renomUIBlock.SetActive(true);
            creditRenom.text = GameMgr.In.dayFame.ToString();
        }
        if (tendency.activeInHierarchy)
        {
            tendencyUIBlock.SetActive(true);
            creditTendency.text = GameMgr.In.dayTendency.ToString();
        }
    }

    public void RefreshPopupPanel()
    {
        if (!string.IsNullOrEmpty(mainChatText.text))
        {
            popupOrderText.text = mainChatText.text;
        }

        if (bluePrintCategorySlotList.Count > 0)
        {
            bluePrintCategorySlotList[0].button.onClick.Invoke();
            if (bluePrintSlotList.Count >= 0)
            {
                bluePrintSlotList[0].button.onClick.Invoke();
            }
        }
    }

    public void RefreshWeaponCategoryButtons()
    {
        var categoryList = GameMgr.In.weaponDataTable.bluePrintCategoryList;
        int j = 0;
        for (int i = 0; i < categoryList.Count; i++)
        {
            int tempNum = i;
            var category = categoryList[tempNum];
            if (category.categoryKey.Equals("t_special"))
            {
                continue;
            }
            var weaponCategorySlot = Instantiate(uiSlotPrefab, weaponCategoryParentTr);
            weaponCategorySlot.key = category.categoryKey;
            weaponCategorySlot.spriteChange.onSprite = category.onSprite;
            weaponCategorySlot.spriteChange.offSprite = category.offSprite;
            weaponCategorySlot.spriteChange.SetOffSprite();
            weaponCategorySlot.spriteChange.type = SpriteChangeType.OnClick;
            if (category.bluePrintList.Count > 0)
            {
                weaponCategorySlot.button.onClick.AddListener(() =>
                {
                    OnClickWeaponCategoryButton(tempNum);
                    currentSelectedWeaponCategoryKey = category.categoryKey;
                    currentSelectedWeaponIndex = -1;
                    RefreshWeaponButtons();
                });
            }
            if (i == 0)
            {
                weaponCategorySlot.spriteChange.SetOnSprite();
                weaponCategorySlot.button.onClick.Invoke();
            }

            bluePrintCategorySlotList.Add(weaponCategorySlot);
        }
    }

    public void RefreshWeaponButtons()
    {
        var category = GameMgr.In.GetWeaponCategory(currentSelectedWeaponCategoryKey);
        for (int i = 0; i < 9; i++)
        {
            int tempNum = i;
            if (category.bluePrintList.Count <= i)
            {
                SetEmptySlot(bluePrintSlotList[i]);
                continue;
            }

            var bp = category.bluePrintList[i];
            if (!bp.createEnable)
            {
                SetEmptySlot(bluePrintSlotList[i]);
                continue;
            }

            bluePrintSlotList[i].key = bp.bluePrintKey;
            bluePrintSlotList[i].innerImage.sprite = bp.icon;
            bluePrintSlotList[i].innerImage.color = Vector4.one;
            bluePrintSlotList[i].spriteChange.SetOffSprite();
            bluePrintSlotList[i].button.onClick.AddListener(() =>
            {
                OnClickWeaponButton(tempNum);
            });
            if (i == 0)
            {
                bluePrintSlotList[i].spriteChange.SetOnSprite();
                bluePrintSlotList[i].button.onClick.Invoke();
            }
        }
    }

    public void EndNormalOrderRoutine()
    {
        Debug.Log("End Daily Order");
        EndText();
        isNormalOrdering = false;

        pc.image.raycastTarget = true;
        var coroutine = StartCoroutine(BlinkNavi());
        pc.onClick.RemoveAllListeners();
        pc.onClick.AddListener(() =>
        {
            DataSaveLoad.dataSave.isLoaded = false;
            StopCoroutine(coroutine);
            deskNavi.SetActive(true);
            RefreshCreditPanel();
            creditPanel.SetActive(true);
            creditDodge.onClick.RemoveAllListeners();
            creditDodge.onClick.AddListener(() =>
            {
                if (GameMgr.In.isBankrupt)
                {
                    Bankrupt();
                    dailyRoutineEndFlag = false;
                }
                else
                {
                    StartCoroutine(FadeToNextDay());
                    dailyRoutineEndFlag = true;
                }
            });
            pc.onClick.RemoveAllListeners();
            pc.image.raycastTarget = false;
            UpdateDayEndMessage();
            FameUIFill();
            TendUIMove();
            if ((int)GameMgr.In.day <= 7 && !GameMgr.In.isBankrupt)
            {
                GameMgr.In.isEventOn = 1;
            }
        });
    }

    public void Bankrupt()
    {
        creditPanel.SetActive(false);
        isEventFlowing = false;
        bankruptPanel.SetActive(true);
        bankruptDodge.onClick.AddListener(() =>
        {
            GameMgr.In.credit = GameMgr.In.lastDayCredit;
            GameMgr.In.fame = GameMgr.In.lastDayFame;
            GameMgr.In.tendency = GameMgr.In.lastDayTend;
            GameMgr.In.ResetDayData();
            bankruptPanel.SetActive(false);
            StartCoroutine(CommonTool.In.AsyncChangeScene("GameScene"));
        });
        gotoMain.onClick.AddListener(() =>
        {
            bankruptPanel.SetActive(false);
            StartCoroutine(CommonTool.In.AsyncChangeScene("StartScene"));
        });
        FameUIFill();
        TendUIMove();
    }

    public void SkipToLastLine()
    {
        if (currentLineIdex < lines.Count - 1)
        {
            lineCnt = currentLineIdex = lines.Count - 1;
            StartNextLine();
        }

        if (chatTargetText)
        {
            chatTargetText.text = lines[lines.Count - 1] + " ▼";
        }
    }

    public void RefreshShopUI()
    {
        foreach (Transform tr in shopItemParentTr)
        {
            Destroy(tr.gameObject);
        }

        int listCnt = 0;
        shopUISlotList.Clear();
        int startIndex = (currentShopPage) * 10;
        if (currentShopTab == ShopTab.Blueprint)
        {
            List<BluePrint> bluePrintList = new List<BluePrint>();
            foreach (var category in GameMgr.In.weaponDataTable.bluePrintCategoryList)
            {
                foreach (var bp in category.bluePrintList)
                {
                    if (bp.howToGet.Equals("상점구매"))
                    {
                        bluePrintList.Add(bp);
                    }
                }
            }
            listCnt = bluePrintList.Count;

            for (int i = startIndex; i < startIndex + 10; i++)
            {
                if (i < listCnt)
                {
                    var state = bluePrintList[i].weaponState;
                    bool isSellable = state < 3;
                    ShopUISlot targetSlotPrefab = isSellable ? shopUiSlotPrefab : shopUiSlotSoldOutPrefab;
                    ShopUISlot item = Instantiate(targetSlotPrefab, shopItemParentTr);

                    item.contentImg.sprite = bluePrintList[i].blueprintSprite;
                    item.contentName.text = bluePrintList[i].name;
                    if (item.contentImg.sprite != null)
                    {
                        var targetSprite = bluePrintList[i].blueprintSprite;
                        var ratio = targetSprite.rect.size.x / targetSprite.rect.size.y;
                        Vector2 size = Vector2.zero;
                        if (targetSprite.rect.size.x <= targetSprite.rect.size.y)
                        {
                            size = new Vector2(128 * ratio, 128);
                        }
                        else
                        {
                            size = new Vector2(128, 128 / ratio);
                        }

                        item.contentImg.rectTransform.sizeDelta = size;
                    }
                    item.price = bluePrintList[i].buyPrice;

                    StringBuilder sb = new StringBuilder();
                    foreach (var ability in bluePrintList[i].requiredChipAbilityList)
                    {
                        var targetAbility = GameMgr.In.abilityTable.abilityList.Find(x => x.abilityKey.Equals(ability.abilityKey));
                        sb.Append(targetAbility.name).Append("+ ").Append(ability.count).Append("\n");
                    }
                    sb.Length--;
                    item.itemData = sb.ToString();

                    if (isSellable)
                    {
                        item.priceText.text = item.price.ToString();

                        int tempNum = i;
                        item.button.onClick.AddListener(() =>
                        {
                            shopPopupUI.itemName.text = item.contentName.text;
                            shopPopupUI.no.onClick.AddListener(() => { shopPopupPanel.gameObject.SetActive(false); });
                            if (item.price <= GameMgr.In.credit)
                            {
                                shopPopupUI.yes.interactable = true;
                                shopPopupUI.yes.onClick.AddListener(() =>
                                {
                                    var bp = GameMgr.In.GetWeapon(bluePrintList[tempNum].bluePrintKey);
                                    bp.createEnable = true;
                                    GameMgr.In.credit -= item.price;
                                    GameMgr.In.dayShopBuyCost -= item.price;
                                    goldText.text = GameMgr.In.credit.ToString();
                                    bluePrintList[tempNum].weaponState = 3;
                                    if (drMadChatRoutine != null)
                                    {
                                        StopCoroutine(drMadChatRoutine);
                                        drMadChatRoutine = null;
                                    }
                                    drMadChatRoutine = StartCoroutine(ShowDrMadChat());
                                    shopPopupPanel.gameObject.SetActive(false);
                                    RefreshShopUI();
                                    shopPopupUI.yes.onClick.RemoveAllListeners();
                                });
                            }
                            else
                            {
                                shopPopupUI.yes.interactable = false;
                            }
                            shopPopupPanel.gameObject.SetActive(true);
                        });
                    }
                    shopUISlotList.Add(item);
                    continue;
                }

                Instantiate(shopUiSlotNoItemPrefab, shopItemParentTr);
            }
        }
        else if (currentShopTab == ShopTab.Chipset)
        {
            var chipList = GameMgr.In.chipTable.chipList.FindAll(x => x.howToGet.Equals("상점구매"));
            if (chipList != null)
            {
                listCnt = chipList.Count;
            }
            for (int i = startIndex; i < startIndex + 10; i++)
            {
                if (i < listCnt)
                {
                    var state = chipList[i].chipState;
                    bool isSellable = state < 3;
                    ShopUISlot targetSlotPrefab = isSellable ? shopUiSlotPrefab : shopUiSlotSoldOutPrefab;
                    ShopUISlot item = Instantiate(targetSlotPrefab, shopItemParentTr);

                    item.contentImg.sprite = chipList[i].chipSprite;
                    item.contentName.text = chipList[i].chipName;

                    var targetSprite = chipList[i].chipSprite;
                    var ratio = targetSprite.rect.size.x / targetSprite.rect.size.y;
                    Vector2 size = Vector2.zero;
                    if (targetSprite.rect.size.x <= targetSprite.rect.size.y)
                    {
                        size = new Vector2(128 * ratio, 128);
                    }
                    else
                    {
                        size = new Vector2(128, 128 / ratio);
                    }

                    item.contentImg.rectTransform.sizeDelta = size;
                    item.price = chipList[i].price;

                    StringBuilder sb = new StringBuilder();
                    foreach (var ability in chipList[i].abilityList)
                    {
                        var targetAbility = GameMgr.In.abilityTable.abilityList.Find(x => x.abilityKey.Equals(ability.abilityKey));
                        sb.Append(targetAbility.name).Append("+ ").Append(ability.count).Append("\n");
                    }
                    sb.Length--;
                    item.itemData = sb.ToString();

                    if (isSellable)
                    {
                        item.priceText.text = item.price.ToString();

                        int tempNum = i;
                        item.button.onClick.AddListener(() =>
                        {
                            shopPopupUI.itemName.text = item.contentName.text;
                            shopPopupUI.no.onClick.AddListener(() => { shopPopupPanel.gameObject.SetActive(false); });
                            if (item.price <= GameMgr.In.credit)
                            {
                                shopPopupUI.yes.interactable = true;
                                shopPopupUI.yes.onClick.AddListener(() =>
                                {
                                    var chip = GameMgr.In.GetChip(chipList[tempNum].chipKey);
                                    chip.createEnable = true;
                                    GameMgr.In.credit -= item.price;
                                    GameMgr.In.dayShopBuyCost -= item.price;
                                    goldText.text = GameMgr.In.credit.ToString();
                                    chipList[tempNum].chipState = 3;
                                    puzzleMgr.creatableChipKeyList.Add(chip.chipKey);
                                    if (drMadChatRoutine != null)
                                    {
                                        StopCoroutine(drMadChatRoutine);
                                        drMadChatRoutine = null;
                                    }
                                    drMadChatRoutine = StartCoroutine(ShowDrMadChat());
                                    shopPopupPanel.gameObject.SetActive(false);
                                    RefreshShopUI();
                                    shopPopupUI.yes.onClick.RemoveAllListeners();
                                });
                            }
                            else
                            {
                                shopPopupUI.yes.interactable = false;
                            }
                            shopPopupPanel.gameObject.SetActive(true);
                        });
                    }
                    shopUISlotList.Add(item);
                    continue;
                }

                Instantiate(shopUiSlotNoItemPrefab, shopItemParentTr);
            }
        }

        shopPageUp.interactable = currentShopPage > 0;
        shopPageDown.interactable = 10 * (currentShopPage + 1) < listCnt;
    }

    public void SetShopButtonListener()
    {
        shop.onClick.AddListener(OnClickShop);
    }

    public void SetNewsButtonListener()
    {
        news.onClick.AddListener(OnClickNews);
    }

    private void OnClickShop()
    {
        if (isWaitingForText) return;

        switch (chatTarget)
        {
            case ChatTarget.Main:
                if (mainChatPanel.activeSelf)
                {
                    mainChatPanel.SetActive(false);
                    activatedObjList.Add(mainChatPanel);
                }
                break;
            case ChatTarget.Mascot:
                if (pcChatPanel.activeSelf)
                {
                    pcChatPanel.SetActive(false);
                    activatedObjList.Add(pcChatPanel);
                }
                break;
            case ChatTarget.Popup:
                if (popupChatPanel.activeSelf)
                {
                    popupChatPanel.SetActive(false);
                    activatedObjList.Add(popupChatPanel);
                }
                break;
        }
        foreach (var img in imageList)
        {
            if (img.imageObj.activeSelf)
            {
                img.imageObj.SetActive(false);
                activatedObjList.Add(img.imageObj);
            }
        }

        OnClickShopBlueprintTab();
        StartCoroutine(StartShopInAnim());
        shop.onClick.RemoveListener(OnClickShop);
    }

    private void OnClickNews()
    {
        StartCoroutine(OpenNewsAnim());
        //news.onClick.RemoveListener(OnClickNews);
    }

    private IEnumerator OpenNewsAnim()
    {
        CommonTool.In.fadeImage.gameObject.SetActive(true);
        CommonTool.In.fadeImage.color = new UnityEngine.Color(0, 0, 0, 1);
        StartCoroutine(CommonTool.In.FadeIn());
        newsPanel.SetActive(true);
        yield break;
    }

    private void SkipCurrLine()
    {
        skipLine = true;
    }

    private void StartNextLine()
    {
        lineCnt++;
    }

    private string ReplaceKeyword(string line)
    {
        return line.Replace("{username}", CommonTool.In.playerName);
    }

    private void EndOrderText()
    {
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;

        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            orderState = OrderState.Accepted;
        });

        no.onClick.RemoveAllListeners();
        no.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            orderState = OrderState.Rejected;
        });

        ActiveYesNoButton(true);
    }

    private void EndOrder()
    {
        EndText();
        HideEmoji();
        orderState = OrderState.Finished;
    }

    private IEnumerator NextDay()
    {
        GameMgr.In.ResetDayData();
        GameMgr.In.SetNextDayData();
        string day = GameMgr.In.day.ToString();
        dateText.text = day;
        dateMessage.text = GameMgr.In.week + "주차\n" + day + "요일";
        prevChatTarget = ChatTarget.None;
        SoundManager.PlayOneShot("bird");
        creditDodge.onClick.RemoveAllListeners();
        yield return StartCoroutine(FadeInOutDateMessage());
    }

    private List<string> SetOrderTextList(List<string> list)
    {
        for (normalOrderPrevLineIndex = normalOrderLineIndex; normalOrderLineIndex < lines.Count; normalOrderLineIndex++)
        {
            if (lines[normalOrderLineIndex].Contains("@"))
            {
                list = lines.GetRange(normalOrderPrevLineIndex, normalOrderLineIndex - normalOrderPrevLineIndex);
                normalOrderLineIndex++;
                break;
            }
        }
        return list;
    }

    private void OnClickIndex()
    {
        RefreshWeaponButtons();
        popupPanel.SetActive(true);
        indexSC.type = SpriteChangeType.OnFocus;
        indexSC.SetOffSprite();
        indexSC.StopAutoMove();
        index.onClick.RemoveAllListeners();
    }

    private void EndFeverModeStartChat()
    {
        ActiveYesNoButton(true);

        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() =>
        {
            EndText();
            ActiveYesNoButton(false);
            isFeverModeConfirmed = true;
            isFeverMode = true;
        });

        no.onClick.RemoveAllListeners();
        no.onClick.AddListener(() =>
        {
            EndText();
            ActiveYesNoButton(false);
            isFeverModeConfirmed = true;
        });

        if (!isFeverModeTutorialDone)
        {
            StartText("FeverMode_Tutorial", () =>
            {
                mainChatPanel.SetActive(true);
                pcChatPanel.SetActive(false);
                EndText();
            });
            isFeverModeTutorialDone = true;
        }
    }

    public IEnumerator FadeInOutDateMessage()
    {
        float fadeValue = 0;
        float actualSpeed = fadeSpeed * 0.01f;
        while (fadeValue < 1)
        {
            fadeValue += actualSpeed;
            dateMessage.color = new UnityEngine.Color(1, 1, 1, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
        yield return new WaitForSeconds(0.5f);
        while (fadeValue > 0)
        {
            fadeValue -= actualSpeed;
            dateMessage.color = new UnityEngine.Color(1, 1, 1, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
    }

    public IEnumerator StartNormalRoutine(int customerCnt, Action onEndRoutine)
    {
        GameMgr.In.isEventOn = 0;
        GameMgr.In.dayCustomerCnt = customerCnt;
        for (int i = 0; i < customerCnt; i++)
        {
            isFeverMode = false;
            if (GameMgr.In.week > 1)
            {
                var val = UnityEngine.Random.Range(0, 100);
                if (val <= GameMgr.In.feverModeProbability)
                {
                    // 테스트코드. 제거해야 함.
                    puzzleMgr.makingDone.gameObject.SetActive(true);
                    renom.SetActive(true);
                    gold.SetActive(true);
                    foreach (var chip in GameMgr.In.chipTable.chipList)
                    {
                        chip.createEnable = true;
                    }
                    foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
                    {
                        foreach (var bp in bpc.bluePrintList)
                        {
                            bp.createEnable = true;
                        }
                    }
                    // 테스트코드

                    // TODO: 단체손님 텍스트 수정 후 여기를 수정
                    StartText("FeverMode_1", EndFeverModeStartChat, EndFeverModeStartChat);

                    while (!isFeverModeConfirmed)
                    {
                        yield return null;
                    }
                    isFeverModeConfirmed = false;

                    GameMgr.In.feverModeProbability /= 10;
                    if (isFeverMode)
                    {
                        gamePanel.SetActive(true);
                        yield return StartCoroutine(puzzleMgr.StartFeverMode());
                        goldText.text = GameMgr.In.credit.ToString();
                        FameUIFill();
                    }

                    bool end = false;
                    // TODO: 단체손님 텍스트 수정 후 여기를 수정
                    StartText("FeverMode_1_End", () =>
                    {
                        imageList.Find(x => x.key.Equals("단체손님")).imageObj.SetActive(false);
                        mainChatPanel.SetActive(false);
                        end = true;
                        EndText();
                    });
                    while (!end)
                    {
                        yield return null;
                    }

                    continue;
                }
            }

            MobSpriteRandomChange();
            var orderTextList = new List<string>();
            var rejectTextList = new List<string>();
            var successTextList = new List<string>();
            var failTextList = new List<string>();

            var order = GameMgr.In.GetRandomNewOrder(prevOrderKey);
            prevOrderKey = order.orderKey;
            lines = order.ta.text.Split('\n').ToList();

            normalOrderLineIndex = 0;
            normalOrderPrevLineIndex = 0;
            orderTextList = SetOrderTextList(orderTextList);
            rejectTextList = SetOrderTextList(rejectTextList);
            successTextList = SetOrderTextList(successTextList);
            failTextList = SetOrderTextList(failTextList);

            lineCnt = -1;
            lines = orderTextList;
            isOnConversation = true;
            isNormalOrdering = true;
            orderState = OrderState.Ordering;
            this.onEndText = EndOrderText;
            prevChatTarget = ChatTarget.None;
            textFlowCoroutine = StartCoroutine(StartTextFlow());
            StartNextLine();

            while (orderState == OrderState.Ordering)
            {
                yield return null;
            }

            switch (orderState)
            {
                case OrderState.Accepted:
                    GameMgr.In.currentOrder = order;
                    StartPuzzleProcess();
                    break;
                case OrderState.Rejected:
                    AdjustFame(-25);
                    if (order.camp == OrderTable.Camp.Hero)
                    {
                        AdjustTendency(-25);
                    }
                    else if (order.camp == OrderTable.Camp.Villain)
                    {
                        AdjustTendency(25);
                    }
                    TendUIMove();
                    FameUIFill();
                    lineCnt = -1;
                    lines = rejectTextList;
                    isOnConversation = true;
                    this.onEndText = EndOrder;
                    emojiRoutine = StartCoroutine(ShowEmoji());
                    textFlowCoroutine = StartCoroutine(StartTextFlow());
                    StartNextLine();
                    break;
                default:
                    Debug.Log("Exception");
                    yield break;
            }

            bool processDone = false;
            while (!processDone)
            {
                if (orderState == OrderState.Succeed || orderState == OrderState.Failed)
                {
                    if (!isOnConversation)
                    {
                        foreach (var o in GameMgr.In.orderTable.orderList)
                        {
                            if (o.orderConditionDictionary.ContainsKey("AfterOneOrder"))
                            {
                                o.orderConditionDictionary["AfterOneOrder"] = true;
                                if (!o.orderConditionDictionary.ContainsValue(false))
                                {
                                    o.orderEnable = true;
                                }
                            }
                        }

                        GameMgr.In.SaveOrderHistory();

                        isOnConversation = true;
                        lineCnt = -1;
                        lines = orderState == OrderState.Succeed ? successTextList : failTextList;
                        this.onEndText = EndOrder;
                        emojiRoutine = StartCoroutine(ShowEmoji());
                        textFlowCoroutine = StartCoroutine(StartTextFlow());
                        StartNextLine();
                    }
                }
                else if (orderState == OrderState.Finished)
                {
                    TendUIMove();
                    FameUIFill();
                    foreach (var image in imageList)
                    {
                        image.imageObj.SetActive(false);
                    }
                    mainChatPanel.SetActive(false);
                    chatTarget = ChatTarget.None;
                    GameMgr.In.dayCustomerCnt -= 1;
                    processDone = true;
                }
                yield return null;
            }
        }

        foreach (var order in GameMgr.In.orderTable.orderList)
        {
            if (order.orderConditionDictionary.ContainsKey("AfterOneOrder"))
            {
                order.orderConditionDictionary["AfterOneOrder"] = true;
                if (!order.orderConditionDictionary.ContainsValue(false))
                {
                    order.orderEnable = true;
                }
            }
        }

        GameMgr.In.orderedBluePrintKeyList.Clear();

        dailyRoutineEndFlag = false;
        onEndRoutine.Invoke();
        while (!dailyRoutineEndFlag)
        {
            yield return null;
        }
    }

    public IEnumerator StartShopInAnim()
    {
        StartCoroutine(shopSpriteAnim.StartAnim());

        while (shopSpriteAnim.textureIndex < (shopSpriteAnim.textureList.Count / 3))
        {
            yield return null;
        }

        deskTr.transform.DOLocalMoveY(-770, 0.5f).SetEase(Ease.InOutQuart);

        while (shopSpriteAnim.textureIndex < (shopSpriteAnim.textureList.Count * 2 / 3))
        {
            yield return null;
        }

        shopPanelTr.gameObject.SetActive(true);
        shopPanelTr.transform.DOScale(Vector3.one, 0.5f);
    }

    public IEnumerator StartShopOutAnim()
    {
        StartCoroutine(shopSpriteAnim.StartAnim(true));

        while (shopSpriteAnim.textureIndex >= (shopSpriteAnim.textureList.Count * 2 / 3))
        {
            yield return null;
        }

        shopPanelTr.transform.DOScale(Vector3.zero, 0.5f);
        deskTr.transform.DOLocalMoveY(-540, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            shopPanelTr.gameObject.SetActive(false);
            SetShopButtonListener();

            foreach (var obj in activatedObjList)
            {
                obj.SetActive(true);
            }
            activatedObjList.Clear();

            isShopAnimating = false;
        });
    }

    public IEnumerator BlinkNavi()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            deskNavi.SetActive(!deskNavi.activeSelf);
        }
    }

    private int currentLineIdex;
    private IEnumerator StartTextFlow()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = ReplaceKeyword(lines[i]);
        }
        for (currentLineIdex = 0; currentLineIdex < lines.Count; currentLineIdex++)
        {
            if (lines[currentLineIdex].StartsWith("!"))
            {
                var commands = lines[currentLineIdex].Split(' ');
                List<string> imageKeyList = new List<string>();
                foreach (var command in commands)
                {
                    string com = command.Trim();
                    if (com.StartsWith("!image"))
                    {
                        imageKeyList.Add(com.Split('_')[1]);
                    }
                    else if (com.StartsWith("!speaker"))
                    {
                        var splittedCom = com.Split('_');
                        string speaker = splittedCom[1];
                        if (splittedCom.Length == 3)
                        {
                            if (speaker.Trim().Equals("popup"))
                            {
                                chatTarget = ChatTarget.Popup;
                                // TODO: speaker에 따라서 이미지 변경
                            }
                            else if (speaker.Trim().Equals("full"))
                            {
                                fullChatName.text = splittedCom[2];
                                chatTarget = ChatTarget.Full;
                            }
                            else if (speaker.Trim().Equals("news"))
                            {
                                var newsChatPanelImg = newsChatPanel.GetComponent<Image>();
                                if (!newsChatPanel.activeSelf)
                                {
                                    newsChatPanel.SetActive(true);
                                }
                                newsChatName.text = splittedCom[2];
                                if (newsChatName.text == "호크")
                                {
                                    newsChatPanelImg.sprite = newsLeftBox;
                                }
                                else if (newsChatName.text == "래트")
                                {
                                    newsChatPanelImg.sprite = newsRightBox;
                                }
                                chatTarget = ChatTarget.News;
                            }
                        }
                        else if (speaker.Equals(CommonTool.In.pcMascotName))
                        {
                            deskNavi.SetActive(true);
                            chatTarget = ChatTarget.Mascot;
                        }
                        else
                        {
                            if (speaker.Equals(CommonTool.In.mascotName))
                            {
                                deskNavi.SetActive(false);
                            }
                            chatTarget = ChatTarget.Main;
                            if (speaker.Equals("{username}"))
                            {
                                speaker = CommonTool.In.playerName;
                            }
                            chatName.text = speaker;
                        }
                        historyText.text += "\n<color=\"red\">" + speaker + "</color>\n";
                    }
                    else if (com.StartsWith("!sound"))
                    {
                        string clipName = com.Split('_')[1];
                        SoundManager.PlayOneShot(clipName);
                    }
                    else if (com.StartsWith("!focusoff"))
                    {
                        CommonTool.In.SetFocusOff();
                    }
                    else if (com.StartsWith("!focus"))
                    {
                        var splittedData = com.Split('_');
                        if (splittedData.Length == 5)
                        {
                            int posX = int.Parse(splittedData[1]);
                            int posY = int.Parse(splittedData[2]);
                            int width = int.Parse(splittedData[3]);
                            int height = int.Parse(splittedData[4]);
                            var pos = new Vector2(posX, posY);
                            var size = new Vector2(width, height);
                            CommonTool.In.SetFocus(pos, size, pcChatPanel.GetComponent<RectTransform>(), chatTargetText.rectTransform);
                        }
                    }
                    else if (com.StartsWith("!credit"))
                    {
                        string creditText = com.Split('_')[1];
                        int credit = int.Parse(creditText);
                        if (credit != 0)
                        {
                            GameMgr.In.credit += credit;
                            GameMgr.In.dayRevenue += credit;
                            goldText.text = GameMgr.In.credit.ToString();
                        }
                    }
                    else if (com.StartsWith("!wait"))
                    {
                        var splittedData = com.Split('_');
                        int time = int.Parse(splittedData[1]);
                        isWaitingForText = true;
                        yield return new WaitForSeconds(time / 100);
                        isWaitingForText = false;
                    }
                    else if (com.Equals("!next"))
                    {
                        lineCnt++;
                        prevText = string.Empty;
                        // historyText.text += "\n";
                    }
                }

                if (imageKeyList.Count > 0)
                {
                    foreach (var img in imageList)
                    {
                        img.imageObj.SetActive(imageKeyList.Contains(img.key));
                    }
                }

                while (currentLineIdex >= lineCnt)
                {
                    yield return new WaitForSeconds(textDelayTime);
                }
                continue;
            }

            if (prevChatTarget != chatTarget)
            {
                switch (chatTarget)
                {
                    case ChatTarget.Main:
                        mainChatPanel.SetActive(true);
                        pcChatPanel.SetActive(false);
                        popupChatPanel.SetActive(false);
                        fullPanel.SetActive(false);
                        deskTr.gameObject.SetActive(true);
                        pc.gameObject.SetActive(true);
                        chatTargetText = mainChatText;
                        break;
                    case ChatTarget.Mascot:
                        mainChatPanel.SetActive(false);
                        pcChatPanel.SetActive(true);
                        popupChatPanel.SetActive(false);
                        fullPanel.SetActive(false);
                        deskTr.gameObject.SetActive(true);
                        pc.gameObject.SetActive(true);
                        chatTargetText = mascotChatText;
                        break;
                    case ChatTarget.Popup:
                        mainChatPanel.SetActive(false);
                        pcChatPanel.SetActive(false);
                        popupChatPanel.SetActive(true);
                        fullPanel.SetActive(false);
                        deskTr.gameObject.SetActive(true);
                        pc.gameObject.SetActive(true);
                        chatTargetText = popupChatText;
                        if (inNews)
                        {
                            deskTr.gameObject.SetActive(false);
                            pc.gameObject.SetActive(false);
                        }
                        break;
                    case ChatTarget.Full:
                        mainChatPanel.SetActive(false);
                        pcChatPanel.SetActive(false);
                        popupChatPanel.SetActive(false);
                        fullPanel.SetActive(true);
                        deskTr.gameObject.SetActive(false);
                        pc.gameObject.SetActive(false);
                        chatTargetText = fullChatText;
                        break;
                    case ChatTarget.News:
                        mainChatPanel.SetActive(false);
                        pcChatPanel.SetActive(false);
                        popupChatPanel.SetActive(false);
                        fullPanel.SetActive(false);
                        deskTr.gameObject.SetActive(false);
                        pc.gameObject.SetActive(false);
                        chatTargetText = newsChatText;
                        break;

                }
                prevChatTarget = chatTarget;
            }

            if (!string.IsNullOrEmpty(prevText))
            {
                prevText = prevText.Replace(" ▼", string.Empty) + "\n";
            }

            isTextFlowing = true;
            historyText.text += lines[currentLineIdex] + "\n";
            for (int j = 0; j < lines[currentLineIdex].Length; j++)
            {
                chatTargetText.text = prevText + lines[currentLineIdex].Substring(0, j + 1);
                yield return new WaitForSeconds(textDelayTime);
                if (j == 0)
                {
                    StartCoroutine(scrollBar.DelayScroll());
                }

                if (skipLine)
                {
                    skipLine = false;
                    if (currentLineIdex + 1 >= lines.Count || lines[currentLineIdex + 1].Contains("!next"))
                    {
                        lineCnt--;
                    }
                    break;
                }
            }
            if (currentLineIdex >= lines.Count)
            {
                currentLineIdex = lines.Count - 1;
            }
            prevText = prevText + lines[currentLineIdex] + " ▼";
            chatTargetText.text = prevText;
            isTextFlowing = false;

            while (currentLineIdex >= lineCnt)
            {
                if (autoTextSkip)
                {
                    yield return new WaitForSeconds(textSkipWaitTime);
                    if (autoTextSkip)
                    {
                        lineCnt++;
                        continue;
                    }
                }
                yield return new WaitForSeconds(textDelayTime);
            }
        }
    }

    private void StartPuzzleProcess()
    {
        EndText(false);

        popupPanel.SetActive(true); //팝업 패널 = bp 인덱스
        RefreshPopupPanel();
    }

    private void OnClickWeaponCategoryButton(int targetNum)
    {
        for (int i = 0; i < bluePrintCategorySlotList.Count; i++)
        {
            if (i == targetNum)
            {
                bluePrintCategorySlotList[i].spriteChange.SetOnSprite();
            }
            else
            {
                bluePrintCategorySlotList[i].spriteChange.SetOffSprite();
            }
        }
    }

    private void OnClickWeaponButton(int targetNum)
    {
        if (currentSelectedWeaponIndex == targetNum) return;
        currentSelectedWeaponIndex = targetNum;

        for (int i = 0; i < bluePrintSlotList.Count; i++)
        {
            if (string.IsNullOrEmpty(bluePrintSlotList[i].key))
            {
                continue;
            }

            if (i == targetNum)
            {
                bluePrintSlotList[i].spriteChange.SetOnSprite();
            }
            else
            {
                bluePrintSlotList[i].spriteChange.SetOffSprite();
            }
        }

        var key = bluePrintSlotList[targetNum].key;
        var weapon = GameMgr.In.GetWeapon(currentSelectedWeaponCategoryKey, key);
        RefreshWeaponData(weapon);
    }

    private void RefreshWeaponData(WeaponDataTable.BluePrint weapon)
    {
        if (weapon.blueprintSprite != null)
        {
            blueprintImg.sprite = weapon.blueprintSprite;
            SetIndexBlueprintImgAspect(weapon.blueprintSprite);
        }
        weaponName.text = weapon.name;
        comment.text = weapon.comment.Replace("\\n", "\n");
        weaponCategory.text = GameMgr.In.GetWeaponCategory(currentSelectedWeaponCategoryKey).name;
        howToGet.text = weapon.howToGet;

        blueprintImgChanger.SetBlueprintImage(weapon);

        string result = string.Empty;
        var abilityList = GameMgr.In.abilityTable.abilityList;
        foreach (var ability in weapon.requiredChipAbilityList)
        {
            var abilityData = GameMgr.In.GetAbility(ability.abilityKey);
            if (abilityData != null)
            {
                result += abilityData.name + "+" + ability.count + "  ";
            }
        }
        essentialCondition.text = result;
        // specialGimmick.text = weapon.name;
    }

    private void SetEmptySlot(UISlot targetSlot)
    {
        targetSlot.key = string.Empty;
        targetSlot.innerImage.color = Vector4.zero;
        targetSlot.image.sprite = blankSlotSprite;
        targetSlot.button.onClick.RemoveAllListeners();
    }

    private void HideEmoji()
    {
        if (emojiRoutine != null)
        {
            StopCoroutine(emojiRoutine);
            emojiRoutine = null;
        }
        emoji.gameObject.SetActive(false);
    }

    private IEnumerator StartEventFlow(EventFlow targetEvent)
    {
        targetEvent.StartFlow();
        while (isEventFlowing)
        {
            yield return null;
        }
    }

    private IEnumerator ShowDrMadChat()
    {
        if (isShopTutorial)
        {
            isShopTutorial = false;
            yield break;
        }
        shopDrMadChat.SetActive(true);
        yield return new WaitForSeconds(2f);
        shopDrMadChat.SetActive(false);
    }

    public IEnumerator ShowEmoji()
    {
        var targetAudioName = "";
        switch (orderState)
        {
            case OrderState.Succeed:
                emoji.sprite = emojiSprites[0];
                targetAudioName = "success";
                break;
            case OrderState.Failed:
                emoji.sprite = emojiSprites[1];
                targetAudioName = "fail";
                break;
            case OrderState.Rejected:
                emoji.sprite = emojiSprites[2];
                targetAudioName = "reject";
                break;
        }
        SoundManager.PlayOneShot(targetAudioName);
        emoji.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        emoji.gameObject.SetActive(false);
    }

    public IEnumerator FadeToNextDay()
    {
        historyText.text = string.Empty;
        creditPanel.SetActive(false);
        yield return StartCoroutine(CommonTool.In.FadeOut());
        yield return StartCoroutine(CommonTool.In.FadeIn());
        isEventFlowing = false;
    }

    public void FameUIFill()
    {
        var renomMaskImage = renomMask?.GetComponent<Image>();
        if (renomMaskImage != null && GameMgr.In.maxFame != 0)
        {
            var a = (float)GameMgr.In.fame / GameMgr.In.maxFame;
            renomMaskImage.fillAmount = a;
        }
        else
        {
            Debug.LogWarning("명성치 오브젝트 누락 혹은 0 나누기 경고");
        }
    }

    public void TendUIMove()
    {
        var a = GameMgr.In.tendency;
        var positionModifier = (float)(a * 0.12f);
        var tendRect = tendPoint.GetComponent<RectTransform>();
        if (positionModifier > 120)
        {
            positionModifier = 120;
        }
        else if (positionModifier < -120)
        {
            positionModifier = 120;
        }
        tendRect.anchoredPosition = new Vector2(positionModifier, 0);
    }

    public void AdjustFame(int val)
    {
        GameMgr.In.fame += val;
        GameMgr.In.dayFame += val;

        if (GameMgr.In.fame < GameMgr.In.minFame)
        {
            GameMgr.In.fame = GameMgr.In.minFame;
        }
        else if (GameMgr.In.fame > GameMgr.In.maxFame)
        {
            GameMgr.In.fame = GameMgr.In.maxFame;
        }
    }

    public void AdjustTendency(int val)
    {
        GameMgr.In.tendency += val;
        GameMgr.In.dayTendency += val;

        if (GameMgr.In.tendency < GameMgr.In.minTend)
        {
            GameMgr.In.tendency = GameMgr.In.minTend;
        }
        else if (GameMgr.In.tendency > GameMgr.In.maxTend)
        {
            GameMgr.In.tendency = GameMgr.In.maxTend;
        }
    }

    private void UpdateDayEndMessage()
    {
        if (GameMgr.In.dayRevenue >= 100)
        {
            creditRevenueResult.text = "좋아, 오늘은 성공적이야 잘했어!";
        }
        else if (GameMgr.In.dayRevenue >= 1)
        {
            creditRevenueResult.text = "그렇게 나쁘진 않네! 힘내자!";
        }
        else
        {
            creditRevenueResult.text = "으음.. 더 노력해야겠는걸?";
        }
    }

}
