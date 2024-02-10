using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay7 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day7_1", EndDay7_1Routine, EndDay7_1Routine);
    }

    private void EndDay7_1Routine()
    {
        mgr.EndText(false);
        mgr.eventBtntext1.text = "이해한다";
        mgr.eventBtntext2.text = "이해 못 해!";
        
        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency -= 25;
            mgr.ActiveEventButton(false);
            mgr.StartText("Day7_2", EndDay7_2Routine, EndDay7_3Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency += 25;
            mgr.ActiveEventButton(false);
            mgr.StartText("Day7_3", EndDay7_2Routine, EndDay7_3Routine);
        });

        mgr.ActiveEventButton(true);
    }
    
    //TODO: 칩셋 획득으로 리소스 업데이트
    private void EndDay7_2Routine() //칩셋 획득 이벤트 
    {
        mgr.EndText();

        mgr.alertPanel.SetActive(true);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Day7_4", EndDay7_3Routine, EndDay7_3Routine);
        });
    }
    
    private void EndDay7_3Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        
        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
