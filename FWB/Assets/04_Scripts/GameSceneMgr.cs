using System.Text;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Drawing;
using static SpriteChange;
using DG.Tweening;
using UnityEngine.Serialization;
using static WeaponDataTable;
using static GameMgr;

/// <summary>
/// 게임 씬의 UI와 동작(메인 게임 플로우)를 관리
/// </summary>
public class GameSceneMgr : MonoBehaviour
{
    [Header("UI")]
    public Button pc;
    public Button popupDodge;
    public Button save;
    public Button load;
    public Button ok;
    public Button yes;
    public Button no;
    public Button eventBtn1;
    public Button eventBtn2;
    public Button returnBtn;
    public Button gotoMain;
    public Button popupYes;
    public Button popupNo;
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
    public List<Button> saveLoadButtons = new List<Button>();
    public Button alertDodge;
    public Button creditDodge;
    public Text chatName;
    public GameObject mainChatPanel;
    public GameObject pcChatPanel;
    public GameObject popupChatPanel;
    public GameObject popupPanel;
    public GameObject fullPanel;
    public GameObject saveLoadPanel;
    public GameObject saveLoadPopup;
    public GameObject day;
    public GameObject renom;
    public GameObject renomMask;
    public GameObject tendency;
    public GameObject tendPoint;
    public GameObject gold;
    public GameObject gamePanel;
    // public GameObject cursor;
    public GameObject alertPanel;
    private Image alertPanelImg;
    public Sprite chipsetAlertImg;
    public GameObject getItemImg;
    public GameObject getItemText;
    public List<Sprite> getItemSprites = new List<Sprite>();
    public List<String> getItemTexts = new List<String>();
    public GameObject creditPanel;
    public GameObject shopUiSlotNoItemPrefab;
    public GameObject shopDrMadChat;
    public GameObject shopControlBlockingPanel;
    public GameObject deskNavi;
    public GameObject shopPopupPanel;
    public ShopUISlot shopUiSlotPrefab;
    public ShopUISlot shopUiSlotSoldOutPrefab;
    public UISlot uiSlotPrefab;
    public ShopFollowUI shopFollowUI;
    public ShopPopupUI shopPopupUI;
    public GameObject mobNpc;
    public Text mainChatText;
    public Text mascotChatText;
    public Text popupChatText;
    public Text fullChatName;
    public Text fullChatText;
    public Text yesText;
    public Text noText;
    public Text eventBtntext1;
    public Text eventBtntext2;
    public Text dateText;
    public Text dateMessage;
    public Text goldText;
    public Text creditTitle;
    public Text creditRevenue;
    public Text creditBonusRevenue;
    public Text creditChipsetCost;
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
    public Image blueprintImg;
    public List<Sprite> bgImgList = new List<Sprite>();
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
    public List<Sprite> mobSpriteList;
    public SpriteAnimation shopSpriteAnim;
    [HideInInspector]
    public Text chatTargetText;
    [Header("Data")]
    public float textDelayTime;
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

    private int page;
    private int fadeSpeed = 1;
    private List<EventTrigger> eventTriggerList = new List<EventTrigger>();
    [SerializeField]
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private bool isWaitingForText;
    public bool autoTextSkip;
    private List<string> lines = new List<string>();
    private int lineCnt = 0;
    private string prevOrderKey = "";
    private int normalOrderLineIndex = 0;
    private int normalOrderPrevLineIndex = 0;
    private int currentSelectedWeaponIndex = -1;
    private float textSkipWaitTime = 1f;
    private string prevText;
    private string currentSelectedWeaponCategoryKey;
    private string currentOrderText;
    private Action onEndText;
    private Action onSkip;
    private Coroutine textFlowCoroutine;
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
    private int previousMobAvatarIndex = -1;

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
        Full
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
        StartCoroutine(CommonTool.In.BGMPlayer());

        blueprintImgRectTr = blueprintImg.GetComponent<RectTransform>();
        puzzleMgr = gamePanel.GetComponent<PuzzleMgr>();
        shopBlueprintTabImg = (Image)shopBlueprintTab.targetGraphic;
        shopChipsetTabImg = (Image)shopChipsetTab.targetGraphic;
        CommonTool.In.shopFollowUI = shopFollowUI;
        alertPanelImg = alertPanel.GetComponentInChildren<Image>();
        indexSC = index.GetComponent<SpriteChange>();

