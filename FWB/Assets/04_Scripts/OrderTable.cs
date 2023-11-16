using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChipObj;
using static AbilityTable;

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
                    var bluePrintList = GameMgr.In.bluePrintTable.bluePrintList;
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
        newOrder.requiredChipAbilityList = newAbilityList;
        return newOrder;
    }
}
