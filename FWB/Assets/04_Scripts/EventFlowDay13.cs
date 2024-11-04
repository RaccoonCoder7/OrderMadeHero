using System.Collections;
using UnityEngine;

/// <summary>
/// Day 13
/// </summary>
public class EventFlowDay13 : EventFlow
{
    private BossBattleManager battleManager;
    private string startDialogueKey;
    private string nextDialogueKey;
    private string finalDialogueKey;
    private bool isHero;

    public override void StartFlow()
    {
        InitializeComponents();
        RegisterEvents();
        DetermineHeroStatus();
        startDialogueKey = isHero ? "Day13_1" : "Day13_2";
        nextDialogueKey = isHero ? "Day13_3" : "Day13_4";
        finalDialogueKey = isHero ? "Day13_5" : "Day13_6";
        mgr.StartText(startDialogueKey, EndDay13_1Routine, EndDay13_1Routine);
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
        mgr.EndText(false);
        mgr.eventBtntext1.text = "가야지.";
        mgr.eventBtntext2.text = "반드시 가야지!";

        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText(nextDialogueKey, EndDay13_2Routine, EndDay13_2Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText(nextDialogueKey, EndDay13_2Routine, EndDay13_2Routine);
        });

        mgr.ActiveEventButton(true);
    }

    private void EndDay13_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }
        battleManager.lastWeekStatus = true;

<<<<<<< Updated upstream
        StartCoroutine(mgr.StartBossRoutine(5, mgr.EndNormalOrderRoutine));
=======
        battleManager.HideInfoForBoss.enabled = true;
        battleManager.lastWeekStatus = true;
        battleManager.DetermineBossAndAlly();
        battleManager.SetBossBattleData();
        mgr.popupPanel.SetActive(true);
        battleManager.SetUIActive(true);

>>>>>>> Stashed changes
    }

    //End Text 
    private void HandleBossBattleEnded(bool success)
    {
        if (success)
        {
            battleManager.lastWeekStatus = false;
            mgr.StartText(finalDialogueKey, EndDay13_3Routine, EndDay13_3Routine);
        }
    }

    private void EndDay13_3Routine()
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