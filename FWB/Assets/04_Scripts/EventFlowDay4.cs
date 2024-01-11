using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay4 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Dummy", EndDay4_1Routine, EndDay4_1Routine);
    }

    private void EndDay4_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);

        mgr.imageList.Find(x => x.key.Equals("매드")).imageObj.SetActive(false);
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
    }
}
