using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static WeaponDataTable;
using static ConditionTable;

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

        // var newRequestList = new List<OrderRequest>();
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
                            bluePrintList.Add(bp);
                        }
                    }
                    while (true)
                    {
                        var bluePrint = bluePrintList[UnityEngine.Random.Range(0, bluePrintList.Count)];
                        if (keyDic.ContainsValue(bluePrint.name))
                        {
                            continue;
                        }
                        keyDic[keys[i]] = bluePrint.name;
                        newOrder.requiredBlueprintKeyList.Add(bluePrint.bluePrintKey);
                        break;
                    }
                    break;
                case MappingText.MappingType.Request:
                    var abilityList = GameMgr.In.abilityTable.abilityList;
                    while (true)
                    {
                        var ability = abilityList[UnityEngine.Random.Range(0, abilityList.Count)];
                        if (keyDic.ContainsValue(ability.desc))
                        {
                            continue;
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
                    var condition = (Condition) conditionNum;
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
}
