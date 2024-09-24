using System.Collections;
using UnityEngine;

/// <summary>
/// Day 13의 이벤트를 제어하는 클래스
/// </summary>
public class EventFlowDay13 : EventFlow
{
    private BossBattleManager battleManager;
    private GameMgr gameMgr2;
    private string nextDialogueKey;
    private bool isHero;
    public override void StartFlow()
    {
        InitializeComponents();
        RegisterEvents();
        DetermineHeroStatus();
        SetupSpecialBlueprintAndChipset();
        string dialogueKey = isHero ? "Day13_1" : "Day13_2";
        nextDialogueKey = isHero ? "Day13_3" : "Day13_4";
        mgr.StartText(dialogueKey, EndDay13_1Routine, EndDay13_1Routine);
    }

    private void InitializeComponents()
    {
        battleManager = FindObjectOfType<BossBattleManager>();
        if (battleManager != null)
        {
            Debug.Log("할당완료");
        }
        gameMgr2 = GameMgr.In;
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
        isHero = gameMgr2.tendency >= 0;
    }
    private void SetupSpecialBlueprintAndChipset()
    {

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
        battleManager.StartBossBattle();
        StartCoroutine(mgr.StartBossRoutine(5, mgr.EndNormalOrderRoutine));
    }

    private void HandleBossBattleEnded(bool success)
    {
        if (success)
        {
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
