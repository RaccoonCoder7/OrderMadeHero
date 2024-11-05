using System.Collections;
using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay10 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.tendencyType == GameMgr.TendencyType.Hero)
        {
            mgr.StartText("Day10_1", EndDay10_1Routine, SkipDay10_1Routine);
        }
        else
        {
            mgr.StartText("Day10_2", EndDay10_1Routine, SkipDay10_1Routine);
        }
    }

    private void EndDay10_1Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);

        EndFlow();
    }

    private void SkipDay10_1Routine()
    {
        foreach (var img in mgr.imageList)
        {
            img.imageObj.SetActive(false);
        }
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);

        EndDay10_1Routine();
    }
}
