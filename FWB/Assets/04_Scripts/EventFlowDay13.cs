using System.Collections;
using UnityEngine;

/// <summary>
/// Day 13의 이벤트를 제어하는 클래스
/// </summary>
public class EventFlowDay13 : EventFlow
{
    private BossBattleManager battleManager;
    private GameMgr gameMgr;
    private string nextDialogueKey;
    private bool isHero;

    void Start()
    {
        InitializeComponents();
        RegisterEvents();
        DetermineHeroStatus();
    }

    private void InitializeComponents()
    {
        battleManager = FindObjectOfType<BossBattleManager>();
        gameMgr = GameMgr.In;
    }

    private void RegisterEvents()
    {
        battleManager.OnBossBattleEnded += HandleBossBattleEnded;
    }

    private void DetermineHeroStatus()
    {
        isHero = gameMgr.tendency >= 0;
    }

    public override void StartFlow()
    {
        string dialogueKey = isHero ? "Day13_1" : "Day13_2";
        nextDialogueKey = isHero ? "Day13_3" : "Day13_4";
        mgr.StartText(dialogueKey, EndDay13_1Routine, EndDay13_1Routine);
    }

    private void EndDay13_1Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        battleManager.lastWeekStatus = true;

        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
    }

    private void HandleBossBattleEnded(bool success)
    {
        if (success)
        {
            battleManager.lastWeekStatus = false;
            mgr.StartText(nextDialogueKey, EndDay13_2Routine, EndDay13_2Routine);
        }
        else
        {
            // 실패시 처리, 현재는 비워둔 상태
        }
    }

    private void EndDay13_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        // 성공 후 루틴으로 넘어가는 추가 로직이 필요하면 여기에 구현
    }

    void OnDestroy()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded -= HandleBossBattleEnded;
        }
    }
}
