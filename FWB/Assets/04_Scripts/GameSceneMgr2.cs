using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Drawing;

public class GameSceneMgr2 : MonoBehaviour
{
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
    public List<Button> weaponSlotButtons = new List<Button>();
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
    public float textDelayTime;
    public List<IntroSceneMgr.ImageData> imageList = new List<IntroSceneMgr.ImageData>();
    public OrderState orderState = OrderState.None;

    private GameSceneMgr gameSceneMgr;
    private int page;
    private int fadeSpeed = 1;
    private List<EventTrigger> eventTriggerList = new List<EventTrigger>();
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private bool isWaitingForText;
    private ChatTarget chatTarget = ChatTarget.Main;
    private ChatTarget prevChatTarget = ChatTarget.None;
    private List<string> lines = new List<string>();
    private int lineCnt = 0;
    private int prevOrderTextNumber = -1;
    private int normalOrderLineIndex = 0;
    private int normalOrderPrevLineIndex = 0;
    private string prevText;
    private IEnumerator onEndText;
    private IEnumerator onSkip;
    private Coroutine textFlowCoroutine;
    private Text chatTargetText;
    private RectTransform popupChatPanelRect;
    private Point cursorPos = new Point();
    private bool visible;
    private bool isNormalOrdering;
    // private AudioSource audioSrc;

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

    IEnumerator Start()
    {
        gameSceneMgr = gamePanel.GetComponent<GameSceneMgr>();
        popupChatPanelRect = popupChatPanel.GetComponent<RectTransform>();

        pc.onClick.AddListener(OnClickPC);
        pcChatWork.onClick.AddListener(OnClickPCChatWork);
        mainBack.onClick.AddListener(OnClickDodgeMainPanel);
        weaponBook.onClick.AddListener(OnClickWeaponBook);
        weaponBack.onClick.AddListener(OnClickWeaponBack);
        weaponFront.onClick.AddListener(OnClickWeaponFront);
        dodgeMainPanel.onClick.AddListener(OnClickDodgeMainPanel);

        push.onClick.AddListener(OnClickPush);
        save.onClick.AddListener(OnClickSave);
        load.onClick.AddListener(OnClickSave);
        returnBtn.onClick.AddListener(OnClickReturn);
        gotoMain.onClick.AddListener(OnClickGoToMain);
        popupYes.onClick.AddListener(OnClickPopupYes);
        setting.onClick.AddListener(OnClickSetting);
        skip.onClick.AddListener(OnClickSkip);
        popupNo.onClick.AddListener(OnClickPopupNo);
        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.AddListener(OnClickSlot);
        }

        ok.onClick.AddListener(OnClickOK);
        yes.onClick.AddListener(OnClickYes);
        no.onClick.AddListener(OnClickNo);

        // audioSrc = GetComponent<AudioSource>();

        for (int i = 0; i < weaponSlotButtons.Count; i++)
        {
            weaponSlotButtons[i].onClick.AddListener(() =>
            {
                //weaponExplanation.text = weaponSlotTexts[tempNum].text + " Slot";
            });

            eventTriggerList.Add(weaponSlotButtons[i].gameObject.GetComponent<EventTrigger>());
        }

