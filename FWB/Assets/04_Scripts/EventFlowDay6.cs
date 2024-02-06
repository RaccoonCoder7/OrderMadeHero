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
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);

        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
