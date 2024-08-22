using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
        mgr.EndText(false);
        mgr.eventBtntext1.text = "이해한다";
        mgr.eventBtntext2.text = "이해 못 해!";

        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency -= 25;
            GameMgr.In.tendency -= 25;
            mgr.TendUIMove();
            mgr.ActiveEventButton(false);
            mgr.StartText("Day7_2", EndDay7_2Routine, EndDay7_2Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            GameMgr.In.dayTendency += 25;
            GameMgr.In.tendency += 25;
            mgr.TendUIMove();
            mgr.ActiveEventButton(false);
            mgr.StartText("Day7_3", EndDay7_2Routine, EndDay7_2Routine);
        });

        mgr.ActiveEventButton(true);
    }

    private void EndDay7_2Routine() //칩셋 획득 이벤트 
    {
        mgr.EndText();

        var speedChip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals("퍼펫"));
        speedChip.createEnable = true;

        mgr.mainChatPanel.SetActive(false);
        mgr.GetChipset(1);
        mgr.alertDodge.onClick.RemoveAllListeners();
        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Day7_4", EndDay7_4Routine, EndDay7_4Routine);
        });
    }

    private void EndDay7_4Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        mgr.isEventFlowing = false;

        foreach (var order in GameMgr.In.orderTable.orderList)
        {
            if (order.orderCondition.Equals("Day8"))
            {
                order.orderEnable = true;
            }
        }

        foreach (var request in GameMgr.In.requestTable.requestList)
        {
            if (request.orderCondition.Equals("Day8"))
            {
                request.orderEnable = true;
            }
        }

        StartCoroutine(QuitGame());
    }

    private IEnumerator QuitGame()
    {
        yield return StartCoroutine(CommonTool.In.FadeOut());
        yield return new WaitForSeconds(1f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
