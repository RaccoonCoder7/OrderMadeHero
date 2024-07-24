using System.Collections.Generic;
using UnityEngine;
using static ChipTable;

/// <summary>
/// 청사진에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "WeaponDataTable", menuName = "SO/WeaponDataTable", order = 4)]
public class WeaponDataTable : ScriptableObject
{
    public List<BluePrintCategory> bluePrintCategoryList = new List<BluePrintCategory>();

    [System.Serializable]
    public class BluePrintCategory
    {
        public string categoryKey;
        public string name;
        public Sprite onSprite;
        public Sprite offSprite;
        public List<BluePrint> bluePrintList = new List<BluePrint>();
    }

    [System.Serializable]
    public class BluePrint
    {
        public string bluePrintKey;
        public string name;
        public string comment;
        public string howToGet;
        public int buyPrice;
        public int sellPrice;
        public TextAsset puzzleCsv;
        public bool orderEnable = false;
        public bool createEnable = false;
        public Sprite blueprintSprite;
        public Sprite icon;
        public List<ChipAbility> requiredChipAbilityList = new List<ChipAbility>();
        public List<string> enableChipKeyList = new List<string>();
    }
}
