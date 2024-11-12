using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Day 20
/// </summary>
public class EventFlowDay20 : EventFlow
{
    private BossBattleManager battleManager;
    private BossCutScene bossCutScene;
    
    private string startDialogueKey;
    private string nextDialogueKey;
    private string finalDialogueKey;
    private bool isHero;

    [SerializeField]
    private GameObject MainPanel;
    public override void StartFlow()
    {
        InitializeComponents();
        RegisterEvents();
        DetermineHeroStatus();
        startDialogueKey = isHero ? "Day20_1" : "Day20_2";
        nextDialogueKey = isHero ? "Day20_3" : "Day20_4";
        finalDialogueKey = isHero ? "Day20_5" : "Day20_6";
        mgr.MobSpriteRandomChange();
        mgr.StartText(startDialogueKey, EndDay20_1Routine, EndDay20_1Routine);
    }


    private void InitializeComponents()
    {
        battleManager = FindObjectOfType<BossBattleManager>();
        bossCutScene = FindObjectOfType<BossCutScene>();
    }


    private void RegisterEvents()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded -= HandleBossBattleEnded;
            battleManager.OnBossBattleEnded += HandleBossBattleEnded;
        }
    }


    private void DetermineHeroStatus()
    {
        isHero = GameMgr.In.tendency >= 0;
    }

    private void EndDay20_1Routine()
    {
        mgr.EndText(false);
        mgr.eventBtntext1.text = "가야지.";
        mgr.eventBtntext2.text = "반드시 가야지!";

        mgr.eventBtn1.onClick.RemoveAllListeners();
        mgr.eventBtn1.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText(nextDialogueKey, EndDay20_2Routine, EndDay20_2Routine);
        });

        mgr.eventBtn2.onClick.RemoveAllListeners();
        mgr.eventBtn2.onClick.AddListener(() =>
        {
            mgr.ActiveEventButton(false);
            mgr.StartText(nextDialogueKey, EndDay20_2Routine, EndDay20_2Routine);
        });

        mgr.ActiveEventButton(true);
    }

    private void EndDay20_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }
        battleManager.lastWeekStatus = true;

        battleManager.HideInfoForBoss.enabled = true;
        battleManager.lastWeekStatus = true;
        battleManager.DetermineBossAndAlly();
        battleManager.SetBossBattleData();
        mgr.popupPanel.SetActive(true);
        battleManager.SetUIActive(true);

    }

    private void HandleBossBattleEnded(bool success)
    {
        if (!success) return;
        MainPanel.gameObject.SetActive(false);
        if (bossCutScene != null)
        {
            if (isHero)
            {
                bossCutScene.StartCutScene();
                bossCutScene.OnCutSceneEnd = () =>
                {
                    MainPanel.gameObject.SetActive(true);
                    mgr.StartText(finalDialogueKey, EndDay20_3Routine, EndDay20_3Routine);
                };
            }
            else
            {
                MainPanel.gameObject.SetActive(true);
                mgr.StartText(finalDialogueKey, EndDay20_3Routine, EndDay20_3Routine);
            }
        }
    }


    private void EndDay20_3Routine()
    {
        battleManager.lastWeekStatus = false;
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        foreach (var image in mgr.imageList)
        {
            image.imageObj.SetActive(false);
        }
        EndFlow();
    }

    void OnDestroy()
    {
        if (battleManager != null)
        {
            battleManager.OnBossBattleEnded -= HandleBossBattleEnded;
        }
    }
}