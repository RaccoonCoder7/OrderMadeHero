using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static WeaponDataTable;

/// <summary>
/// 주문에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "OrderTable", menuName = "SO/OrderTable", order = 1)]
public class OrderTable : ScriptableObject
{
    public List<Order> orderList = new List<Order>();
    public List<MappingText> mappingTextList = new List<MappingText>();

    [System.Serializable]
    public class Order
    {
        public string orderKey;
        public TextAsset ta;
        public List<string> requiredBlueprintKeyList = new List<string>();
        public List<RequiredAbility> requiredAbilityList = new List<RequiredAbility>();
        public Condition condition = Condition.상태없음;
        public string orderCondition;
        public bool orderEnable = true;
    }

    [System.Serializable]
    public class RequiredAbility
    {
        public string abilityKey;
        public int count;
    }

    [System.Serializable]
    public class MappingText
    {
        public string mappingKey;
        public MappingType mappingType;

        public enum MappingType
        {
            BluePrint,
            Request,
            Condition,
        }
    }


    [System.Serializable]
    public enum Condition
    {
        상태없음,
        완벽한,
        적당한,
        대충한,
        // 멋있는,
        // 화려한,
        // 눈부신,
        일회용
    }


    /// <summary>
    /// 주문정보를 새로 생성하여 반환함
    /// </summary>
    /// <param name="orderKey">주문 키</param>
    /// <returns>새로 생성 된 주문정보</returns>
    public Order GetNewOrder(string orderKey)
    {
        var targetOrder = orderList.Find(x => x.orderKey.Equals(orderKey));
        if (targetOrder == null)
        {
            Debug.Log("orderKey에 해당하는 Order가 없습니다.");
            return null;
        }

        return GetNewOrder(targetOrder);
    }

