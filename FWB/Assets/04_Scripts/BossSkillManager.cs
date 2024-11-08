using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
 * Boss Skill 
 */
public class BossSkillManager : MonoBehaviour
{
    [Header("UI")]
    public BossBattleManager battleManager;

    private Dictionary<int, Dictionary<int, System.Action>> bossSkills;

    private void Awake()
    {
        InitializeSkills();
    }

    /*
     * 1.Puppet
     * 2.Bunny
     */

    private void InitializeSkills()
    {
        bossSkills = new Dictionary<int, Dictionary<int, System.Action>> {
            { 1, new Dictionary<int, System.Action> {
                { 1, () => HideChipsetInfo() },
                { 2, () => InvertScreen() }
            }},
            { 2, new Dictionary<int, System.Action> {
                { 1, () => ReduceTime(15f) }
            }}
        };
    }

    public void ExecuteSkill(int bossId, int skillIndex)
    {
        if (bossSkills.TryGetValue(bossId, out var skills) && skills.TryGetValue(skillIndex, out var skill))
        {
            skill();
        }
    }

    private void InvertScreen()
    {
        battleManager.screenTarget.rotation = Quaternion.Euler(0, 0, 180);
    }

    private void HideChipsetInfo()
    {
        foreach (Transform chipSlot in battleManager.chipsetPanel.transform)
        {
            var slotImage = chipSlot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.color = Color.black;
            }

            if (chipSlot.childCount > 0)
            {
                var firstChild = chipSlot.GetChild(0);
                var rawImage = firstChild.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.color = Color.black;
                }
            }

            if (chipSlot.childCount > 1)
            {
                var secondChild = chipSlot.GetChild(1).gameObject;
                secondChild.SetActive(false);
            }
        }
    }

    private void ReduceTime(float timeReduction)
    {
        battleManager.maxTime -= timeReduction;
        battleManager.timer = Mathf.Min(battleManager.timer, battleManager.maxTime);
        battleManager.UpdateGage();
    }

}
