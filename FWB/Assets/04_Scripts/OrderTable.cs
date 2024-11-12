using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static WeaponDataTable;
using System;

/// <summary>
/// 주문에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "OrderTable", menuName = "SO/OrderTable", order = 1)]
public class OrderTable : ScriptableObject
{
    public List<Order> orderList = new List<Order>();
    public List<FeverModeOrder> feverModeOrderList = new List<FeverModeOrder>();
    public List<MappingText> mappingTextList = new List<MappingText>();

    [System.Serializable]
    public class Order
    {
        public string orderKey;
        public TextAsset ta;
        public List<string> requiredBlueprintKeyList = new List<string>();
        public List<RequiredAbility> requiredAbilityList = new List<RequiredAbility>();
        public Condition condition = Condition.상태없음;
        public Gimmick gimmick = Gimmick.None;
        public Camp camp = Camp.None;
        public StringBool orderConditionDictionary = new StringBool();
        public bool orderEnable = true;
        [HideInInspector]
        public List<string> addedRequestKeyList = new List<string>();
    }

    [System.Serializable]
    public class FeverModeOrder
    {
        public string orderKey;
        public TextAsset ta;
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
            을,
            이,
            면,
            로,
        }
    }

    public class MatchingData
    {
        public string key;
        public string value;
        public bool isNoun;
        public int index;
        public int usedCnt;

        public MatchingData(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }


    [System.Serializable]
    public enum Condition
    {
        상태없음,
        완벽한,
        적당한,
        허술한,
        // 멋있는,
        // 화려한,
        // 눈부신,
        조악한
    }

    [System.Serializable]
    public enum Gimmick
    {
        None,
        NotSoldToday,
        SoldToday,
        MostSoldToday,
        NotHeavy,
        HighestAttack,
        PreviousOrder,
        SatisfyOneRequest,
    }

    [System.Serializable]
    public enum Camp
    {
        None,
        Hero,
        Villain
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

        newOrder.condition = targetOrder.condition;
        newOrder.gimmick = targetOrder.gimmick;
        newOrder.camp = targetOrder.camp;

        foreach (var requestKey in targetOrder.addedRequestKeyList)
        {
            newOrder.addedRequestKeyList.Add(string.Copy(requestKey));
        }

        // 기믹 파싱
        var matchingDataList = new List<MatchingData>();
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
            var matchedData = matchingDataList.Find(x => x.key.Equals(key));
            if (matchedData == null)
            {
                matchingDataList.Add(new MatchingData(key, string.Empty));
            }
            i = tempIndex;
        }

        for (int i = 0; i < matchingDataList.Count; i++)
        {
            var mappingData = mappingTextList.Find(x => matchingDataList[i].key.Contains(x.mappingKey));
            if (mappingData == null)
            {
                Debug.Log("매칭되는 데이터가 없음: " + matchingDataList[i].key);
                return null;
            }

            switch (mappingData.mappingType)
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
                        int cnt = 0;
                        var matchedData = matchingDataList.Find(x => x.value.Equals(bluePrint.name));
                        if (matchedData != null)
                        {
                            cnt++;
                            if (cnt > 100) break;
                            continue;
                        }
                        matchingDataList[i].value = bluePrint.name;
                        matchingDataList[i].isNoun = true;
                        matchingDataList[i].index = i;
                        newOrder.requiredBlueprintKeyList.Add(bluePrint.bluePrintKey);
                        break;
                    }
                    break;
                case MappingText.MappingType.Request:
                    var requestList = GameMgr.In.requestTable.requestList;
                    var orderableRequestList = requestList.Where(x => x.orderEnable).ToList();
                    while (true)
                    {
                        var request = orderableRequestList[UnityEngine.Random.Range(0, orderableRequestList.Count)];
                        var matchedData = matchingDataList.Find(x => x.value.Equals(request.name));
                        int cnt = 0;
                        if (matchedData != null)
                        {
                            cnt++;
                            if (cnt > 100) break;
                            continue;
                        }
                        matchingDataList[i].value = request.name;
                        newOrder.addedRequestKeyList.Add(request.requestKey);

                        foreach (var ability in request.requiredAbilityList)
                        {
                            var existAbility = newOrder.requiredAbilityList.Find(x => x.abilityKey.Equals(ability.abilityKey));
                            if (existAbility != null)
                            {
                                existAbility.count += ability.count;
                                continue;
                            }

                            var ra = new RequiredAbility();
                            ra.abilityKey = ability.abilityKey;
                            ra.count = ability.count;
                            newOrder.requiredAbilityList.Add(ra);
                        }
                        break;
                    }
                    break;
                case MappingText.MappingType.Condition:
                    var conditionNum = UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(Condition)).Length);
                    var condition = (Condition)conditionNum;
                    newOrder.condition = condition;
                    matchingDataList[i].value = condition.ToString();
                    break;
                case MappingText.MappingType.이:
                    if (i == 0)
                    {
                        matchingDataList[i].value = "이";
                        break;
                    }
                    var blueprintName1 = GetMatchedBlueprintName(matchingDataList);
                    matchingDataList[i].value = IsExistLastSyllable(blueprintName1) ? "이" : "가";
                    break;
                case MappingText.MappingType.을:
                    if (i == 0)
                    {
                        matchingDataList[i].value = "을";
                        break;
                    }
                    var blueprintName2 = GetMatchedBlueprintName(matchingDataList);
                    matchingDataList[i].value = IsExistLastSyllable(blueprintName2) ? "을" : "를";
                    break;
                case MappingText.MappingType.면:
                    if (i == 0)
                    {
                        matchingDataList[i].value = "이면";
                        break;
                    }
                    var blueprintName3 = GetMatchedBlueprintName(matchingDataList);
                    matchingDataList[i].value = IsExistLastSyllable(blueprintName3) ? "이면" : "면";
                    break;
                case MappingText.MappingType.로:
                    if (i == 0)
                    {
                        matchingDataList[i].value = "으로";
                        break;
                    }
                    var blueprintName4 = GetMatchedBlueprintName(matchingDataList);
                    matchingDataList[i].value = IsExistLastSyllable(blueprintName4) ? "으로" : "로";
                    break;
                default:
                    break;
            }

            text = text.Replace(matchingDataList[i].key, matchingDataList[i].value);
        }
        newOrder.ta = new TextAsset(text);
        return newOrder;
    }

    public bool IsConditionMatched(int frameCnt, int totalSizeOfChips, Dictionary<Ability, int> abilityDic, Condition condition)
    {
        if (condition == Condition.상태없음 || condition == Condition.허술한)
        {
            return true;
        }

        switch (condition)
        {
            case Condition.완벽한:
                return totalSizeOfChips == frameCnt;
            case Condition.적당한:
                return totalSizeOfChips >= frameCnt / 2;
            case Condition.조악한:
                var durability = abilityDic.FirstOrDefault(x => x.Key.abilityKey.Equals("a_durability"));
                // TODO: 청사진 필수 내구도를 제외하는 로직 추가 (할 필요 있는지?)
                return durability.Key == null && durability.Value == 0;
        }
        return false;
    }

    public bool IsGimmickMatched(List<PuzzleFrame> puzzleFrameList, Dictionary<Ability, int> currentAbilityInPuzzleDic)
    {
        var order = GameMgr.In.currentOrder;
        var key = GameMgr.In.currentBluePrint.bluePrintKey;
        var history = GameMgr.In.orderedBluePrintKeyList;
        switch (order.gimmick)
        {
            case Gimmick.None:
                // Debug.Log("기믹이 없음");
                return true;
            case Gimmick.NotSoldToday:
                // Debug.Log("오늘 안팔렸나?: " + string.IsNullOrEmpty(history.Find(x => x.Equals(key))));
                return string.IsNullOrEmpty(history.Find(x => x.Equals(key)));
            case Gimmick.SoldToday:
                // Debug.Log("오늘 팔렸나?: " + !string.IsNullOrEmpty(history.Find(x => x.Equals(key))));
                return !string.IsNullOrEmpty(history.Find(x => x.Equals(key)));
            case Gimmick.MostSoldToday:
                // var most = history.GroupBy(x => x).OrderByDescending(grp => grp.Count());
                var dict = history.ToLookup(x => x);
                var maxCount = dict.Max(x => x.Count());
                var mostList = dict.Where(x => x.Count() == maxCount).Select(x => x.Key);
                bool result = mostList.Contains(key);
                // Debug.Log("오늘 젤 많이 팔렸나?: " + result);
                return result;
            case Gimmick.NotHeavy:
                // Debug.Log("무게가 2 이하인가?: " + (GetAbilityCount(puzzleFrameList, "a_weight") <= 2));
                return GetAbilityCount(puzzleFrameList, "a_weight") <= 2;
            case Gimmick.HighestAttack:
                int maxAttack = 0;
                List<string> bluePrintKeyList = new List<string>();
                foreach (var category in GameMgr.In.weaponDataTable.bluePrintCategoryList)
                {
                    foreach (var bp in category.bluePrintList)
                    {
                        if (!bp.createEnable) continue;
                        var attackAbility = bp.requiredChipAbilityList.Find(x => x.abilityKey.Equals("a_attack"));
                        if (attackAbility == null) continue;
                        if (attackAbility.count < maxAttack) continue;
                        if (attackAbility.count > maxAttack)
                        {
                            maxAttack = attackAbility.count;
                            bluePrintKeyList.Clear();
                        }
                        bluePrintKeyList.Add(bp.bluePrintKey);
                    }
                }
                // Debug.Log("가진 청사진 중 가장 강한가?: " + bluePrintKeyList.Contains(key));
                return bluePrintKeyList.Contains(key);
            case Gimmick.PreviousOrder:
                // Debug.Log("바로 전 사람의 주문과 같은가?: " + history[history.Count - 1] == key);
                return history[history.Count - 1] == key;
            case Gimmick.SatisfyOneRequest:
                for (int i = 0; i < GameMgr.In.currentOrder.addedRequestKeyList.Count; i++)
                {
                    List<RequiredAbility> requiredAbilityList = new List<RequiredAbility>();
                    var removeTargetRequestKey = GameMgr.In.currentOrder.addedRequestKeyList[i];
                    var removeTargetRequest = GameMgr.In.GetRequest(removeTargetRequestKey);

                    foreach (var ability in GameMgr.In.currentOrder.requiredAbilityList)
                    {
                        int abilityCount = ability.count;
                        var matchedAbility = removeTargetRequest.requiredAbilityList.Find(x => x.abilityKey.Equals(ability.abilityKey));
                        if (matchedAbility != null)
                        {
                            abilityCount -= matchedAbility.count;
                        }

                        if (abilityCount > 0)
                        {
                            var requiredAbility = new RequiredAbility();
                            requiredAbility.abilityKey = ability.abilityKey;
                            requiredAbility.count = abilityCount;
                            requiredAbilityList.Add(requiredAbility);
                            // Debug.Log("요구: " + ability.abilityKey + "=>" + abilityCount);
                        }
                    }

                    bool isSatisfied = IsRequestSatisfied(requiredAbilityList, currentAbilityInPuzzleDic);
                    // Debug.Log((i + 1) + "번째 조건을 만족하는가?: " + isSatisfied);
                    if (isSatisfied)
                    {
                        return true;
                    }
                }
                return false;
        }
        return false;
    }

    private bool IsRequestSatisfied(List<RequiredAbility> requiredAbilityList, Dictionary<Ability, int> currentAbilityInPuzzleDic)
    {
        foreach (var requiredAbility in requiredAbilityList)
        {
            var targetAbility = currentAbilityInPuzzleDic.FirstOrDefault(x => x.Key.abilityKey.Equals(requiredAbility.abilityKey));
            if (targetAbility.Equals(default(KeyValuePair<Ability, int>)))
            {
                Debug.Log("능력치 요구조건 불충족");
                return false;
            }

            if (targetAbility.Value < requiredAbility.count)
            {
                Debug.Log("능력치 요구조건 불충족");
                return false;
            }
        }
        return true;
    }

    private int GetAbilityCount(List<PuzzleFrame> puzzleFrameList, string abilityKey)
    {
        int count = 0;
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
                        foreach (var ability in chip.chipAbilityList)
                        {
                            if (ability.abilityKey.Equals(abilityKey))
                            {
                                count += ability.count;
                            }
                        }
                    }
                }
            }
        }
        return count;
    }

    private string GetMatchedBlueprintName(List<MatchingData> matchingDataList)
    {
        var nounList = matchingDataList.FindAll(x => x.isNoun);
        if (nounList.Count == 0)
        {
            Debug.Log("명사가 없음");
            return "없음";
        }
        else if (nounList.Count == 1)
        {
            return nounList[0].value;
        }
        else
        {
            var orderedList = nounList.OrderBy(x => x.usedCnt).ThenBy(x => x.index).ToList();
            orderedList[0].usedCnt++;
            return orderedList[0].value;
        }
    }

    private bool IsExistLastSyllable(string word)
    {
        char c = word[word.Length - 1];
        int n = Convert.ToInt32(c);

        if (n < 0xAC00 || n > 0xD7A3)
        {
            throw new Exception("한글이 아님");                
        }

        if((n - 0xAC00) % 28 > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
