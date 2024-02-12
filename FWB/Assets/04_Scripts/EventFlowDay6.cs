using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay6 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day6_1", EndDay6_1Routine, EndDay6_1Routine);
    }

    private void EndDay6_1Routine() //칩셋 획득 이벤트 
    {
        mgr.EndText();

        var speedChip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals("버니"));
        speedChip.createEnable = true;

        mgr.mainChatPanel.SetActive(false);
        mgr.alertPanel.SetActive(true);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Day6_2", EndDay6_2Routine, EndDay6_2Routine);
        });
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
            // TODO: UI에 tendency 반영
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_3", EndDay6_3Routine, EndDay6_3Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency -= 25;
            // TODO: UI에 tendency 반영
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_4", EndDay6_3Routine, EndDay6_3Routine);
        });

        mgr.ActiveEventButton(true);
    }
    private void EndDay6_3Routine()
    {
        mgr.EndText();
        mgr.renom.SetActive(true);
        mgr.StartText("Day6_5", EndDay6_5Routine);
    }

    private void EndDay6_5Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        mgr.isEventFlowing = false;
    }
}
