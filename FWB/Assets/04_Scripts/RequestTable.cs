using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestTable", menuName = "SO/RequestTable", order = 4)]
public class RequestTable : ScriptableObject
{
    public List<OrderRequest> requestList = new List<OrderRequest>();

    [System.Serializable]
    public class OrderRequest
    {
        public string requestKey;
        public string requestName;
        public bool weaponMinReq; //무기 최소 요구치
        public float filledChipRatio; //칩셋 칸 채워진 비율 확인 (요구사항1 - 완벽한, 적당한)
        //멋 칩셋 사용 여부, 외곽에 사용 비율 확인 (요구사항1 - 멋있는, 화려한, 눈부신)
        //내구도 칩셋 사용 여부 확인 (요구사항1 - 일회용)
    }
}
