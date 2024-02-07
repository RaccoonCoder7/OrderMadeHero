using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 칩의 정보를 저장하는 SO
/// </summary>
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
        public string howToGet;
        public int price;
        public Sprite chipSprite;
        public List<ChipAbility> abilityList = new List<ChipAbility>();
    }

    [System.Serializable]
    public class ChipAbility
    {
        public string abilityKey;
        public int count;
    }
}
