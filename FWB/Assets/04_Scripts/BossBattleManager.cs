using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
//보스전
public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager instance;
    [Header("Managers")]
    public PuzzleMgr puzzleMgr;
    public GameMgr gameMgr;
    public GameSceneMgr gameSceneMgr;
    public BossSkillManager bossSkillMgr;
    public BossDialogueManager bossDialogueMgr;
    public BossWeaponSetting bossWeaponSettings;

    [Header("UI")]
    public RectTransform gageRectTr;
    public RectTransform gage;
    public Text teamDialogue;
    public Button clearPuzzleButton;
    public Button failPuzzleButton;
    public RectTransform dialogueBox;
    public Image dialogueBoxImage;
    public Canvas gameCanvas;
    public GameObject screenShakeTarget;
    public Transform chipsetPanel;
    public Transform screenTarget;
    private List<Color> originalChipColors;
    private List<Color> originalRawChipColors;
    public Image HideInfoForBoss;

    [Header("Sprites")]
    public Sprite puppetNormalSprite;
    public Sprite puppetCrackedSprite;
    public Sprite puppetBrokenSprite;
    public Sprite bunnyNormalSprite;
    public Sprite bunnyCrackedSprite;
    public Sprite bunnyBrokenSprite;

    [Header("Dialogue Pos")]
    public float dialogueBoxHiddenX = -1350f;
    public float dialogueBoxVisibleX = -590f;

    [Header("Game Setting")]
    public float maxTime = 60f;
    public int maxPuzzleCnt;

    private bool isPuzzleCompleted;
    public bool isGamePlaying;
    private int succeedPuzzleCnt;
    private int failureCount;
    public float timer;
    private int currentPuzzleIndex;
    private bool isHero;
    private float initialGageWidth;
    private Vector2 initialGagePos;
    private string currentBossKey;

    public bool lastWeekStatus = false;
    private bool isBossBattleActive = false;
    private bool isGameCanvasActive;
    private int bossIndex;
    private string bossname;
    public delegate void BossBattleResult(bool success);
    public event BossBattleResult OnBossBattleEnded;

    public static BossBattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BossBattleManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        Initialize();
        HideInfoForBoss.enabled = false;
        isGameCanvasActive = gameCanvas.enabled;
        // SetUIActive(lastWeekStatus);
        if (lastWeekStatus && isGameCanvasActive)
        {
            StartCoroutine(StartBossBattle());
        }
    }

    private void Update()
    {
        if (lastWeekStatus && !isBossBattleActive)
        {
            SetUIActive(lastWeekStatus);
            if (lastWeekStatus && isGameCanvasActive)
            {
                StartCoroutine(StartBossBattle());
                isBossBattleActive = true;
                HideInfoForBoss.enabled = true;
            }
            else
            {
                StopAllCoroutines();
                ResetGameState();
                isBossBattleActive = false;
                HideInfoForBoss.enabled = false;
            }
        }

        if (gameCanvas.enabled != isGameCanvasActive)
        {
            isGameCanvasActive = gameCanvas.enabled;
            if (isGameCanvasActive && lastWeekStatus)
            {
                SetUIActive(true);
                StartCoroutine(StartBossBattle());
                isBossBattleActive = true;
            }
        }

        if (isGamePlaying && isGameCanvasActive && lastWeekStatus)
        {
            UpdateTimer();
        }
    }

    private void Initialize()
    {
        initialGageWidth = gageRectTr.sizeDelta.x;
        initialGagePos = gageRectTr.anchoredPosition;
        SaveOriginalChipColors();

        clearPuzzleButton.onClick.RemoveAllListeners();
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(2));

        failPuzzleButton.onClick.RemoveAllListeners();
        failPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(1));
    }

    private void SetUIActive(bool isActive)
    {
        gage.gameObject.SetActive(isActive);
        clearPuzzleButton.gameObject.SetActive(isActive);
        failPuzzleButton.gameObject.SetActive(isActive);
        dialogueBox.gameObject.SetActive(isActive);
    }

    private void DetermineBossAndAlly()
    {
        if (gameMgr.tendency >= 0)
        {
            isHero = true;
            dialogueBoxImage.sprite = bunnyNormalSprite;
            bossname = "bunny";
        }
        else
        {
            isHero = false;
            dialogueBoxImage.sprite = puppetNormalSprite;
            bossname = "puppet";
        }
        bossIndex = isHero ? 1 : 2;

        SetTableDatasForBossMode(bossname);
    }
    
    private IEnumerator StartBossBattle()
    {
        ResetGameState();
        DetermineBossAndAlly();
        WeaponSetting(bossname, 0);

        puzzleMgr.isFeverMode = false;
        puzzleMgr.isTutorial = false;
        isGamePlaying = true;

        puzzleMgr.OnMakingDone += OnBossBattleMakingDone;

        yield return bossDialogueMgr.ShowDialogue("chapter1",bossIndex,0,"start");

        while (currentPuzzleIndex < maxPuzzleCnt)
        {
            isPuzzleCompleted = false;
            yield return new WaitUntil(() => isPuzzleCompleted);
        }

        EndBossBattle(true);
    }

    private void WeaponSetting(string bossKey, int weaponIdx)
    {
        var bossWeapon = bossWeaponSettings.bossWeapons.Find(b => b.bossKey == bossKey);
        if (bossWeapon != null)
        {
            if (weaponIdx >= 0 && weaponIdx < bossWeapon.weaponKeys.Count)
            {
                string weaponKey = bossWeapon.weaponKeys[weaponIdx];
                GameMgr.In.currentBluePrint = GameMgr.In.GetWeapon("t_special", weaponKey);

                gameSceneMgr.SetBossWeapon(weaponKey);
                Debug.Log("Key Setting: " + bossKey + ", " + weaponIdx + " with key: " + weaponKey);
                Debug.Log("Current Blueprint set to: " + GameMgr.In.currentBluePrint.name);
            }
            else
            {
                Debug.LogError("Invalid weaponIndex: " + weaponIdx + " for bossKey: " + bossKey);
            }
        }
    }

    private void SetTableDatasForBossMode(string bossname)
    {
        foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in bpc.bluePrintList)
            {
                bp.createEnable = false;
            }
        }

        foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
        {
            if (bpc.categoryKey.Equals("t_special"))
            {
                foreach (var bp in bpc.bluePrintList)
                {
                    if (bossWeaponSettings.bossWeapons.Any(bw => bw.bossKey == bossname && bw.weaponKeys.Contains(bp.bluePrintKey)))
                    {
                        bp.createEnable = true;
                    }
                }
            }
        }
    }

    private void ApplyBossGimmick()
    {
        if (isHero)
        {
            if (currentPuzzleIndex == 1)
            {
                bossSkillMgr.ExecuteSkill(1, 1);
            }
            else if (currentPuzzleIndex == 2)
            {
                bossSkillMgr.ExecuteSkill(1, 2);
            }
        }
        else
        {
            if (currentPuzzleIndex == 1)
            {
                bossSkillMgr.ExecuteSkill(2, 1);
            }
            else if (currentPuzzleIndex == 2)
            {
                bossSkillMgr.ExecuteSkill(2, 1);
            }
            UpdateGage();
        }
    }

    private void OnBossBattleMakingDone(int result)
    {
        if (!isGamePlaying) return;
        string resultKey = result == 2 ? "success" : "fail";

        StartCoroutine(HandleDialogueAndContinue(bossIndex, resultKey, result));
    }

    private IEnumerator HandleDialogueAndContinue(int bossIndex, string resultKey, int result)
    {

        if (result == 2)
        {
            succeedPuzzleCnt++;
            ScreenShake();
        }
        else
        {
            failureCount++;
            UpdateCharacterPopup();
            if (failureCount >= 3)
            {
                EndBossBattle(false);
                yield break;
            }
        }
        yield return bossDialogueMgr.ShowDialogue("chapter1", bossIndex, currentPuzzleIndex + 1, resultKey);

        isPuzzleCompleted = true;
        currentPuzzleIndex++;
        WeaponSetting(bossname, currentPuzzleIndex);
        ApplyBossGimmick();

        if (currentPuzzleIndex < maxPuzzleCnt)
        {
            yield return bossDialogueMgr.ShowDialogue("chapter1", bossIndex, currentPuzzleIndex+1, "start");
        }
    }

    private void UpdateCharacterPopup(bool reset = false)
    {
        if (isHero)
        {
            if (reset)
            {
                dialogueBoxImage.sprite = bunnyNormalSprite;
            }
            else
            {
                switch (failureCount)
                {
                    case 1:
                        dialogueBoxImage.sprite = bunnyCrackedSprite;
                        break;
                    case 2:
                        dialogueBoxImage.sprite = bunnyBrokenSprite;
                        break;
                    default:
                        dialogueBoxImage.sprite = bunnyNormalSprite;
                        break;
                }
            }
        }
        else 
        {
            if (reset)
            {
                dialogueBoxImage.sprite = puppetNormalSprite; 
            }
            else
            {
                switch (failureCount)
                {
                    case 1:
                        dialogueBoxImage.sprite = puppetCrackedSprite; 
                        break;
                    case 2:
                        dialogueBoxImage.sprite = puppetBrokenSprite; 
                        break;
                    default:
                        dialogueBoxImage.sprite = puppetNormalSprite; 
                        break;
                }
            }
        }
    }


    private void ProcessPuzzleResult(int result)
    {
        if (!isGamePlaying) return;
        OnBossBattleMakingDone(result);
    }

    private void EndBossBattle(bool success)
    {
        isGamePlaying = false;

        if (success)
        {
            CommonTool.In.OpenAlertPanel("보스전 성공!");
            gameCanvas.gameObject.SetActive(false);
        }
        else
        {
            CommonTool.In.OpenAlertPanel("보스전 실패...");
        }

        ResetGameState();

        clearPuzzleButton.onClick.RemoveAllListeners();
        failPuzzleButton.onClick.RemoveAllListeners();

        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(2));
        failPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(1));

        if (!success)
        {
            StartCoroutine(StartBossBattle());
        }

        OnBossBattleEnded?.Invoke(success);
    }

    private void ResetGameState()
    {
        isGamePlaying = false;
        timer = maxTime;
        failureCount = 0;
        succeedPuzzleCnt = 0;
        currentPuzzleIndex = 0;
        isPuzzleCompleted = false;

        RestoreOriginalChipColors();
        UpdateGage();
        UpdateCharacterPopup(true);

        clearPuzzleButton.onClick.RemoveAllListeners();
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(2));
        failPuzzleButton.onClick.RemoveAllListeners();
        failPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(1));
    }


    private void UpdateTimer()
    {
        if (!isGameCanvasActive && !lastWeekStatus) return;

        timer -= Time.deltaTime;
        UpdateGage();
        if (timer <= 0)
        {
            EndBossBattle(false);
        }
    }

    public void UpdateGage()
    {
        float gageWidth = initialGageWidth * (timer / maxTime);
        Vector2 sizeDelta = gageRectTr.sizeDelta;
        sizeDelta.x = gageWidth;
        gageRectTr.sizeDelta = sizeDelta;
        gageRectTr.anchoredPosition = new Vector2(initialGagePos.x - (initialGageWidth - gageWidth) / 2, initialGagePos.y);
    }

    public void ToggleCanvasInteractable(bool isInteractable)
    {
        foreach (var button in gameCanvas.GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractable;
        }
    }

    public void ShowDialogueBox()
    {
        dialogueBox.DOLocalMoveX(dialogueBoxVisibleX, 1f).SetEase(Ease.OutQuad);
    }

    public void HideDialogueBox()
    {
        dialogueBox.DOLocalMoveX(dialogueBoxHiddenX, 1f).SetEase(Ease.InQuad);
    }

    private void SaveOriginalChipColors()
    {
        originalChipColors = new List<Color>();
        originalRawChipColors = new List<Color>();

        var images = chipsetPanel.GetComponentsInChildren<Image>();
        var rawImages = chipsetPanel.GetComponentsInChildren<RawImage>();

        foreach (var image in images)
        {
            originalChipColors.Add(image.color);
        }

        foreach (var rawImage in rawImages)
        {
            originalRawChipColors.Add(rawImage.color);
        }
    }

    private void RestoreOriginalChipColors()
    {
        var images = chipsetPanel.GetComponentsInChildren<Image>();
        var rawImages = chipsetPanel.GetComponentsInChildren<RawImage>();

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalChipColors[i];
        }

        for (int i = 0; i < rawImages.Length; i++)
        {
            rawImages[i].color = originalRawChipColors[i];
        }
    }

    private void ScreenShake()
    {
        screenShakeTarget.transform.DOShakePosition(1, new Vector3(20, 20, 0), 20, 90, false, true);
    }

    private void ResetScreen()
    {
        screenTarget.rotation = Quaternion.Euler(0, 0, 0);
    }
}
