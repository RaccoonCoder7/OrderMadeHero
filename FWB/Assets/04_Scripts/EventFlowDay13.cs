using System.Collections;
using UnityEngine;

/// <summary>
/// Day 13
/// </summary>
public class EventFlowDay13 : EventFlow
{
    private BossBattleManager battleManager;
    private string nextDialogueKey;
    private bool isHero;

    public override void StartFlow()
    {
        InitializeComponents();
        RegisterEvents();
        DetermineHeroStatus();
        string dialogueKey = isHero ? "Day13_1" : "Day13_2";
        nextDialogueKey = isHero ? "Day13_3" : "Day13_4";
        mgr.StartText(dialogueKey, EndDay13_1Routine, EndDay13_1Routine);
    }

    private void InitializeComponents()
    {
        battleManager = FindObjectOfType<BossBattleManager>();
    }

    private void RegisterEvents()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded += HandleBossBattleEnded;
        }
    }

    private void DetermineHeroStatus()
    {
        isHero = GameMgr.In.tendency >= 0;
    }


    private void EndDay13_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }
        battleManager.lastWeekStatus = true;

        StartCoroutine(mgr.StartBossRoutine(5, mgr.EndNormalOrderRoutine));
    }

    private void HandleBossBattleEnded(bool success)
    {
        if (success)
        {
            battleManager.lastWeekStatus = false;
            mgr.StartText(nextDialogueKey, EndDay13_2Routine, EndDay13_2Routine);
        }
    }

    private void EndDay13_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }
        battleManager.lastWeekStatus = false;
        GameMgr.In.isEventOn = 0;
        mgr.NextDay();
    }

    void OnDestroy()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded -= HandleBossBattleEnded;
        }
    }
}