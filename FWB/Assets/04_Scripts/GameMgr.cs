using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : SingletonMono<GameMgr>
{
    public bool initDone;
    public string day = "월";
    public int week = 1;
    public int credit;
    public int dayMaterialCost;
    public int dayStoreCost;
    public int dayRentCost;
    public int dayRevenue;

    public void ResetDayData()
    {
        dayMaterialCost = 0;
        dayStoreCost = 0;
        dayRentCost = 0;
        dayRevenue = 0;
    }
}