        /// <summary>
    /// 주문정보를 새로 생성하여 반환함
    /// </summary>
    /// <param name="orderKey">주문 키</param>
    /// <returns>새로 생성 된 주문정보</returns>
    public Order GetNewOrder(Order targetOrder)
    {
        // 주문 깊은복사
        var newOrder = new Order();
        newOrder.orderKey = targetOrder.orderKey;

        foreach (var bpKey in targetOrder.requiredBlueprintKeyList)
        {
            newOrder.requiredBlueprintKeyList.Add(bpKey);
        }

        foreach (var ability in targetOrder.requiredAbilityList)
        {
            RequiredAbility ra = new RequiredAbility();
            ra.abilityKey = ability.abilityKey;
            ra.count = ability.count;
            newOrder.requiredAbilityList.Add(ra);
        }

        // 기믹 파싱
        var keyDic = new Dictionary<string, string>();
        string text = string.Copy(targetOrder.ta.text);
        for (int i = 0; i < text.Length; i++)
        {
            if (!text[i].Equals('{'))
            {
                continue;
            }

            if (i + 1 >= text.Length)
            {
                Debug.Log("ta에 비정상적인 패턴이 포함되어있습니다.");
                break;
            }

            int tempIndex = i;
            while (tempIndex < text.Length)
            {
                if (text[tempIndex].Equals('}'))
                {
                    break;
                }
                tempIndex++;
            }

            var key = text.Substring(i, tempIndex - i + 1);
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, string.Empty);
            }
            i = tempIndex;
        }

        var keys = keyDic.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            var matchedData = mappingTextList.Find(x => keys[i].Contains(x.mappingKey));
            if (matchedData == null)
            {
                Debug.Log("매칭되는 데이터가 없음: " + keys[i]);
                return null;
            }

            switch (matchedData.mappingType)
            {
                case MappingText.MappingType.BluePrint:
                    var bluePrintList = new List<BluePrint>();
                    foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
                    {
                        foreach (var bp in bpc.bluePrintList)
                        {
                            if (bp.orderEnable) bluePrintList.Add(bp);
                        }
                    }
                    while (true)
                    {
                        var bluePrint = bluePrintList[UnityEngine.Random.Range(0, bluePrintList.Count)];
                        if (keyDic.ContainsValue(bluePrint.name))
                        {
                            if (bluePrintList.Count >= 2) continue;
                        }
                        keyDic[keys[i]] = bluePrint.name;
                        newOrder.requiredBlueprintKeyList.Add(bluePrint.bluePrintKey);
                        break;
                    }
                    break;
                case MappingText.MappingType.Request:
                    // TODO: AbilityList가 아닌 Request리스트에서 찾아야함
                    var abilityList = GameMgr.In.abilityTable.abilityList;
                    var orderableAbilityList = abilityList.Where(x => x.orderEnable).ToList();
                    while (true)
                    {
                        var ability = orderableAbilityList[UnityEngine.Random.Range(0, abilityList.Count)];
                        if (keyDic.ContainsValue(ability.desc))
                        {
                            if (orderableAbilityList.Count >= 2) continue;
                        }
                        if (newOrder.requiredAbilityList.Find(x => x.abilityKey.Equals(ability.abilityKey)) != null)
                        {
                            continue;
                        }
                        keyDic[keys[i]] = ability.desc;

                        var ra = new RequiredAbility();
                        ra.abilityKey = ability.abilityKey;
                        ra.count = 1;
                        newOrder.requiredAbilityList.Add(ra);

                        break;
                    }
                    break;
                case MappingText.MappingType.Condition:
                    var conditionNum = UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(Condition)).Length);
                    var condition = (Condition)conditionNum;
                    newOrder.condition = condition;
                    keyDic[keys[i]] = condition.ToString();
                    break;
                default:
                    break;
            }

            text = text.Replace(keys[i], keyDic[keys[i]]);
        }
        newOrder.ta = new TextAsset(text);
        return newOrder;
    }

    public bool IsConditionMatched(List<PuzzleFrame> puzzleFrameList, Condition condition)
    {
        if (condition == Condition.상태없음 || condition == Condition.대충한)
        {
            return true;
        }

        var abilityDic = new Dictionary<string, int>();
        List<ChipObj> chipObjList = new List<ChipObj>();
        foreach (var puzzleFrame in puzzleFrameList)
        {
            if (puzzleFrame.transform.childCount > 0)
            {
                var child = puzzleFrame.transform.GetChild(0);
                if (child)
                {
                    var chip = child.GetComponent<ChipObj>();
                    if (chip)
                    {
                        chipObjList.Add(chip);
                        foreach (var ability in chip.chipAbilityList)
                        {
                            if (abilityDic.TryGetValue(ability.abilityKey, out int currentCount))
                            {
                                abilityDic[ability.abilityKey] = currentCount + 1;
                            }
                            else
                            {
                                abilityDic.Add(ability.abilityKey, 1);
                            }
                        }
                    }
                }
            }
        }

        switch (condition)
        {
            case Condition.완벽한:
                int totalSize1 = GetTotalSizeOfChips(chipObjList);
                return totalSize1 == puzzleFrameList.Count;
            case Condition.적당한:
                int totalSize2 = GetTotalSizeOfChips(chipObjList);
                return totalSize2 >= puzzleFrameList.Count / 2;
            case Condition.일회용:
                var durability = abilityDic.FirstOrDefault(x => x.Key.Equals("a_durability"));
                if (durability.Equals(default(KeyValuePair<string, int>)))
                {
                    return true;
                }

                foreach (var ra in GameMgr.In.currentBluePrint.requiredChipAbilityList)
                {
                    if (!ra.abilityKey.Equals("a_durability")) continue;
                    return durability.Value <= ra.count;
                }

                return false;
        }
        return false;
    }

    private int GetTotalSizeOfChips(List<ChipObj> chipObjList)
    {
        int totalSize = 0;
        foreach (var chipObj in chipObjList)
        {
            totalSize += chipObj.GetChipSize();
        }

        return totalSize;
    }
}
