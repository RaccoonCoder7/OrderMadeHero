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
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Tutorial2", EndTutorial2Routine, EndTutorial2Routine);
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

    private void EndTutorial2Routine()
    {
        mgr.EndText(true);
        mgr.day.SetActive(true);
        mgr.tendency.SetActive(true);
        mgr.gold.SetActive(true);
        StartCoroutine(mgr.FadeInOutDateMessage());

        mgr.StartText("Tutorial3", EndTutorial3Routine, SkipTutorial3Routine);
    }

    private void EndTutorial3Routine()
    {
        mgr.EndText(false);
        mgr.yesText.text = "Yes";
        mgr.noText.text = "No";
        mgr.yes.interactable = true;
        mgr.no.interactable = false;
        mgr.ActiveYesNoButton(true);
        CommonTool.In.SetFocus(new Vector2(1352, 555), new Vector2(155, 70));

        mgr.yes.onClick.AddListener(() =>
        {
            mgr.no.interactable = true;
            mgr.ActiveYesNoButton(false);
            mgr.mainPanel.SetActive(true);
            mgr.weaponUI.SetActive(true);
            var pos = mgr.popupChatPanelRect.anchoredPosition;
            pos.x = 150;
            mgr.popupChatPanelRect.anchoredPosition = pos;
            mgr.popupChatPanel.SetActive(true);
            CommonTool.In.SetFocusOff();
            mgr.StartText("Tutorial4", EndTutorial4Routine, EndTutorial4Routine);
        });
    }

    private void SkipTutorial3Routine()
    {
        mgr.chatTarget = GameSceneMgr.ChatTarget.Main;
        mgr.chatTargetText = mgr.mainChatText;
        mgr.popupChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatName.text = "모브NPC";
        mgr.imageList.Find(x => x.key.Equals("모브NPC")).imageObj.SetActive(true);
        mgr.SkipToLastLine();

        EndTutorial3Routine();
    }

    private void EndTutorial4Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);

        mgr.bluePrintSlot[0].button.onClick.AddListener(() =>
        {
            mgr.mainPanel.SetActive(false);
            mgr.weaponUI.SetActive(false);
            mgr.gamePanel.SetActive(true);
            mgr.popupChatPanel.SetActive(true);
            mgr.StartText("Tutorial5", EndTutorial5Routine, EndTutorial5Routine);

            mgr.puzzleMgr.OnMakingDone += OnMakingDone;
            GameMgr.In.currentBluePrint = GameMgr.In.bluePrintTable.bluePrintList.Find(x => x.bluePrintKey.Equals("sword"));
            mgr.puzzleMgr.StartTutorialPuzzle();
            mgr.bluePrintSlot[0].button.onClick.RemoveAllListeners();
        });
    }

    // private void EndTutorial5Routine()
    // {
    //     mgr.EndText();
    //     mgr.popupChatPanel.SetActive(false);

    //     mgr.bluePrintSlot[0].button.onClick.AddListener(() =>
    //     {
    //         mgr.mainPanel.SetActive(false);
    //         mgr.weaponUI.SetActive(false);
    //         mgr.gamePanel.SetActive(true);
    //         mgr.popupChatPanel.SetActive(true);
    //         mgr.StartText("Tutorial6", EndTutorial6Routine, SkipTutorial6Routine);

    //         mgr.puzzleMgr.OnMakingDone += OnMakingDone;
    //         GameMgr.In.currentBluePrint = GameMgr.In.bluePrintTable.bluePrintList.Find(x => x.bluePrintKey.Equals("sword"));
    //         mgr.puzzleMgr.StartTutorialPuzzle();
    //         mgr.bluePrintSlot[0].button.onClick.RemoveAllListeners();
    //     });
    // }

    private void EndTutorial5Routine()
    {
        mgr.EndText();
        CommonTool.In.SetFocusOff();
        mgr.popupChatPanel.SetActive(false);
    }

    private void OnMakingDone()
    {
        mgr.mainChatText.text = string.Empty;
        mgr.gamePanel.SetActive(false);
        mgr.StartText("Tutorial6", EndTutorial6Routine, SkipTutorial6Routine);
    }

    private void EndTutorial6Routine()
    {
        mgr.EndText();

        mgr.alertPanel.SetActive(true);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Tutorial7", EndTutorial7Routine, SkipTutorial7Routine);
        });
    }

    private void SkipTutorial6Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("모브NPC")).imageObj.SetActive(false);
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(true);
        mgr.chatName.text = "샤일로";
        EndTutorial6Routine();
    }

    private void EndTutorial7Routine()
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

    private void SkipTutorial7Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(false);
        mgr.mainChatPanel.SetActive(false);
        EndTutorial7Routine();
    }
}
