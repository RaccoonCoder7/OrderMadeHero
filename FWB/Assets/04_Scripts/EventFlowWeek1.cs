using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlowWeek1 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            CommonTool.In.SetFocus(new Vector2(1200, 35), new Vector2(300,125));
            mgr.SetNewsButtonListener();
            mgr.news.onClick.AddListener(OnClickNews);
        }
        else
        {
            StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        }
    }

    private void OnClickNews()
    {
        CommonTool.In.SetFocusOff();
        StartCoroutine(DelayFlow());
    }
    
    private void FirstWeekNews1()
    {
        mgr.StartText("Week1_2", FirstWeekNews2, FirstWeekNews2);
    }
    
    private void FirstWeekNews2()
    {
        mgr.StartText("Week1_3", FirstWeekNews2, FirstWeekNews2);
    }
    
    private IEnumerator DelayFlow()
    {
        mgr.inNews = true;
        mgr.news.onClick.RemoveListener(OnClickNews);
        yield return new WaitForSeconds(1.5f);
        mgr.StartText("Week1_1", FirstWeekNews1, FirstWeekNews1);
    }
}
