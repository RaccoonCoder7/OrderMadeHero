using System.Collections;
using UnityEngine;

/// <summary>
/// Day 13�� �̺�Ʈ�� �����ϴ� Ŭ����
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
            // ���н� ó��, ����� ����� ����
        }
    }

    private void EndDay13_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        // ���� �� ��ƾ���� �Ѿ�� �߰� ������ �ʿ��ϸ� ���⿡ ����
    }

    void OnDestroy()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded -= HandleBossBattleEnded;
        }
    }
}
