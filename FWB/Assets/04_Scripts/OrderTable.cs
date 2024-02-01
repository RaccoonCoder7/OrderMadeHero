using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static WeaponDataTable;
using static RequestTable;

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
        public List<OrderRequest> requiredRequestList = new List<OrderRequest>();
        public List<Ability> requiredChipAbilityList = new List<Ability>();
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

        var newOrder = new Order();
        newOrder.orderKey = targetOrder.orderKey;

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

        var newRequestList = new List<OrderRequest>();
        var newAbilityList = new List<Ability>();
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
                // TODO: 컨디션 분리
                // TODO: desc, value 쓰는게 아니라 RequestTable을 만들어서 딕셔너리 밸류값이랑 어빌리티값 넣게 해야함
                // TODO: 조건이 >, < 등 여러가지가 될 수 있게 RequestTable을 만들어야 함
                    var requestList = GameMgr.In.requestTable.requestList;
                    while (true)
                    {
                        var request = requestList[Random.Range(0, requestList.Count)];
                        if (keyDic.ContainsValue(request.requestName))
                        {
                            continue;
                        }
                        keyDic[keys[i]] = request.requestName;
                        newRequestList.Add(request);
                        break;
                    }
                    break;
                case MappingText.MappingType.Condition:
                    var abilityList = GameMgr.In.abilityTable.abilityList;
                    while (true)
                    {
                        var ability = abilityList[UnityEngine.Random.Range(0, abilityList.Count)];
                        if (keyDic.ContainsValue(ability.desc))
                        {
                            continue;
                        }
                        keyDic[keys[i]] = ability.desc;
                        newAbilityList.Add(ability);
                        break;
                    }
                    break;
                default:
                    break;
            }

            text = text.Replace(keys[i], keyDic[keys[i]]);
        }

        newOrder.ta = new TextAsset(text);
        newAbilityList.AddRange(targetOrder.requiredChipAbilityList);
        // TODO: 교체가 아니라 concat으로 해야 함 (요구사항이 고정+변동 주문의 경우)
        newOrder.requiredChipAbilityList = newAbilityList;
        
        newRequestList.AddRange(targetOrder.requiredRequestList);
        newOrder.requiredRequestList = newRequestList;
        
        return newOrder;
    }
}
