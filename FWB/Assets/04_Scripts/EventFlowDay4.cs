using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay4 : EventFlow
{
    public override void StartFlow()
    {
        if (DataSaveLoad.dataSave.isLoaded == true)
        {
            StartCoroutine(mgr.StartNormalRoutine(GameMgr.In.dayCustomerCnt, mgr.EndNormalOrderRoutine));
            DataSaveLoad.dataSave.isLoaded = false;
        }
        else
        {
            mgr.StartText("Day4_1", EndDay4_1Routine, EndDay4_1Routine);
        }
    }

    private void EndDay4_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
    }
}
