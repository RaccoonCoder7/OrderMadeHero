using UnityEngine;

/// <summary>
/// day1의 이벤트를 제어
/// </summary>
public class EventFlowDay1 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Tutorial", EndTutorialRoutine, SkipTutorialRoutine);
    }

    private void EndTutorialRoutine()
    {
        mgr.EndText(false);

        mgr.yes.onClick.RemoveAllListeners();
        mgr.yes.onClick.AddListener(() =>
        {
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Tutorial_Explain", EndExplainRoutine, EndExplainRoutine);
        });

        mgr.no.onClick.RemoveAllListeners();
        mgr.no.onClick.AddListener(() =>
        {
            mgr.day.SetActive(true);
            mgr.tendency.SetActive(true);
            mgr.gold.SetActive(true);
            StartCoroutine(mgr.FadeInOutDateMessage());

            mgr.ActiveYesNoButton(false);
            mgr.StartText("Tutorial3", EndTutorial3Routine, SkipTutorial3Routine);
        });

        mgr.ActiveYesNoButton(true);
    }

    private void SkipTutorialRoutine()
    {
        mgr.SkipToLastLine();
        EndTutorialRoutine();
    }

    private void EndExplainRoutine()
    {
        mgr.EndText();
        mgr.no.onClick.Invoke();
    }

    private void EndTutorial3Routine()
    {
        mgr.EndText(false);
        mgr.yesText.text = "Yes";
        mgr.noText.text = "No";
        mgr.yes.interactable = false;
        mgr.no.interactable = false;
        mgr.ActiveYesNoButton(true);
        CommonTool.In.SetFocus(new Vector2(1352, 555), new Vector2(155, 70));

        mgr.StartText("Tutorial4", EndTutorial4Routine, EndTutorial4Routine);
    }

    private void SkipTutorial3Routine()
    {
        mgr.chatTarget = GameSceneMgr.ChatTarget.Main;
        mgr.chatTargetText = mgr.mainChatText;
        mgr.popupChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatName.text = "모브NPC";
        mgr.yesText.text = "Yes";
        mgr.noText.text = "No";
        mgr.no.interactable = false;
        mgr.yes.interactable = false;
        mgr.ActiveYesNoButton(true);
        mgr.imageList.Find(x => x.key.Equals("모브NPC")).imageObj.SetActive(true);
        CommonTool.In.SetFocus(new Vector2(1352, 602), new Vector2(155, 70));
        mgr.SkipToLastLine();

        mgr.StartText("Tutorial4", EndTutorial4Routine, EndTutorial4Routine);
    }

    private void EndTutorial4Routine()
    {
        mgr.popupChatPanel.SetActive(false);

        mgr.yes.interactable = true;
        mgr.yes.onClick.RemoveAllListeners();
        mgr.yes.onClick.AddListener(() =>
        {
            mgr.EndText();
            mgr.no.interactable = true;
            mgr.ActiveYesNoButton(false);
            mgr.mainPanel.SetActive(true);
            mgr.weaponUI.SetActive(true);
            var pos = mgr.popupChatPanelRect.anchoredPosition;
            pos.x = 150;
            mgr.popupChatPanelRect.anchoredPosition = pos;
            mgr.popupChatPanel.SetActive(true);
            CommonTool.In.SetFocusOff();
            mgr.StartText("Tutorial5", EndTutorial5Routine, EndTutorial5Routine);
        });
    }

    private void EndTutorial5Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);

        mgr.bluePrintSlot[0].button.onClick.AddListener(() =>
        {
            mgr.mainPanel.SetActive(false);
            mgr.weaponUI.SetActive(false);
            mgr.gamePanel.SetActive(true);
            mgr.popupChatPanel.SetActive(true);
            mgr.StartText("Tutorial6", EndTutorial6Routine, SkipTutorial6Routine);

            mgr.puzzleMgr.OnMakingDone += OnMakingDone;
            GameMgr.In.currentBluePrint = GameMgr.In.bluePrintTable.bluePrintList.Find(x => x.bluePrintKey.Equals("sword"));
            mgr.puzzleMgr.StartTutorialPuzzle();
            mgr.bluePrintSlot[0].button.onClick.RemoveAllListeners();
        });
    }

    private void EndTutorial6Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);
    }

    private void SkipTutorial6Routine()
    {
        CommonTool.In.SetFocusOff();
        EndTutorial6Routine();
    }

    private void OnMakingDone()
    {
        mgr.mainChatText.text = string.Empty;
        mgr.gamePanel.SetActive(false);
        mgr.StartText("Tutorial7", EndTutorial7Routine, SkipTutorial7Routine);
    }

    private void EndTutorial7Routine()
    {
        mgr.EndText();

        mgr.alertPanel.SetActive(true);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Tutorial8", EndTutorial8Routine, SkipTutorial8Routine);
        });
    }

    private void SkipTutorial7Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("모브NPC")).imageObj.SetActive(false);
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(true);
        mgr.chatName.text = "샤일로";
        EndTutorial7Routine();
    }

    private void EndTutorial8Routine()
    {
        mgr.EndText();
        mgr.prevChatTarget = GameSceneMgr.ChatTarget.None;
        mgr.pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        mgr.pc.onClick.RemoveAllListeners();
        mgr.pc.onClick.AddListener(() =>
        {
            mgr.RefreshCreditPanel();
            mgr.creditPanel.SetActive(true);
            mgr.creditDodge.onClick.RemoveAllListeners();
            mgr.creditDodge.onClick.AddListener(() =>
            {
                EndFlow();
            });
            mgr.pc.onClick.RemoveAllListeners();
        });
    }

    private void SkipTutorial8Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(false);
        mgr.mainChatPanel.SetActive(false);
        EndTutorial8Routine();
    }
}
