using static BluePrintTable;

/// <summary>
/// 게임 중 사용되는 데이터를 관리하는 매니저 컴포넌트
/// </summary>
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

    /// <summary>
    /// 하루가 지났을 때에 리셋해야하는 정보들을 리셋함
    /// </summary>
    public void ResetDayData()
    {
        dayMaterialCost = 0;
        dayStoreCost = 0;
        dayRentCost = 0;
        dayRevenue = 0;
    }
}
