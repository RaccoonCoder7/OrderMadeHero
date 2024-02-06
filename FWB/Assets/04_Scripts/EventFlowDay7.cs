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
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);

        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
