using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay2 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            mgr.StartText("Day2_1", EndDay2_1Routine, SkipDay2_1Routine);
        }
        else
        {
            StartCoroutine(mgr.StartNormalRoutine(4, mgr.EndNormalOrderRoutine));
        }
    }

    private void EndDay2_1Routine()
    {
        mgr.EndText(false);

        mgr.yes.onClick.RemoveAllListeners();
        mgr.yes.onClick.AddListener(() =>
        {
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Day2_2", EndDay2_2Routine, SkipDay2_4Routine);
        });

        mgr.no.onClick.RemoveAllListeners();
        mgr.no.onClick.AddListener(() =>
        {
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Day2_3", EndDay2_3Routine, SkipDay2_4Routine);
        });
        
        mgr.ActiveYesNoButton(true);
    }

    private void SkipDay2_1Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(true);
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatTarget = GameSceneMgr.ChatTarget.Main;
        mgr.chatTargetText = mgr.mainChatText;
        mgr.SkipToLastLine();
        EndDay2_1Routine();
    }

    private void EndDay2_2Routine()
    {
        mgr.EndText();
        mgr.ObjectBlinker(mgr.renom, 10, 2);
        mgr.StartText("Day2_4", EndDay2_4Routine);
    }

    private void EndDay2_3Routine()
    {
        mgr.EndText();
        mgr.ObjectBlinker(mgr.renom, 10, 2);
        mgr.StartText("Day2_4", EndDay2_4Routine);
    }

    private void EndDay2_4Routine()
    {
        mgr.EndText();
        mgr.prevChatTarget = GameSceneMgr.ChatTarget.None;
        mgr.pcChatPanel.SetActive(false);
        GameMgr.In.isEventOn = 0;
        StartCoroutine(mgr.StartNormalRoutine(4, mgr.EndNormalOrderRoutine));
    }

    private void SkipDay2_4Routine()
    {
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }

        mgr.mainChatPanel.SetActive(false);
        mgr.renom.SetActive(true);
        EndDay2_4Routine();
    }
}
