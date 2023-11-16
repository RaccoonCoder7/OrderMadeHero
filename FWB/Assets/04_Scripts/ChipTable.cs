using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityTable;
using static ChipObj;

[CreateAssetMenu(fileName = "ChipTable", menuName = "SO/ChipTable", order = 2)]
public class ChipTable : ScriptableObject
{
    public List<Chip> chipList = new List<Chip>();

    [System.Serializable]
    public class Chip
    {
        public string chipKey;
        public string chipName;
        public string desc;
        public int price;
        public List<ChipAbility> abilityList = new List<ChipAbility>();
    }

    [System.Serializable]
    public class ChipAbility
    {
        public string abilityKey;
        public int count;
    }
}
