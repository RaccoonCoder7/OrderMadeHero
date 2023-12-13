using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Drawing;

/// <summary>
/// 게임 씬의 UI와 동작(메인 게임 플로우)를 관리
/// </summary>
public class GameSceneMgr : MonoBehaviour
{
    [Header("UI")]
    public Button pc;
    public Button pcChatWork;
    public Button mainBack;
    public Button weaponBook;
    public Button push;
    public Button save;
    public Button load;
    public Button ok;
    public Button yes;
    public Button no;
    public Button returnBtn;
    public Button gotoMain;
    public Button popupYes;
    public Button popupNo;
    public Button setting;
    public Button skip;
    public List<Button> saveLoadButtons = new List<Button>();
    public List<BluePrintSlot> bluePrintSlot = new List<BluePrintSlot>();
    public List<Text> weaponSlotTexts = new List<Text>();
    public Button weaponBack;
    public Button weaponFront;
    public Button dodgeMainPanel;
    public Button alertDodge;
    public Button creditDodge;
    public Text chatName;
    public Text weaponExplanation;
    public GameObject mainChatPanel;
    public GameObject pcChatPanel;
    public GameObject popupChatPanel;
    public GameObject mainPanel;
    public GameObject mainUI;
    public GameObject weaponUI;
    public GameObject stick;
    public GameObject saveLoadPanel;
    public GameObject saveLoadPopup;
    public GameObject day;
    public GameObject renom;
    public GameObject tendency;
    public GameObject gold;
    public GameObject gamePanel;
    public GameObject cursor;
    public GameObject alertPanel;
    public GameObject creditPanel;
    public Text mainChatText;
    public Text mascotChatText;
    public Text popupChatText;
    public Text yesText;
    public Text noText;
    public Text dateText;
    public Text dateMessage;
    public Text goldText;
    public Text creditTitle;
    public Text creditTotalRevenue;
    public Text creditMaterialCost;
    public Text creditStore;
    public Text creditRentCost;
    public Text creditRevenue;
    public List<IntroSceneMgr.ImageData> imageList = new List<IntroSceneMgr.ImageData>();
    [HideInInspector]
    public Text chatTargetText;
    [HideInInspector]
    public RectTransform popupChatPanelRect;
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
    public PuzzleMgr puzzleMgr;

