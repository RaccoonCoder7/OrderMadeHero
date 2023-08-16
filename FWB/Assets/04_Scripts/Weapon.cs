using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie Data", menuName = "Scriptable Object/Zombie Data")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public string descript;
    public Texture thumbnail;
    public List<Stat> requiredStatList = new List<Stat>();
}

public class Stat
{
    public StatType statType;
    public int value;
    public Texture thumbnail;
}

public enum StatType
{
    Sharpness,
    SuperPower,
    Durability,
    Fire,
    Ice,
    Electric,
}