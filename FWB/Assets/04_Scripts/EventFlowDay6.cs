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

    private void EndDay6_1Routine()
    {
        mgr.EndText(false);
        
        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_2", EndDay6_2Routine, EndDay6_4Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText("Day6_3", EndDay6_3Routine, EndDay6_4Routine);
        });

        mgr.ActiveEventButton(true);
    }
    private void EndDay6_2Routine()
    {
        mgr.EndText();
        mgr.renom.SetActive(true);
        mgr.StartText("Day6_4", EndDay6_4Routine);
    }

    private void EndDay6_3Routine()
    {
        mgr.EndText();
        mgr.renom.SetActive(true);
        mgr.StartText("Day6_4", EndDay6_4Routine);
    }
    private void EndDay6_4Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        
        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
