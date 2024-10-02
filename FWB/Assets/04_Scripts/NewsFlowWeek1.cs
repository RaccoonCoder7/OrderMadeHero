using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsFlowWeek1 : EventFlow
{
    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            mgr.SetNewsButtonListener();
            mgr.news.onClick.AddListener(OnClickNews);
        }
        else
        {
            
        }
    }

    private void OnClickNews()
    {
        
    }
    
    private void FirstWeekNews1()
    {
        mgr.StartText("Week1_1", FirstWeekNews1, FirstWeekNews1);
    }
}
