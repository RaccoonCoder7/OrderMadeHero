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
        mgr.goldText.text = GameMgr.In.credit.ToString();
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
        CommonTool.In.SetFocus(new Vector2(1325, 555), new Vector2(155, 70));

        mgr.yes.onClick.AddListener(() =>
        {
            mgr.no.interactable = true;
            mgr.ActiveYesNoButton(false);
            mgr.popupPanel.SetActive(true);
            mgr.RefreshPopupPanel();
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
        CommonTool.In.SetFocus(new Vector2(285, 620), new Vector2(60, 60));

        // TODO: 다른 버튼에 대한 조작을 막는 방법??
        mgr.bluePrintSlotList[0].button.onClick.AddListener(OnClickBlueprintSlot);
    }

    private void OnClickBlueprintSlot()
    {
        CommonTool.In.SetFocusOff();
        mgr.StartText("Tutorial5_1", EndTutorial5_1Routine, EndTutorial5_1Routine);
        mgr.bluePrintSlotList[0].button.onClick.RemoveListener(OnClickBlueprintSlot);
    }

    private void EndTutorial5_1Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);
        CommonTool.In.SetFocus(new Vector2(950, 170), new Vector2(280, 90));
        mgr.weaponCreate.onClick.AddListener(OnClickWeaponCreate);
    }

    private void OnClickWeaponCreate()
    {
        CommonTool.In.SetFocusOff();
        CommonTool.In.cancelBtn.interactable = false;
        CommonTool.In.confirmBtn.onClick.AddListener(OnClickConfirm);
        mgr.weaponCreate.onClick.RemoveListener(OnClickWeaponCreate);
    }

    private void OnClickConfirm()
    {
        mgr.StartText("Tutorial5_2", EndTutorial5_2Routine, EndTutorial5_2Routine);
        CommonTool.In.cancelBtn.interactable = true;
    }

    private void EndTutorial5_2Routine()
    {
        mgr.EndText();
        CommonTool.In.SetFocusOff();
        mgr.popupChatPanel.SetActive(false);
        mgr.puzzleMgr.OnMakingDone += OnMakingDone;
    }

    private void OnMakingDone()
    {
        mgr.StartText("Tutorial6", EndTutorial6Routine, SkipTutorial6Routine);
        mgr.puzzleMgr.OnMakingDone -= OnMakingDone;
    }

    private void EndTutorial6Routine() //청사진 획득 이벤트 
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
        mgr.pcChatPanel.SetActive(false);
        mgr.chatName.text = "샤일로";
        EndTutorial6Routine();
    }

    private void EndTutorial7Routine()
    {
        mgr.EndText();
        mgr.prevChatTarget = GameSceneMgr.ChatTarget.None;
        mgr.pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        var coroutine = StartCoroutine(mgr.BlinkNavi());
        mgr.pc.onClick.RemoveAllListeners();
        mgr.pc.onClick.AddListener(() =>
        {
            StopCoroutine(coroutine);
            mgr.deskNavi.SetActive(true);
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
