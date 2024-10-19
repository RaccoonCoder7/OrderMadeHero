using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ������ �����ϴ� SO
/// </summary>
[CreateAssetMenu(fileName = "BossWeaponSetting", menuName = "SO/BossWeaponSetting", order = 1)]
public class BossWeaponSetting : ScriptableObject
{
    [System.Serializable]
    public class BossWeapon
    {
        public string bossKey; 
        public List<string> weaponKeys = new List<string>();
    }

    public List<BossWeapon> bossWeapons = new List<BossWeapon>();
}
