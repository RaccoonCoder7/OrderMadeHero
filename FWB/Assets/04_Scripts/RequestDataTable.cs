using System.Collections.Generic;
using UnityEngine;
using static ChipTable;
using static OrderTable;

/// <summary>
/// 요청사항에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "RequestDataTable", menuName = "SO/RequestDataTable", order = 5)]
public class RequestDataTable : ScriptableObject
{
    public List<Request> requestList = new List<Request>();

    [System.Serializable]
    public class Request
    {
        public string requestKey;
        public string name;
        public string orderCondition;
        public bool orderEnable = true;
        public List<RequiredAbility> requiredAbilityList = new List<RequiredAbility>();
    }
}
