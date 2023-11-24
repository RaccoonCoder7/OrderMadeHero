using System.Collections.Generic;
using UnityEngine;
using static ChipTable;

/// <summary>
/// 청사진에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "BluePrintTable", menuName = "SO/BluePrintTable", order = 0)]
public class BluePrintTable : ScriptableObject
{
    public List<BluePrint> bluePrintList = new List<BluePrint>();

    [System.Serializable]
    public class BluePrint
    {
        public string bluePrintKey;
        public string name;
        public List<ChipAbility> requiredChipAbilityList = new List<ChipAbility>();
    }
}
