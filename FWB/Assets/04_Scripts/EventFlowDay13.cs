using System.Collections;
using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay13 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.tendencyType == GameMgr.TendencyType.Hero)
        {
            mgr.StartText("Day13_1", EndDay13_1Routine, SkipDay13_1Routine);
        }
        else
        {
            mgr.StartText("Day13_2", EndDay13_1Routine, SkipDay13_1Routine);
        }
    }

    private void EndDay13_1Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        mgr.doDaySkip = true;
        GameMgr.In.SetNextDayData();

        EndFlow();
    }

    private void SkipDay13_1Routine()
    {
        foreach (var img in mgr.imageList)
        {
            img.imageObj.SetActive(false);
        }
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);

        EndDay13_1Routine();
    }
}
