using System.Collections;
using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay17 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.tendencyType == GameMgr.TendencyType.Hero)
        {
            mgr.StartText("Day17_1", EndDay17_1Routine, SkipDay17_1Routine);
        }
        else
        {
            mgr.StartText("Day17_2", EndDay17_1Routine, SkipDay17_1Routine);
        }
    }

    private void EndDay17_1Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);

        EndFlow();
    }

    private void SkipDay17_1Routine()
    {
        foreach (var img in mgr.imageList)
        {
            img.imageObj.SetActive(false);
        }
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);

        EndDay17_1Routine();
    }
}
