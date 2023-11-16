using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChipObj;

[CreateAssetMenu(fileName = "AbilityTable", menuName = "SO/AbilityTable", order = 3)]
public class AbilityTable : ScriptableObject
{
    public List<Ability> abilityList = new List<Ability>();

    [System.Serializable]
    public class Ability
    {
        public string abilityKey;
        public string name;
        public string desc;
        public int value; // TODO: 없어져야함
    }
}