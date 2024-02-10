using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestTable", menuName = "SO/RequestTable", order = 4)]
public class ConditionTable : ScriptableObject
{
    public List<Condition> conditionList = new List<Condition>();

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

    public bool IsConditionMatched()
    {
        return false;
    }
}
