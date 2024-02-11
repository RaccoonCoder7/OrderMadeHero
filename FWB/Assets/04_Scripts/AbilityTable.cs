using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 칩의 능력치에 대한 정보를 저장하는 SO
/// </summary>
[CreateAssetMenu(fileName = "AbilityTable", menuName = "SO/AbilityTable", order = 3)]
public class AbilityTable : ScriptableObject
{
    public List<Ability> abilityList = new List<Ability>();

    [System.Serializable]
    public class Ability
    {
        public string abilityKey;
        public string name;
    }
}