        for (int i = 0; i < eventTriggerList.Count; i++)
        {
            int tempNum = i;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { OnPointerEnterForButton(eventTriggerList[tempNum], tempNum); });
            eventTriggerList[i].triggers.Add(entry);
        }

        yield return StartCoroutine(CommonTool.In.FadeIn());
        StartText("Tutorial", EndTutorialRoutine());
        StartNextLine();
        onSkip = EndTutorialRoutine();
        mainChatPanel.SetActive(true);
    }

    public void OnClickPC()
    {
        // pcChatPanel.SetActive(true);
    }

    public void OnClickPCChatWork()
    {
        pcChatPanel.SetActive(false);
        mainPanel.SetActive(true);
        mainUI.SetActive(true);
        dodgeMainPanel.gameObject.SetActive(true);
    }

    public void OnClickWeaponBook()
    {
        mainUI.SetActive(false);
        weaponUI.SetActive(true);
        RefreshWeaponSlots();
    }

    public void OnClickWeaponBack()
    {
        if (page <= 0) return;

        page--;
        RefreshWeaponSlots();
    }

    public void OnClickWeaponFront()
    {
        page++;
        RefreshWeaponSlots();
    }

    public void OnClickDodgeMainPanel()
    {
        weaponUI.SetActive(false);
        mainUI.SetActive(false);
        mainPanel.SetActive(false);
        dodgeMainPanel.gameObject.SetActive(false);
    }

    public void OnClickPush()
    {
        if (save.gameObject.activeSelf)
        {
            stick.SetActive(false);
            save.gameObject.SetActive(false);
            load.gameObject.SetActive(false);
        }
        else
        {
            stick.SetActive(true);
            save.gameObject.SetActive(true);
            load.gameObject.SetActive(true);
        }
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
            StartCoroutine(onSkip);
            onSkip = null;
            if (currentLineIdex < lines.Count - 1)
            {
                lineCnt = currentLineIdex = lines.Count - 1;
                StartNextLine();
            }
            chatTargetText.text = lines[lines.Count - 1];
        }
    }

    private void FixedUpdate()
    {
        if (visible)
        {
            SetCursorPos(cursorPos.X, cursorPos.Y);
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
            StartCoroutine(onEndText);
            return;
        }

        StartNextLine();
    }

    public void OnClickOK()
    {
        foreach (var img in imageList)
        {
            img.imageObj.SetActive(false);
        }
        mainChatPanel.SetActive(false);
    }

    public void OnClickYes()
    {
        ActiveYesNoButton(false);
        StartText("Tutorial_Explain", EndExplainRoutine());
        StartNextLine();
        onSkip = EndExplainRoutine();
    }

    public void OnClickNo()
    {
        day.SetActive(true);
        tendency.SetActive(true);
        gold.SetActive(true);
        StartCoroutine(FadeInOutDateMessage());

        ActiveYesNoButton(false);
        StartText("Tutorial3", EndTutorial3Routine());
        StartNextLine();
        onSkip = SkipTutorial3Routine();
    }

    public void OnMakingDone()
    {
        mainChatText.text = string.Empty;
        gamePanel.SetActive(false);

        StartText("Tutorial7", EndTutorial7Routine());
        StartNextLine();
        onSkip = SkipTutorial7Routine();
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
            prevText = prevText + lines[currentLineIdex];
            chatTargetText.text = prevText;
            isTextFlowing = false;

            while (currentLineIdex >= lineCnt)
            {
                yield return new WaitForSeconds(textDelayTime);
            }
        }
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

    private void RefreshWeaponSlots()
    {
        for (int i = 1; i < weaponSlotTexts.Count + 1; i++)
        {
            weaponSlotTexts[i - 1].text = "slot " + (6 * page + i);
        }
    }

    private void OnPointerEnterForButton(EventTrigger et, int num)
    {
        weaponExplanation.text = weaponSlotTexts[num].text + " slot";
    }

    private void ActiveYesNoButton(bool isActive)
    {
        yes.gameObject.SetActive(isActive);
        no.gameObject.SetActive(isActive);
    }

    private void StartText(string textName, IEnumerator onEndText)
    {
        lineCnt = -1;
        lines = CommonTool.In.GetText(textName);
        isOnConversation = true;
        this.onEndText = onEndText;
        textFlowCoroutine = StartCoroutine(StartTextFlow());
    }

    private void EndText()
    {
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;
        chatTargetText.text = string.Empty;
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

    private void RefreshCreditPanel()
    {
        creditTitle.text = GameMgr.In.week + "주차\n" + GameMgr.In.day + "요일";
        creditTotalRevenue.text = GameMgr.In.credit + " C";
        creditMaterialCost.text = GameMgr.In.dayMaterialCost + " C";
        creditStore.text = GameMgr.In.dayStoreCost + " C";
        creditRentCost.text = GameMgr.In.dayRentCost + " C";
        creditRevenue.text = GameMgr.In.dayRevenue + " C";
    }

    private IEnumerator EndTutorialRoutine()
    {
        yield return null;
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;

        ActiveYesNoButton(true);
    }

    private IEnumerator EndExplainRoutine()
    {
        yield return null;
        EndText();

        OnClickNo();
    }

    private IEnumerator EndTutorial3Routine()
    {
        yield return null;
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;

        yesText.text = "Yes";
        noText.text = "No";
        yes.interactable = false;
        no.interactable = false;
        ActiveYesNoButton(true);
        CommonTool.In.SetFocus(new Vector2(1352, 602), new Vector2(155, 70));
        StartText("Tutorial4", EndTutorial4Routine());
        StartNextLine();
    }

    private IEnumerator SkipTutorial3Routine()
    {
        chatTarget = ChatTarget.Main;
        chatTargetText = mainChatText;
        popupChatPanel.SetActive(false);
        mainChatPanel.SetActive(true);
        pcChatPanel.SetActive(false);

        chatName.text = "모브NPC";
        yesText.text = "Yes";
        noText.text = "No";
        no.interactable = false;
        ActiveYesNoButton(true);
        CommonTool.In.SetFocus(new Vector2(1352, 602), new Vector2(155, 70));

        imageList.Find(x => x.key.Equals("모브NPC")).imageObj.SetActive(true);
        StartCoroutine(EndTutorial4Routine());
        yield return null;
    }

    private IEnumerator EndTutorial4Routine()
    {
        popupChatPanel.SetActive(false);
        yes.interactable = true;
        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() =>
        {
            EndText();

            no.interactable = true;
            ActiveYesNoButton(false);
            CommonTool.In.SetFocusOff();
            mainPanel.SetActive(true);
            weaponUI.SetActive(true);
            var pos = popupChatPanelRect.anchoredPosition;
            pos.x = 150;
            popupChatPanelRect.anchoredPosition = pos;
            popupChatPanel.SetActive(true);
            StartText("Tutorial5", EndTutorial5Routine());
            StartNextLine();
            onSkip = SkipTutorial5Routine();
        });
        yield return null;
    }

    private IEnumerator SkipTutorial4Routine()
    {
        yield return null;
    }

    private IEnumerator EndTutorial5Routine()
    {
        yield return null;
        EndText();
        popupChatPanel.SetActive(false);

        weaponSlotButtons[0].onClick.AddListener(() =>
        {
            mainPanel.SetActive(false);
            weaponUI.SetActive(false);
            gamePanel.SetActive(true);
            popupChatPanel.SetActive(true);
            StartText("Tutorial6", EndTutorial6Routine());
            StartNextLine();
            onSkip = SkipTutorial6Routine();
            gameSceneMgr.OnMakingDone += OnMakingDone;

            gameSceneMgr.StartPuzzle1();
        });
    }

    private IEnumerator SkipTutorial5Routine()
    {
        yield return null;
    }

    private IEnumerator EndTutorial6Routine()
    {
        yield return null;
        popupChatPanel.SetActive(false);
        EndText();
    }

    private IEnumerator SkipTutorial6Routine()
    {
        CommonTool.In.SetFocusOff();
        yield return null;
    }

    private IEnumerator EndTutorial7Routine()
    {
        yield return null;
        EndText();

        alertPanel.SetActive(true);
        alertDodge.onClick.RemoveAllListeners();
        alertDodge.onClick.AddListener(() =>
        {
            alertPanel.SetActive(false);
            StartText("Tutorial8", EndTutorial8Routine());
            StartNextLine();
        });
    }

    private IEnumerator SkipTutorial7Routine()
    {
        EndText();

        foreach (var image in imageList)
        {
            image.imageObj.SetActive(false);
        }
        GameMgr.In.credit = 100;
        GameMgr.In.dayRevenue = 100;
        goldText.text = GameMgr.In.credit.ToString();
        CommonTool.In.SetFocusOff();
        mainChatPanel.SetActive(false);
        alertPanel.SetActive(false);

        RefreshCreditPanel();
        creditPanel.SetActive(true);
        creditDodge.onClick.RemoveAllListeners();
        creditDodge.onClick.AddListener(() =>
        {
            creditPanel.SetActive(false);
            GameMgr.In.day = "화";
            dateText.text = GameMgr.In.day;
            prevChatTarget = ChatTarget.None;
            StartText("Day2_1", EndDay2_1Routine());
            StartNextLine();
            onSkip = SkipDay2_1Routine();
            creditDodge.onClick.RemoveAllListeners();
            GameMgr.In.ResetDayData();
            dateMessage.text = "1주차\n화요일";
            StartCoroutine(FadeInOutDateMessage());
        });
        pc.onClick.RemoveAllListeners();
        yield return null;
    }

    private IEnumerator EndTutorial8Routine()
    {
        yield return null;
        EndText();
        pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        pc.onClick.RemoveAllListeners();
        pc.onClick.AddListener(() =>
        {
            RefreshCreditPanel();
            creditPanel.SetActive(true);
            creditDodge.onClick.RemoveAllListeners();
            creditDodge.onClick.AddListener(() =>
            {
                creditPanel.SetActive(false);
                GameMgr.In.day = "화";
                dateText.text = GameMgr.In.day;
                StartText("Day2_1", EndDay2_1Routine());
                StartNextLine();
                onSkip = SkipDay2_1Routine();
                creditDodge.onClick.RemoveAllListeners();
                GameMgr.In.ResetDayData();
                dateMessage.text = "1주차\n화요일";
                StartCoroutine(FadeInOutDateMessage());
            });
            pc.onClick.RemoveAllListeners();
        });
    }

    private IEnumerator EndDay2_1Routine()
    {
        yield return null;
        StopCoroutine(textFlowCoroutine);
        lineCnt = -1;
        prevText = string.Empty;

        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            StartText("Day2_2", EndDay2_2Routine());
            onSkip = SkipDay2_4Routine();
            StartNextLine();
        });

        no.onClick.RemoveAllListeners();
        no.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            StartText("Day2_3", EndDay2_3Routine());
            onSkip = SkipDay2_4Routine();
            StartNextLine();
        });

        ActiveYesNoButton(true);
    }

    private IEnumerator SkipDay2_1Routine()
    {
        imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(true);

        mainChatPanel.SetActive(true);
        pcChatPanel.SetActive(false);
        chatTarget = ChatTarget.Main;
        chatTargetText = mainChatText;

        StartCoroutine(EndDay2_1Routine());
        yield return null;
    }

    private IEnumerator EndDay2_2Routine()
    {
        yield return null;
        EndText();

        renom.SetActive(true);

        StartText("Day2_4", EndDay2_4Routine());
        StartNextLine();
    }

    private IEnumerator EndDay2_3Routine()
    {
        yield return null;
        EndText();

        renom.SetActive(true);

        StartText("Day2_4", EndDay2_4Routine());
        StartNextLine();
    }

    private IEnumerator EndDay2_4Routine()
    {
        yield return null;
        EndText();

        prevChatTarget = ChatTarget.None;
        pcChatPanel.SetActive(false);

        StartCoroutine(StartNormalRoutine(5, EndDay2_5Routine()));
    }

    private IEnumerator SkipDay2_4Routine()
    {
        foreach (var image in imageList)
        {
            image.imageObj.SetActive(false);
        }

        mainChatPanel.SetActive(false);
        renom.SetActive(true);

        StartCoroutine(EndDay2_4Routine());
        yield return null;
    }

    private IEnumerator EndDay2_5Routine()
    {
        yield return null;
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
                creditPanel.SetActive(false);
                GameMgr.In.day = "수";
                dateText.text = GameMgr.In.day;
                prevChatTarget = ChatTarget.None;
                CommonTool.In.PlayOneShot("bird");
                StartCoroutine(StartNormalRoutine(5, EndDay3Routine()));
                creditDodge.onClick.RemoveAllListeners();
                GameMgr.In.ResetDayData();
                dateMessage.text = "1주차\n수요일";
                StartCoroutine(FadeInOutDateMessage());
            });
            pc.onClick.RemoveAllListeners();
        });
    }

    private IEnumerator EndDay3Routine()
    {
        yield return null;
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
                creditPanel.SetActive(false);
                GameMgr.In.day = "목";
                dateText.text = GameMgr.In.day;
                prevChatTarget = ChatTarget.None;
                CommonTool.In.PlayOneShot("bird");
                // StartCoroutine(StartNormalRoutine(5, EndDay3Routine()));
                creditDodge.onClick.RemoveAllListeners();
                GameMgr.In.ResetDayData();
                dateMessage.text = "1주차\n목요일";
                StartCoroutine(FadeInOutDateMessage());
            });
            pc.onClick.RemoveAllListeners();
        });
    }

    private IEnumerator FadeInOutDateMessage()
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

    private IEnumerator StartNormalRoutine(int customerCnt, IEnumerator onEndRoutine)
    {
        for (int i = 0; i < customerCnt; i++)
        {
            var orderTextList = new List<string>();
            var rejectTextList = new List<string>();
            var successTextList = new List<string>();
            var failTextList = new List<string>();

            int targetNumber = GetTargetNumber();
            string normalOrderTextName = "NormalOrder_" + targetNumber;
            lines = CommonTool.In.GetText(normalOrderTextName);

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
            this.onEndText = EndOrderText();
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
                    StartCoroutine(StartPuzzle2Process());
                    break;
                case OrderState.Rejected:
                    lineCnt = -1;
                    lines = rejectTextList;
                    isOnConversation = true;
                    this.onEndText = EndOrder();
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
                        this.onEndText = EndOrder();
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
        StartCoroutine(onEndRoutine);
    }

    private IEnumerator StartPuzzle2Process()
    {
        yield return null;
        EndText();

        mainPanel.SetActive(true);
        weaponUI.SetActive(true);

        weaponSlotButtons[0].onClick.RemoveAllListeners();
        weaponSlotButtons[0].onClick.AddListener(() =>
        {
            mainPanel.SetActive(false);
            weaponUI.SetActive(false);
            gamePanel.SetActive(true);
            gameSceneMgr.OnMakingDone += () =>
            {
                mainChatText.text = string.Empty;
                gamePanel.SetActive(false);
                // onSkip = SkipTutorial7Routine();
            };
            gameSceneMgr.StartPuzzle2();
        });
    }

    private IEnumerator EndOrderText()
    {
        yield return null;
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

    private IEnumerator EndOrder()
    {
        yield return null;
        EndText();

        orderState = OrderState.Finished;
    }
}
