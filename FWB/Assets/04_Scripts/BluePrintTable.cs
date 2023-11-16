using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static ChipObj;
using static ChipTable;

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
