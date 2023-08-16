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
    public float textDelayTime;
    public List<IntroSceneMgr.ImageData> imageList = new List<IntroSceneMgr.ImageData>();

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
    private string prevText;
    private IEnumerator onEndText;
    private Coroutine textFlowCoroutine;
    private Text chatTargetText;
    private RectTransform popupChatPanelRect;
    private Point cursorPos = new Point();
    private bool visible;
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

    IEnumerator Start()
    {
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
            // TODO: 애니메이션
            stick.SetActive(false);
            save.gameObject.SetActive(false);
            load.gameObject.SetActive(false);
        }
        else
        {
            // TODO: 애니메이션
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
    }

    public void OnMakingDone()
    {
        mainChatText.text = string.Empty;
        gamePanel.SetActive(false);

        StartText("Tutorial7", EndTutorial7Routine());
        StartNextLine();
    }

    private IEnumerator StartTextFlow()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = ReplaceKeyword(lines[i]);
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("!"))
            {
                var commands = lines[i].Split(' ');
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
                            goldText.text = GameMgr.In.credit + " G";
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

                while (i >= lineCnt)
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
            for (int j = 0; j < lines[i].Length; j++)
            {
                chatTargetText.text = prevText + lines[i].Substring(0, j + 1);
                yield return new WaitForSeconds(textDelayTime);

                if (skipLine)
                {
                    skipLine = false;
                    if (i + 1 >= lines.Count || lines[i + 1].Contains("!next"))
                    {
                        lineCnt--;
                    }
                    break;
                }
            }
            prevText = prevText + lines[i];
            chatTargetText.text = prevText;
            isTextFlowing = false;

            while (i >= lineCnt)
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

    private IEnumerator EndTutorialRoutine()
    {
        yield return null;
        EndText();

        ActiveYesNoButton(true);
    }

    private IEnumerator EndExplainRoutine()
    {
        yield return null;
        EndText();

        OnClickNo();
    }

    // private IEnumerator EndTutorial2Routine()
    // {
    //     yield return null;
    //     EndText();

    //     // TODO: UI 표시 시작

    //     StartText("Tutorial3", EndTutorial3Routine());
    //     StartNextLine();
    // }

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
        CommonTool.In.SetFocus(new Vector2(1400, 500), new Vector2(180, 100));

        StartText("Tutorial4", EndTutorial4Routine());
        StartNextLine();
    }

    private IEnumerator EndTutorial4Routine()
    {
        yield return null;

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
        });
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
        });
    }

    private IEnumerator EndTutorial6Routine()
    {
        yield return null;
        popupChatPanel.SetActive(false);
        EndText();
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

    private IEnumerator EndTutorial8Routine()
    {
        yield return null;
        EndText();
        pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        pc.onClick.RemoveAllListeners();
        pc.onClick.AddListener(() =>
        {
            creditPanel.SetActive(true);
            creditDodge.onClick.RemoveAllListeners();
            creditDodge.onClick.AddListener(() =>
            {
                creditPanel.SetActive(false);
                dateText.text = "1주차\n화요일";
                prevChatTarget = ChatTarget.None;
                StartText("Day2_1", EndDay2_1Routine());
                StartNextLine();
                creditDodge.onClick.RemoveAllListeners();
            });
            pc.onClick.RemoveAllListeners();
        });
    }

    private IEnumerator EndDay2_1Routine()
    {
        yield return null;
        EndText();

        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            StartText("Day2_2", EndDay2_2Routine());
            StartNextLine();
        });

        no.onClick.RemoveAllListeners();
        no.onClick.AddListener(() =>
        {
            ActiveYesNoButton(false);
            StartText("Day2_3", EndDay2_3Routine());
            StartNextLine();
        });

        ActiveYesNoButton(true);
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

        // TODO
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
}
