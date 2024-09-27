using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay4 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            mgr.StartText("Day4_1", EndDay4_1Routine, EndDay4_1Routine);
        }
        else
        {
            StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        }
    }

    private void EndDay4_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        GameMgr.In.isEventOn = 0;
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
    }
}
