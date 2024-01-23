using UnityEngine;
using static BluePrintTable;
using static WeaponDataTable;

/// <summary>
/// 게임 중 사용되는 데이터를 관리하는 매니저 컴포넌트
/// </summary>
public class GameMgr : SingletonMono<GameMgr>
{
    public bool initDone;
    public Day day = Day.월;
    public int week = 1;
    public int credit;
    public int dayMaterialCost;
    public int dayStoreCost;
    public int dayRentCost;
    public int dayRevenue;
    public WeaponDataTable weaponDataTable;
    public OrderTable orderTable;
    public ChipTable chipTable;
    public AbilityTable abilityTable;
    public WeaponDataTable.BluePrint currentBluePrint;

    public enum Day
    {
        월 = 1,
        화 = 2,
        수 = 3,
        목 = 4,
        금 = 5,
    }

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

    /// <summary>
    /// 하루가 지났을 때에 변경해줘야 하는 정보들을 세팅함
    /// </summary>
    public void SetNextDayData()
    {
        day++;
        if ((int)day > 5)
        {
            week++;
            day = (Day)1;
        }
    }

    /// <summary>
    /// 키 값에 맞는 무기카테고리정보를 반환
    /// </summary>
    public BluePrintCategory GetWeaponCategory(string categoryKey)
    {
        return GameMgr.In.weaponDataTable.bluePrintCategoryList.Find(x => x.categoryKey.Equals(categoryKey));
    }

    /// <summary>
    /// 키 값에 맞는 무기정보를 반환
    /// </summary>
    public WeaponDataTable.BluePrint GetWeapon(string categoryKey, string key)
    {
        var category = GetWeaponCategory(categoryKey);
        return category.bluePrintList.Find(x => x.bluePrintKey.Equals(key));
    }

    /// <summary>
    /// 키 값에 맞는 능력치 정보를 반환
    /// </summary>
    public AbilityTable.Ability GetAbility(string abilityKey)
    {
        return abilityTable.abilityList.Find(x => x.abilityKey.Equals(abilityKey));
    }
}
