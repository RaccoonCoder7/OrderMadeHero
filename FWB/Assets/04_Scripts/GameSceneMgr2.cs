using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSceneMgr2 : MonoBehaviour
{
    public Button pc;
    public Button pcChatWork;
    public Button mainBack;
    public Button weaponBook;
    public Button push;
    public Button save;
    public Button load;
    public Button city;
    public Button hero;
    public Button villain;
    public Button money;
    public Button weapon;
    public Button fame;
    public Button tendency;
    public Button ok;
    public Button yes;
    public Button no;
    public Button returnBtn;
    public Button gotoMain;
    public Button popupYes;
    public Button popupNo;
    public List<Button> saveLoadButtons = new List<Button>();
    public List<Button> weaponSlotButtons = new List<Button>();
    public List<Text> weaponSlotTexts = new List<Text>();
    public Button weaponBack;
    public Button weaponFront;
    public Button dodgeMainPanel;
    public Text chatName;
    public Text weaponExplanation;
    public GameObject mainChatPanel;
    public GameObject pcChatPanel;
    public GameObject mainPanel;
    public GameObject mainUI;
    public GameObject weaponUI;
    public GameObject stick;
    public GameObject saveLoadPanel;
    public GameObject saveLoadPopup;
    public Text mainChatText;
    public Text mascotChatText;
    public Text yesText;
    public Text noText;
    public float textDelayTime;
    public List<IntroSceneMgr.ImageData> imageList = new List<IntroSceneMgr.ImageData>();

    private int page;
    private List<EventTrigger> eventTriggerList = new List<EventTrigger>();
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private bool isMascotChat = false;
    private bool prevIsMascotChat = true;
    private List<string> lines = new List<string>();
    private int lineCnt = -1;
    private string prevText;
    private IEnumerator onEndText;
    private Coroutine textFlowCoroutine;
    private Text chatTargetText;
    // private AudioSource audioSrc;


    IEnumerator Start()
    {
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
        popupNo.onClick.AddListener(OnClickPopupNo);
        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.AddListener(OnClickSlot);
        }

        city.onClick.AddListener(OnClickCity);
        hero.onClick.AddListener(OnClickHero);
        villain.onClick.AddListener(OnClickVillain);
        money.onClick.AddListener(OnClickMoney);
        weapon.onClick.AddListener(OnClickWeapon);
        fame.onClick.AddListener(OnClickFame);
        tendency.onClick.AddListener(OnClickTendency);
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
        pcChatPanel.SetActive(true);
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

    public void OnClickPopupNo()
    {
        saveLoadPopup.SetActive(false);
    }

    public void OnClickChatBox()
    {
        if (!isOnConversation) return;

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

    public void OnClickCity()
    {
        lines = CommonTool.In.GetText("Tutorial_City");
        OnClickTutorialButton();
    }

    public void OnClickHero()
    {
        lines = CommonTool.In.GetText("Tutorial_Hero");
        OnClickTutorialButton();
    }

    public void OnClickVillain()
    {
        lines = CommonTool.In.GetText("Tutorial_Villain");
        OnClickTutorialButton();
    }

    public void OnClickMoney()
    {
        lines = CommonTool.In.GetText("Tutorial_Money");
        OnClickTutorialButton();
    }

    public void OnClickWeapon()
    {
        lines = CommonTool.In.GetText("Tutorial_Weapon");
        OnClickTutorialButton();
    }

    public void OnClickFame()
    {
        lines = CommonTool.In.GetText("Tutorial_Fame");
        OnClickTutorialButton();
    }

    public void OnClickTendency()
    {
        lines = CommonTool.In.GetText("Tutorial_Tendency");
        OnClickTutorialButton();
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
        ActiveYesNoButton(false);
        StartText("Tutorial2", EndTutorial2Routine());
        StartNextLine();
    }

    private void OnClickTutorialButton()
    {
        isOnConversation = true;
        TutorialButtonsSetActive(false);
        onEndText = EndTutorialRoutine();
        textFlowCoroutine = StartCoroutine(StartTextFlow());
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
                    if (com.Contains("!image"))
                    {
                        imageKeyList.Add(com.Split('_')[1]);
                    }
                    else if (com.Contains("!speaker"))
                    {
                        string speaker = com.Split('_')[1];
                        if (speaker.Equals("{username}"))
                        {
                            speaker = CommonTool.In.playerName;
                        }
                        // TODO: username이 마스코트 인 경우??
                        if (speaker.Equals("마스코트"))
                        {
                            isMascotChat = true;
                            speaker = CommonTool.In.mascotName;
                            continue;
                        }
                        else
                        {
                            isMascotChat = false;
                        }
                        chatName.text = speaker;
                    }
                    else if (com.Contains("!sound"))
                    {
                        string clipName = com.Split('_')[1];
                        CommonTool.In.PlayOneShot(clipName);
                    }
                    else if (com.Equals("!next"))
                    {
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

                lineCnt++;
                continue;
            }

            if (prevIsMascotChat != isMascotChat)
            {
                mainChatPanel.SetActive(!isMascotChat);
                pcChatPanel.SetActive(isMascotChat);
                chatTargetText = isMascotChat ? mascotChatText : mainChatText;
                prevIsMascotChat = isMascotChat;
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

    private void TutorialButtonsSetActive(bool isActive)
    {
        // city.gameObject.SetActive(isActive);
        // hero.gameObject.SetActive(isActive);
        // villain.gameObject.SetActive(isActive);
        // money.gameObject.SetActive(isActive);
        // weapon.gameObject.SetActive(isActive);
        // fame.gameObject.SetActive(isActive);
        // tendency.gameObject.SetActive(isActive);
        // ok.gameObject.SetActive(isActive);
        // yes.gameObject.SetActive(true);
        // no.gameObject.SetActive(true);
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

    private IEnumerator EndTutorial2Routine()
    {
        yield return null;
        EndText();

        // TODO: UI 표시 시작

        StartText("Tutorial3", EndTutorial3Routine());
        StartNextLine();
    }

    private IEnumerator EndTutorial3Routine()
    {
        yield return null;
        EndText();

        yesText.text = "Yes";
        noText.text = "No";
        no.interactable = false;
        yes.onClick.RemoveAllListeners();
        yes.onClick.AddListener(() => { mainChatPanel.gameObject.SetActive(false); });
        ActiveYesNoButton(true);
        // TODO: FOCUS
    }
}
