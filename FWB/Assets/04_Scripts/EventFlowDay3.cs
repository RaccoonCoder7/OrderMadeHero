using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay3 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day3_1", EndDay3_1Routine, EndDay3_1Routine);
    }

    private void EndDay3_1Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();
    }
}