        popupDodge.onClick.AddListener(OnClickDodgePopup);
        weaponLeft.onClick.AddListener(OnClickWeaponLeft);
        weaponRight.onClick.AddListener(OnClickWeaponRight);
        weaponCreate.onClick.AddListener(OnClickWeaponCreate);
        // save.onClick.AddListener(OnClickSave);
        // load.onClick.AddListener(OnClickSave);
        returnBtn.onClick.AddListener(OnClickReturn);
        gotoMain.onClick.AddListener(OnClickGoToMain);
        popupYes.onClick.AddListener(OnClickPopupYes);
        popupNo.onClick.AddListener(OnClickPopupNo);
        setting.onClick.AddListener(OnClickSetting);
        skip.onClick.AddListener(OnClickSkip);
        shopBlueprintTab.onClick.AddListener(OnClickShopBlueprintTab);
        shopChipsetTab.onClick.AddListener(OnClickShopChipsetTab);

        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.AddListener(OnClickSlot);
        }

        eventFlowList = GetComponents<EventFlow>().ToList();
        foreach (var eventflow in eventFlowList)
        {
            eventflow.mgr = this;
        }

        RefreshWeaponCategoryButtons();

        yield return StartCoroutine(CommonTool.In.FadeIn());

        // TODO: day limit 추가
        for (int i = startDay; i <= 7; i++)
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

            if (isEventFlowing)
            {
                yield return null;
            }

            if (i < 7)
            {
                NextDay();
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
        if (currentSelectedWeaponIndex <= 0)
        {
            return;
        }

        bluePrintSlotList[currentSelectedWeaponIndex - 1].button.onClick.Invoke();
    }

    public void OnClickWeaponRight()
    {
        if (currentSelectedWeaponIndex + 1 >= bluePrintSlotList.Count
         || string.IsNullOrEmpty(bluePrintSlotList[currentSelectedWeaponIndex + 1].key))
        {
            return;
        }

        bluePrintSlotList[currentSelectedWeaponIndex + 1].button.onClick.Invoke();
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
        if (mobSpriteList.Count > 0)
        {
            int randomIndex = 0;
            do
            {
                randomIndex = UnityEngine.Random.Range(0, mobSpriteList.Count);
            } while (previousMobAvatarIndex == randomIndex);
            previousMobAvatarIndex = randomIndex;
            Sprite randomSprite = mobSpriteList[randomIndex];

            var image = mobNpc.GetComponent<Image>();
            image.sprite = randomSprite;

            RectTransform rectTransform = image.rectTransform;
            float fixedHeight = (randomIndex >= 0 && randomIndex <= 2) ? 410.0f : 520.0f;
            rectTransform.anchoredPosition = (randomIndex >= 0 && randomIndex <= 2) ? new Vector2(-675, -135) : new Vector2(-630, -80);
            float spriteRatio = randomSprite.rect.width / randomSprite.rect.height;
            rectTransform.sizeDelta = new Vector2(fixedHeight * spriteRatio, fixedHeight);
        }
        else
        {
            Debug.LogWarning("모브 스프라이트 없음.");
        }
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
        saveLoadPanel.SetActive(true);
    }

    public void OnClickReturn()
    {
        saveLoadPanel.SetActive(false);
    }

    public void OnClickGoToMain()
    {
        StartCoroutine(CommonTool.In.AsyncChangeScene("StartScene"));
    }

    public void OnClickSlot()
    {
        saveLoadPopup.SetActive(true);
    }

    public void OnClickPopupYes()
    {
        saveLoadPopup.SetActive(false);
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
        saveLoadPopup.SetActive(false);
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
        RefreshShopUI();
    }

    public void OnClickShopChipsetTab()
    {
        if (currentShopTab == ShopTab.Chipset) return;
        shopChipsetTabImg.sprite = shopTabSpriteList[3];
        shopBlueprintTabImg.sprite = shopTabSpriteList[0];
        currentShopTab = ShopTab.Chipset;
        RefreshShopUI();
    }

    public void OnClickShopDodge()
    {
        if (isShopAnimating) return;
        isShopAnimating = true;
        StartCoroutine(StartShopOutAnim());
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
        creditChipsetCost.text = "칩셋구입 " + GameMgr.In.daySpendCredit;
        if (GameMgr.In.day == Day.금)
        {
            GameMgr.In.dayRentCost = -100;
            GameMgr.In.credit += GameMgr.In.dayRentCost;
            goldText.text = GameMgr.In.credit.ToString();
        }
        else
        {
            GameMgr.In.dayRentCost = 0;
        }
        creditRentCost.text = "임대료 " + GameMgr.In.dayRentCost;
        var totalRevenue = GameMgr.In.dayRevenue + GameMgr.In.dayBonusRevenue + GameMgr.In.dayRentCost + GameMgr.In.daySpendCredit;
        creditTotalRevenue.text = totalRevenue + " 크레딧";
        creditCustomerCnt.text = GameMgr.In.dayCustomerCnt.ToString();
        creditRenom.text = GameMgr.In.dayFame.ToString();
        creditTendency.text = GameMgr.In.dayTendency.ToString();
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
        EndText();
        isNormalOrdering = false;

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
                StartCoroutine(FadeToNextDay());
            });
            pc.onClick.RemoveAllListeners();
            UpdateDayEndMessage();
            FameUIFill();
            TendUIMove();
        });
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

            for (int i = 0; i < 10; i++)
            {
                if (i < listCnt)
                {
                    var state = PlayerPrefs.GetInt(bluePrintList[i].bluePrintKey);
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
                                    GameMgr.In.daySpendCredit -= item.price;
                                    goldText.text = GameMgr.In.credit.ToString();
                                    PlayerPrefs.SetInt(bluePrintList[tempNum].bluePrintKey, 3);
                                    StartCoroutine(DrMadChatRoutine());
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
            for (int i = 0; i < 10; i++)
            {
                if (i < listCnt)
                {
                    var state = PlayerPrefs.GetInt(chipList[i].chipKey);
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
                                    GameMgr.In.daySpendCredit -= item.price;
                                    goldText.text = GameMgr.In.credit.ToString();
                                    PlayerPrefs.SetInt(chipList[tempNum].chipKey, 3);
                                    StartCoroutine(DrMadChatRoutine());
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
    }

    public void SetShopButtonListener()
    {
        shop.onClick.AddListener(OnClickShop);
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

        orderState = OrderState.Finished;
    }

    private void NextDay()
    {
        GameMgr.In.ResetDayData();
        GameMgr.In.SetNextDayData();
        string day = GameMgr.In.day.ToString();
        dateText.text = day;
        dateMessage.text = GameMgr.In.week + "주차\n" + day + "요일";
        prevChatTarget = ChatTarget.None;
        CommonTool.In.PlayOneShot("bird");
        creditDodge.onClick.RemoveAllListeners();
        StartCoroutine(FadeInOutDateMessage());
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
        yield return new WaitForSeconds(2);
        while (fadeValue > 0)
        {
            fadeValue -= actualSpeed;
            dateMessage.color = new UnityEngine.Color(1, 1, 1, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
    }

    public IEnumerator StartNormalRoutine(int customerCnt, Action onEndRoutine)
    {
        GameMgr.In.dayCustomerCnt = customerCnt;
        for (int i = 0; i < customerCnt; i++)
        {
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
                    TendUIMove();
                    FameUIFill();
                    lineCnt = -1;
                    lines = rejectTextList;
                    isOnConversation = true;
                    this.onEndText = EndOrder;
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
                            if (o.orderCondition.Equals("AfterOneOrder"))
                            {
                                o.orderEnable = true;
                            }
                        }

                        GameMgr.In.SaveOrderHistory();

                        isOnConversation = true;
                        lineCnt = -1;
                        lines = orderState == OrderState.Succeed ? successTextList : failTextList;
                        this.onEndText = EndOrder;
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
            if (order.orderCondition.Equals("AfterOneOrder"))
            {
                order.orderEnable = false;
            }
        }
        
        GameMgr.In.orderedBluePrintKeyList.Clear();

        onEndRoutine.Invoke();
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
                    }
                    else if (com.StartsWith("!sound"))
                    {
                        string clipName = com.Split('_')[1];
                        CommonTool.In.PlayOneShot(clipName);
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
                            CommonTool.In.SetFocus(pos, size);
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
                        //historyText.text += "\n";
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
                }
                prevChatTarget = chatTarget;
            }

            if (!string.IsNullOrEmpty(prevText))
            {
                prevText = prevText.Replace(" ▼", string.Empty) + "\n";
            }

            isTextFlowing = true;
            //historyText.text += lines[i] + "\n";
            for (int j = 0; j < lines[currentLineIdex].Length; j++)
            {
                chatTargetText.text = prevText + lines[currentLineIdex].Substring(0, j + 1);
                yield return new WaitForSeconds(textDelayTime);

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
                break;
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
        comment.text = weapon.comment;
        weaponCategory.text = GameMgr.In.GetWeaponCategory(currentSelectedWeaponCategoryKey).name;
        howToGet.text = weapon.howToGet;

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

    private IEnumerator StartEventFlow(EventFlow targetEvent)
    {
        targetEvent.StartFlow();
        while (isEventFlowing)
        {
            yield return null;
        }
    }

    private IEnumerator DrMadChatRoutine()
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

    public IEnumerator FadeToNextDay()
    {
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
