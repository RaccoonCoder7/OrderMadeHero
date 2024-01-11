using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay5 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day5_1", EndDay5_1Routine, EndDay5_1Routine);
    }

    private void EndDay5_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);

        mgr.imageList.Find(x => x.key.Equals("매드")).imageObj.SetActive(false);
        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
