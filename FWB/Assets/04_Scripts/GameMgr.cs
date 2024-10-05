using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BluePrintTable;
using static OrderTable;
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
    public int dayShopBuyCost;
    public int dayChipUseCost;
    public int dayBonusRevenue;
    public int dayRentCost = 0;
    public int dayRevenue;
    public int dayCustomerCnt;
    public int dayFame;
    public int dayTendency;
    public int tendency;
    public int fame;
    public int minFame = 0;
    public int maxFame = 1000;
    public int minTend = -1000;
    public int maxTend = 1000;
    public int lastDayCredit = 0;
    public int lastDayFame = 0;
    public int lastDayTend = 0;
    public int isEventOn = 0;
    public bool isBankrupt = false;
    public int endDay = 28;
    public int continuousSuccessCnt = 1;
    public int continuousPerfectCnt = 1;
    public float feverModeProbability;
    public WeaponDataTable weaponDataTable;
    public OrderTable orderTable;
    public ChipTable chipTable;
    public AbilityTable abilityTable;
    public RequestDataTable requestTable;
    public WeaponDataTable.BluePrint currentBluePrint;
    [HideInInspector]
    public Order currentOrder;
    [HideInInspector]
    public List<string> orderedBluePrintKeyList = new List<string>();

    public enum Day
    {
        월 = 1,
        화 = 2,
        수 = 3,
        목 = 4,
        금 = 5,
        토 = 6,
        일 = 7
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        PlayerPrefs.DeleteAll();
        ResetDataTables();
    }

    /// <summary>
    /// 하루가 지났을 때에 리셋해야하는 정보들을 리셋함
    /// </summary>
    public void ResetDayData()
    {
        dayShopBuyCost = 0;
        dayChipUseCost = 0;
        dayBonusRevenue = 0;
        dayRevenue = 0;
        dayCustomerCnt = 0;
        dayFame = 0;
        dayTendency = 0;
    }

    /// <summary>
    /// 하루가 지났을 때에 변경해줘야 하는 정보들을 세팅함
    /// </summary>
    public void SetNextDayData()
    {
        day++;
        if ((int)day > System.Enum.GetValues(typeof(Day)).Length)
        {
            week++;
            if (week > 4) week = 1;
            day = (Day)1;
            GameMgr.In.feverModeProbability = 0;
        }

        fame = ClampValue(fame, maxFame, minFame);
        tendency = ClampValue(tendency, maxTend, minTend);
    }

    private int ClampValue(int value, int max, int min)
    {
        if (value <= min)
        {
            return min;
        }
        if (value >= max)
        {
            return max;
        }
        return value;
    }

    /// <summary>
    /// 키 값에 맞는 무기카테고리정보를 반환
    /// </summary>
    public BluePrintCategory GetWeaponCategory(string categoryKey)
    {
        return weaponDataTable.bluePrintCategoryList.Find(x => x.categoryKey.Equals(categoryKey));
    }

    /// <summary>
    /// 키 값에 맞는 무기정보를 반환 (카테고리키를 알고있는 경우, 이 함수를 사용할 것)
    /// </summary>
    public WeaponDataTable.BluePrint GetWeapon(string categoryKey, string key)
    {
        var category = GetWeaponCategory(categoryKey);
        return category.bluePrintList.Find(x => x.bluePrintKey.Equals(key));
    }

    /// <summary>
    /// 키 값에 맞는 무기정보를 반환
    /// </summary>
    public WeaponDataTable.BluePrint GetWeapon(string key)
    {
        foreach (var category in weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in category.bluePrintList)
            {
                if (bp.bluePrintKey.Equals(key))
                {
                    return bp;
                }
            }
        }
        return null;
    }

    public Order GetOrder(string orderKey)
    {
        var targetOrder = orderTable.orderList.Find(x => x.orderKey.Equals(orderKey));
        if (targetOrder == null)
        {
            Debug.Log("orderKey에 해당하는 Order가 없습니다.");
            return null;
        }

        return targetOrder;
    }

    public Order GetRandomNewOrder(string exceptionKey)
    {
        var orderableOrderList = orderTable.orderList.FindAll(x =>
            x.orderEnable && !x.orderKey.Equals(exceptionKey)).ToList();
        var index = UnityEngine.Random.Range(0, orderableOrderList.Count);
        return orderTable.GetNewOrder(orderableOrderList[index]);
    }

    /// <summary>
    /// 키 값에 맞는 칩 정보를 반환
    /// </summary>
    public ChipTable.Chip GetChip(string chipKey)
    {
        return chipTable.chipList.Find(x => x.chipKey.Equals(chipKey));
    }

    /// <summary>
    /// 키 값에 맞는 능력치 정보를 반환
    /// </summary>
    public AbilityTable.Ability GetAbility(string abilityKey)
    {
        return abilityTable.abilityList.Find(x => x.abilityKey.Equals(abilityKey));
    }

    /// <summary>
    /// 키 값에 맞는 요청사항 정보를 반환
    /// </summary>
    public RequestDataTable.Request GetRequest(string requestKey)
    {
        return requestTable.requestList.Find(x => x.requestKey.Equals(requestKey));
    }

    /// <summary>
    /// 현재 주문/청사진 정보를 저장
    /// </summary>
    public void SaveOrderHistory()
    {
        if (currentBluePrint == null) return;
        orderedBluePrintKeyList.Add(currentBluePrint.bluePrintKey);
    }

    /// <summary>
    /// 피버모드 발생 확률 조정
    /// </summary>
    /// <param name="score"></param>
    public void AdjustFeverModeProbability(int score)
    {
        switch (score)
        {
            case 0:
            case 1:
                continuousSuccessCnt = 1;
                continuousPerfectCnt = 1;
                feverModeProbability /= 2;
                break;
            case 2:
                feverModeProbability += continuousSuccessCnt * 2;
                if (feverModeProbability > 100)
                {
                    feverModeProbability = 100;
                }
                continuousSuccessCnt++;
                break;
            case 3:
                feverModeProbability += continuousPerfectCnt * 4;
                if (feverModeProbability > 100)
                {
                    feverModeProbability = 100;
                }
                continuousPerfectCnt++;
                break;
        }
    }

    private void ResetDataTables()
    {
        foreach (var category in weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in category.bluePrintList)
            {
                bool enable = string.IsNullOrEmpty(bp.howToGet);
                bp.orderEnable = enable;
                bp.createEnable = enable;
                bp.weaponState = 0;
            }
        }

        foreach (var order in orderTable.orderList)
        {
            order.orderEnable = order.orderConditionDictionary.Count == 0;
        }

        foreach (var chip in chipTable.chipList)
        {
            bool enable = string.IsNullOrEmpty(chip.howToGet);
            chip.createEnable = enable;
            chip.chipState = 0;
        }

        foreach (var request in requestTable.requestList)
        {
            request.orderEnable = string.IsNullOrEmpty(request.orderCondition);
        }
    }
}
