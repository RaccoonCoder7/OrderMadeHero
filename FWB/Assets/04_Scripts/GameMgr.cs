using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BluePrintTable;
using static OrderTable;

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
    public BluePrintTable bluePrintTable;
    public OrderTable orderTable;
    public ChipTable chipTable;
    public AbilityTable abilityTable;
    public BluePrint currentBluePrint;

    public void ResetDayData()
    {
        dayMaterialCost = 0;
        dayStoreCost = 0;
        dayRentCost = 0;
        dayRevenue = 0;
    }
}
