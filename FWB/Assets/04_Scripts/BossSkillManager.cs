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
            var chipSlotImage = chipSlot.GetComponent<Image>();
            if (chipSlotImage != null)
            {
                chipSlotImage.color = Color.black;
            }

            foreach (var image in chipSlot.GetComponentsInChildren<Image>())
            {
                image.color = Color.black;
            }

            foreach (var rawImage in chipSlot.GetComponentsInChildren<RawImage>())
            {
                rawImage.color = Color.black;
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