    private int page;
    private int fadeSpeed = 1;
    private List<EventTrigger> eventTriggerList = new List<EventTrigger>();
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private bool isWaitingForText;
    private List<string> lines = new List<string>();
    private int lineCnt = 0;
    private int prevOrderTextNumber = -1;
    private int normalOrderLineIndex = 0;
    private int normalOrderPrevLineIndex = 0;
    private string prevText;
    private Action onEndText;
    private Action onSkip;
    private Coroutine textFlowCoroutine;
    private Point cursorPos = new Point();
    private bool visible;
    private List<EventFlow> eventFlowList = new List<EventFlow>();

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);

    public enum ChatTarget
    {
        None,
        Main,
        Mascot,
        Popup
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


    private void FixedUpdate()
    {
        if (visible)
        {
            SetCursorPos(cursorPos.X, cursorPos.Y);
        }
    }

    private IEnumerator Start()
    {
        puzzleMgr = gamePanel.GetComponent<PuzzleMgr>();
        popupChatPanelRect = popupChatPanel.GetComponent<RectTransform>();

        //TODO: 추후 기능개발시 참고
        // pc.onClick.AddListener(OnClickPC);
        // pcChatWork.onClick.AddListener(OnClickPCChatWork);
        // weaponBook.onClick.AddListener(OnClickWeaponBook);
        // weaponBack.onClick.AddListener(OnClickWeaponBack);
        // weaponFront.onClick.AddListener(OnClickWeaponFront);
        // ok.onClick.AddListener(OnClickOK);
        mainBack.onClick.AddListener(OnClickDodgeMainPanel);
        dodgeMainPanel.onClick.AddListener(OnClickDodgeMainPanel);
        save.onClick.AddListener(OnClickSave);
        load.onClick.AddListener(OnClickSave);
        returnBtn.onClick.AddListener(OnClickReturn);
        gotoMain.onClick.AddListener(OnClickGoToMain);
        popupYes.onClick.AddListener(OnClickPopupYes);
        popupNo.onClick.AddListener(OnClickPopupNo);
        setting.onClick.AddListener(OnClickSetting);
        skip.onClick.AddListener(OnClickSkip);

        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.AddListener(OnClickSlot);
        }

        eventFlowList = GetComponents<EventFlow>().ToList();
        foreach (var eventflow in eventFlowList)
        {
            eventflow.mgr = this;
        }

        yield return StartCoroutine(CommonTool.In.FadeIn());

        // TODO: day limit 추가
        for (int i = 1; i <= 5; i++)
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

            if (i < 5)
            {
                NextDay();
            }
        }
    }

    //TODO: 추후 기능개발시 참고
    // public void OnClickPCChatWork()
    // {
    //     pcChatPanel.SetActive(false);
    //     mainPanel.SetActive(true);
    //     mainUI.SetActive(true);
    //     dodgeMainPanel.gameObject.SetActive(true);
    // }

    // public void OnClickWeaponBook()
    // {
    //     mainUI.SetActive(false);
    //     weaponUI.SetActive(true);
    //     RefreshWeaponSlots();
    // }

    // public void OnClickWeaponBack()
    // {
    //     if (page <= 0) return;

    //     page--;
    //     RefreshWeaponSlots();
    // }

    // public void OnClickWeaponFront()
    // {
    //     page++;
    //     RefreshWeaponSlots();
    // }

    // public void OnClickOK()
    // {
    //     foreach (var img in imageList)
    //     {
    //         img.imageObj.SetActive(false);
    //     }
    //     mainChatPanel.SetActive(false);
    // }

    public void OnClickDodgeMainPanel()
    {
        weaponUI.SetActive(false);
        mainUI.SetActive(false);
        mainPanel.SetActive(false);
        dodgeMainPanel.gameObject.SetActive(false);
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
        GetCursorPos(out cursorPos);
        Cursor.visible = visible;
        visible = !visible;
        cursor.SetActive(visible);
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
            SkipCurrLine();
            StartNextLine();
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

    public void ActiveYesNoButton(bool isActive)
    {
        yes.gameObject.SetActive(isActive);
        no.gameObject.SetActive(isActive);
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
        if (clearText)
        {
            chatTargetText.text = string.Empty;
        }
    }

    public void RefreshCreditPanel()
    {
        creditTitle.text = GameMgr.In.week + "주차\n" + GameMgr.In.day + "요일";
        creditTotalRevenue.text = GameMgr.In.credit + " C";
        creditMaterialCost.text = GameMgr.In.dayMaterialCost + " C";
        creditStore.text = GameMgr.In.dayStoreCost + " C";
        creditRentCost.text = GameMgr.In.dayRentCost + " C";
        creditRevenue.text = GameMgr.In.dayRevenue + " C";
    }

    public void SetBlueprintButton()
    {
        for (int i = 0; i < bluePrintSlot.Count; i++)
        {
            int tempNum = i;
            bluePrintSlot[tempNum].button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                mainPanel.SetActive(false);
                weaponUI.SetActive(false);
                gamePanel.SetActive(true);
                puzzleMgr.OnMakingDone += () =>
                {
                    mainChatText.text = string.Empty;
                    gamePanel.SetActive(false);
                };
                var key = bluePrintSlot[tempNum].bluePrintKey;
                GameMgr.In.currentBluePrint = GameMgr.In.bluePrintTable.bluePrintList.Find((Predicate<BluePrintTable.BluePrint>)(x => x.bluePrintKey.Equals(key)));
                puzzleMgr.StartPuzzle();
            }));
        }
    }

    public void EndNormalOrderRoutine()
    {
        EndText();
        isNormalOrdering = false;

        pc.onClick.RemoveAllListeners();
        pc.onClick.AddListener(() =>
        {
            RefreshCreditPanel();
            creditPanel.SetActive(true);
            creditDodge.onClick.RemoveAllListeners();
            creditDodge.onClick.AddListener(() =>
            {
                isEventFlowing = false;
            });
            pc.onClick.RemoveAllListeners();
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
            chatTargetText.text = lines[lines.Count - 1];
        }
    }

    //TODO: 추후 기능개발시 참고
    // private void RefreshWeaponSlots()
    // {
    //     for (int i = 1; i < weaponSlotTexts.Count + 1; i++)
    //     {
    //         weaponSlotTexts[i - 1].text = "slot " + (6 * page + i);
    //     }
    // }

    // private void OnPointerEnterForButton(EventTrigger et, int num)
    // {
    //     weaponExplanation.text = weaponSlotTexts[num].text + " slot";
    // }

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

    private int GetTargetNumber()
    {
        // TODO: 아래 최대범위 숫자를 주문타입 갯수로 바꾸기
        int value = UnityEngine.Random.Range(0, 19);
        if (value == prevOrderTextNumber)
        {
            value = GetTargetNumber();
        }
        prevOrderTextNumber = value;
        return value;
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
        creditPanel.SetActive(false);
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

    // private void EndDay3Routine()
    // {
    //     EndText();
    //     isNormalOrdering = false;

    //     pc.onClick.RemoveAllListeners();
    //     pc.onClick.AddListener(() =>
    //     {
    //         RefreshCreditPanel();
    //         creditPanel.SetActive(true);
    //         creditDodge.onClick.RemoveAllListeners();
    //         creditDodge.onClick.AddListener(() =>
    //         {
    //             creditPanel.SetActive(false);
    //             GameMgr.In.day = "목";
    //             dateText.text = GameMgr.In.day;
    //             prevChatTarget = ChatTarget.None;
    //             CommonTool.In.PlayOneShot("bird");
    //             // StartCoroutine(StartNormalRoutine(5, EndDay3Routine()));
    //             creditDodge.onClick.RemoveAllListeners();
    //             GameMgr.In.ResetDayData();
    //             dateMessage.text = "1주차\n목요일";
    //             StartCoroutine(FadeInOutDateMessage());
    //         });
    //         pc.onClick.RemoveAllListeners();
    //     });
    // }

    public IEnumerator FadeInOutDateMessage()
    {
        float fadeValue = 0;
        float actualSpeed = fadeSpeed * 0.01f;
        while (fadeValue < 1)
        {
            fadeValue += actualSpeed;
            dateMessage.color = new UnityEngine.Color(0, 0, 0, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
        yield return new WaitForSeconds(2);
        while (fadeValue > 0)
        {
            fadeValue -= actualSpeed;
            dateMessage.color = new UnityEngine.Color(0, 0, 0, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
    }

    public IEnumerator StartNormalRoutine(int customerCnt, Action onEndRoutine)
    {
        for (int i = 0; i < customerCnt; i++)
        {
            var orderTextList = new List<string>();
            var rejectTextList = new List<string>();
            var successTextList = new List<string>();
            var failTextList = new List<string>();

            int targetNumber = GetTargetNumber();
            string orderKey = "NormalOrder_" + targetNumber;
            var order = GameMgr.In.orderTable.GetNewOrder(orderKey);
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
                    StartPuzzleProcess();
                    break;
                case OrderState.Rejected:
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
                    foreach (var image in imageList)
                    {
                        image.imageObj.SetActive(false);
                    }
                    mainChatPanel.SetActive(false);
                    chatTarget = ChatTarget.None;
                    processDone = true;
                }
                yield return null;
            }
        }
        onEndRoutine.Invoke();
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
                        if (speaker.Trim().Equals("popup") && splittedCom.Length == 3)
                        {
                            chatTarget = ChatTarget.Popup;
                            // TODO: speaker에 따라서 이미지 변경
                        }
                        else if (speaker.Equals(CommonTool.In.mascotName))
                        {
                            chatTarget = ChatTarget.Mascot;
                        }
                        else
                        {
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
                        chatTargetText = mainChatText;
                        break;
                    case ChatTarget.Mascot:
                        mainChatPanel.SetActive(false);
                        pcChatPanel.SetActive(true);
                        popupChatPanel.SetActive(false);
                        chatTargetText = mascotChatText;
                        break;
                    case ChatTarget.Popup:
                        popupChatPanel.SetActive(true);
                        chatTargetText = popupChatText;
                        break;
                }
                prevChatTarget = chatTarget;
            }

            if (!string.IsNullOrEmpty(prevText))
            {
                prevText = prevText + "\n";
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
            prevText = prevText + lines[currentLineIdex];
            chatTargetText.text = prevText;
            isTextFlowing = false;

            while (currentLineIdex >= lineCnt)
            {
                yield return new WaitForSeconds(textDelayTime);
            }
        }
    }

    private void StartPuzzleProcess()
    {
        EndText();

        mainPanel.SetActive(true);
        weaponUI.SetActive(true);

        bluePrintSlot[0].button.onClick.RemoveAllListeners();
        bluePrintSlot[0].button.onClick.AddListener(() =>
        {
            mainPanel.SetActive(false);
            weaponUI.SetActive(false);
            gamePanel.SetActive(true);
            puzzleMgr.OnMakingDone += () =>
            {
                mainChatText.text = string.Empty;
                gamePanel.SetActive(false);
            };
            puzzleMgr.StartPuzzle();
        });
    }

    private IEnumerator StartEventFlow(EventFlow targetEvent)
    {
        targetEvent.StartFlow();
        while (isEventFlowing)
        {
            yield return null;
        }
    }
    
}
