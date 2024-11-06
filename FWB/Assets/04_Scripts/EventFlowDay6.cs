using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay6 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day6_1", EndDay6_1Routine, SkipDay6_1Routine);
    }

    private void EndDay6_1Routine() //칩셋 획득 이벤트 
    {
        mgr.EndText();

        var speedChip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals("버니"));
        speedChip.createEnable = true;
        speedChip.chipState = 3;
        
        mgr.mainChatPanel.SetActive(false);
        mgr.GetChipset(0);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Day6_2", EndDay6_2Routine, SkipDay6_2Routine);
        });
    }

    private void SkipDay6_1Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("버니")).imageObj.SetActive(true);
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        EndDay6_1Routine();
    }

    private void EndDay6_2Routine()
    {
        mgr.EndText(false);
        mgr.eventBtntext1.text = "신념과 정의";
        mgr.eventBtntext2.text = "욕망과 자본";

        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency += 25;
            GameMgr.In.tendency += 25;
            mgr.TendUIMove();
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_3", EndDay6_3Routine, SkipDay6_3Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency -= 25;
            GameMgr.In.tendency -= 25;
            mgr.TendUIMove();
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_4", EndDay6_3Routine, SkipDay6_3Routine);
        });

        mgr.ActiveEventButton(true);
    }

    private void SkipDay6_2Routine()
    {
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatTarget = GameSceneMgr.ChatTarget.Main;
        mgr.chatTargetText = mgr.mainChatText;
        mgr.SkipToLastLine();
        EndDay6_2Routine();
    }

    private void EndDay6_3Routine()
    {
        mgr.EndText();
        mgr.renom.SetActive(true);
        mgr.mainChatPanel.SetActive(false);
        mgr.StartText("Day6_5", EndDay6_5Routine, SkipDay6_3Routine);
    }

    private void SkipDay6_3Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("버니")).imageObj.SetActive(false);
        EndDay6_5Routine();
    }

    private void EndDay6_5Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        EndFlow();
    }
}
