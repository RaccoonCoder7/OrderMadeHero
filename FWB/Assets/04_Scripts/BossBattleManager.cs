using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using System.Reflection;
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
    //public Button clearPuzzleButton;
    //public Button failPuzzleButton;
    public RectTransform TeamdialogueBox;
    public Image TeamdialogueBoxImage;
    public RectTransform BossdialogueBox;
    public Image BossdialogueBoxImage;
    public Canvas gameCanvas;
    public GameObject screenShakeTarget;
    public Transform chipsetPanel;
    public Transform screenTarget;
    private List<Color> originalChipColors;
    private List<Color> originalRawChipColors;
    public Image HideInfoForBoss;
    public Button LeftButton;
    public Button RightButton;

    [Header("Sprites")]
    public Sprite puppetNormalSprite;
    public Sprite puppetCrackedSprite;
    public Sprite puppetBrokenSprite;
    public Sprite bunnyNormalSprite;
    public Sprite bunnyCrackedSprite;
    public Sprite bunnyBrokenSprite;

    [Header("Dialogue Pos")]
    public float TeamdialogueBoxHiddenX = -485f;
    public float TeamdialogueBoxVisibleX = 320f;
    public float BossdialogueBoxHiddenX = -485f;
    public float BossdialogueBoxVisibleX = 320f;

    [Header("Game Setting")]
    public float maxTime = 60f;
    public int maxPuzzleCnt;

    private bool isPuzzleCompleted;
    public bool isGamePlaying;
    private int succeedPuzzleCnt;
    private int failureCount = 0;
    public float timer;
    private int currentPuzzleIndex;
    private bool isHero;
    private float initialGageWidth;
    private Vector2 initialGagePos;
    private string currentBossKey;

    public bool lastWeekStatus = false;
    private bool isGameCanvasActive;
    public int bossIndex;
    private string bossname;
    public delegate void BossBattleResult(bool success);
    public event BossBattleResult OnBossBattleEnded;
    public string weaponKey;
    private int TeamHP = 3;
    private int BossHP = 3;

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
    private string resultKey;
    private void Start()
    {
        Initialize();
        HideInfoForBoss.enabled = false;
        isGameCanvasActive = gameCanvas.enabled;
        SaveOriginalChipColors();
    }

    private void Update()
    {
        if (isGamePlaying)
        {
            UpdateTimer();
        }
    }

    private void Initialize()
    {
        initialGageWidth = gageRectTr.sizeDelta.x;
        initialGagePos = gageRectTr.anchoredPosition;

        LeftButton.gameObject.SetActive(false);
        RightButton.gameObject.SetActive(false);
    }

    public void SetUIActive(bool isActive)
    {
        gage.gameObject.SetActive(isActive);
        /*
        clearPuzzleButton.gameObject.SetActive(isActive);
        failPuzzleButton.gameObject.SetActive(isActive);
        */
        TeamdialogueBox.gameObject.SetActive(isActive);
        BossdialogueBox.gameObject.SetActive(isActive);
    }

    public void DetermineBossAndAlly()
    {
        if (gameMgr.tendency >= 0)
        {
            isHero = true;
            TeamdialogueBoxImage.sprite = bunnyNormalSprite;
            BossdialogueBoxImage.sprite = puppetNormalSprite;
            bossname = "bunny";
        }
        else
        {
            isHero = false;
            TeamdialogueBoxImage.sprite = puppetNormalSprite;
            BossdialogueBoxImage.sprite = bunnyNormalSprite;
            bossname = "puppet";
        }
        bossIndex = isHero ? 1 : 2;

        SetTableDatasForBossMode(bossname);
    }

    public void SetBossBattleData()
    {
        ResetGameState();
        WeaponSetting(bossname, 0);

        puzzleMgr.isFeverMode = false;
        puzzleMgr.isTutorial = false;

        puzzleMgr.OnMakingDone += OnBossBattleMakingDone;
    }

    public IEnumerator StartBossBattle()
    {
        ResetTimer();
        if (currentPuzzleIndex == 0)
        {
            yield return bossDialogueMgr.ShowDialogue("chapter1", bossIndex, 0, "start");
        }
        else
        {
            if (currentPuzzleIndex < maxPuzzleCnt)
            {
                yield return bossDialogueMgr.ShowDialogue("chapter1", bossIndex, currentPuzzleIndex + 1, "start");
            }
        }

        isGamePlaying = true;

        while (currentPuzzleIndex < maxPuzzleCnt)
        {
            puzzleMgr.OnMakingDone -= OnBossBattleMakingDone;
            puzzleMgr.OnMakingDone += OnBossBattleMakingDone;

            isPuzzleCompleted = false;
            while (!isPuzzleCompleted)
            {
                if (!isGamePlaying)
                {
                    yield break;
                }
                yield return null;
            }
        }
    }

    private void WeaponSetting(string bossKey, int weaponIdx)
    {
        var bossWeapon = bossWeaponSettings.bossWeapons.Find(b => b.bossKey == bossKey);
        if (bossWeapon != null)
        {
            if (weaponIdx >= 0 && weaponIdx < bossWeapon.weaponKeys.Count)
            {
                weaponKey = bossWeapon.weaponKeys[weaponIdx];
                gameSceneMgr.SetBossWeapon(weaponKey);

                /*테스트용 코드
                var chipList = GameMgr.In.chipTable.chipList;
                foreach (var chip in chipList)
                {
                    chip.createEnable = true;
                }
                */
                BossChipSetActive();
                Debug.Log("Key Setting: " + bossKey + ", " + weaponIdx + " with key: " + weaponKey);
                Debug.Log("Current Blueprint set to: " + GameMgr.In.currentBluePrint.name);
            }
            else
            {
                Debug.LogError("Invalid weaponIndex: " + weaponIdx + " for bossKey: " + bossKey);
            }
        }
        else
        {
            Debug.LogError("BossWeapon not found for bossKey: " + bossKey);
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

    private void BossChipSetActive()
    {
        string chipKeyFirstRound = isHero ? "퍼펫보스전_1차전" : "버니보스전_1차전";
        var firstRoundChip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals(chipKeyFirstRound));
        if (firstRoundChip != null)
        {
            firstRoundChip.createEnable = true;
        }

        if (currentPuzzleIndex == 2)
        {
            string chipKeyThirdRound = isHero ? "퍼펫보스전_3차전" : "버니보스전_3차전";
            var thirdRoundChip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals(chipKeyThirdRound));
            if (thirdRoundChip != null)
            {
                thirdRoundChip.createEnable = true;
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
        isPuzzleCompleted = true;
        isGamePlaying = false;

        switch (result)
        {
            case 1:
                resultKey = "fail";
                failureCount += 2;
                TeamHP -= 2;
                break;
            case 2:
                resultKey = "success";
                succeedPuzzleCnt++;
                failureCount += 1;
                TeamHP -= 1;
                break;
            case 3:
                resultKey = "greatSuccess";
                succeedPuzzleCnt++;
                break;
            default:
                break;
        }
        UpdateCharacterPopup(result,false);
        StartCoroutine(HandleDialogueAndContinue(bossIndex, resultKey, result));
    }
    private IEnumerator HandleDialogueAndContinue(int bossIndex, string resultKey, int result)
    {
        if (result == 2 || result == 3)
        {
            ScreenShake();
            if (currentPuzzleIndex == maxPuzzleCnt - 1)
            {
                EndBossBattle(true);
                yield break;
            }
        }
        else
        {
            if (failureCount >= 3)
            {
                EndBossBattle(false);
                yield break;
            }
        }

        yield return bossDialogueMgr.ShowDialogueAndContinue("chapter1", bossIndex, currentPuzzleIndex + 1, resultKey, () =>
        {
            isPuzzleCompleted = true;
            currentPuzzleIndex++;

            if (currentPuzzleIndex < maxPuzzleCnt)
            {
                WeaponSetting(bossname, currentPuzzleIndex);
                gameSceneMgr.gamePanel.SetActive(false);
                gameSceneMgr.popupPanel.SetActive(true);

                ApplyBossGimmick();
            }
            else
            {
                EndBossBattle(true);
            }
        });
    }


    private void UpdateCharacterPopup(int result, bool reset = false)
    {

        if (reset)
        {
            TeamdialogueBoxImage.sprite = isHero ? bunnyNormalSprite : puppetNormalSprite;
            BossdialogueBoxImage.sprite = isHero ? puppetNormalSprite : bunnyNormalSprite;
            return;
        }

        TeamdialogueBoxImage.sprite = ChangeTeamSprite();

        if (result == 2 || result == 3)
        {
            BossHP--;
        }

        BossdialogueBoxImage.sprite = ChangeBossSprite();
    }

    private Sprite ChangeTeamSprite()
    {
        if (TeamHP == 3)
        {
            return isHero ? bunnyNormalSprite : puppetNormalSprite;
        }
        else if (TeamHP == 2)
        {
            return isHero ? bunnyCrackedSprite : puppetCrackedSprite;
        }
        else
        {
            return isHero ? bunnyBrokenSprite : puppetBrokenSprite;
        }
    }

    private Sprite ChangeBossSprite()
    {
        if (BossHP == 3)
        {
            return isHero ? puppetNormalSprite : bunnyNormalSprite;
        }
        else if (BossHP == 2)
        {
            return isHero ? puppetCrackedSprite : bunnyCrackedSprite;
        }
        else
        {
            return isHero ? puppetBrokenSprite : bunnyBrokenSprite;
        }
    }


    /* Test Code
    private void ProcessPuzzleResult(int result)
    {
        if (!isGamePlaying) return;
        puzzleMgr.ClearPuzzle();
        OnBossBattleMakingDone(result);
    }
    */

    private void EndBossBattle(bool success)
    {
        isGamePlaying = false;

        if (success)
        {
            CommonTool.In.OpenAlertPanel("보스전 성공!", () => {
                gameCanvas.gameObject.SetActive(false);
                OnBossBattleEnded?.Invoke(success);
            });
        }
        else
        {
            CommonTool.In.OpenAlertPanel("보스전 실패...", () => {
                ResetGameState();
                StartCoroutine(StartBossBattle());
                puzzleMgr.StartPuzzle();
                OnBossBattleEnded?.Invoke(success);
            });
        }
    }


    private void ResetGameState()
    {
        isGamePlaying = false;
        timer = maxTime;
        failureCount = 0;
        succeedPuzzleCnt = 0;
        currentPuzzleIndex = 0;
        isPuzzleCompleted = false;
        BossHP = 3;
        TeamHP = 3;

        ResetChipsActivation();
        puzzleMgr.ClearPuzzle();
        WeaponSetting(bossname, 0);
        ResetScreen();
        RestoreOriginalChipColors();

        puzzleMgr.OnMakingDone -= OnBossBattleMakingDone;
        puzzleMgr.OnMakingDone += OnBossBattleMakingDone;

        UpdateGage();
        UpdateCharacterPopup(-1, true);

        /*
        clearPuzzleButton.onClick.RemoveAllListeners();
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(3));
        failPuzzleButton.onClick.RemoveAllListeners();
        failPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(2));
        */
    }

    private void ResetChipsActivation()
    {
        string[] chipKeys = new string[] {
        "퍼펫보스전_1차전","퍼펫보스전_3차전","버니보스전_1차전","버니보스전_3차전"
    };

        foreach (var key in chipKeys)
        {
            var chip = GameMgr.In.chipTable.chipList.Find(x => x.howToGet.Equals(key));
            if (chip != null)
            {
                chip.createEnable = false;
            }
        }
    }

    private void ResetTimer()
    {
        timer = maxTime;
        UpdateGage();
    }

    public void UpdateTimer()
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

        foreach (Transform chipSlot in chipsetPanel.transform)
        {
            var slotImage = chipSlot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.raycastTarget = isInteractable;
            }

            if (chipSlot.childCount > 0)
            {
                var rawImage = chipSlot.GetChild(0).GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.raycastTarget = isInteractable;
                }

                var image = chipSlot.GetChild(0).GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = isInteractable;
                }
            }
        }
    }

    public void ShowDialogueBox(string speaker)
    {
        if (speaker == "Team")
        {
            TeamdialogueBox.gameObject.SetActive(true);
            TeamdialogueBox.DOLocalMoveX(TeamdialogueBoxVisibleX, 1f).SetEase(Ease.OutQuad);
        }
        else if (speaker == "Boss")
        {
            BossdialogueBox.gameObject.SetActive(true);
            BossdialogueBox.DOLocalMoveX(BossdialogueBoxVisibleX, 1f).SetEase(Ease.OutQuad);
        }
    }

    public void HideDialogueBox()
    {
        TeamdialogueBox.DOLocalMoveX(TeamdialogueBoxHiddenX, 1f).SetEase(Ease.InQuad).OnComplete(() => TeamdialogueBox.gameObject.SetActive(false));
        BossdialogueBox.DOLocalMoveX(BossdialogueBoxHiddenX, 1f).SetEase(Ease.InQuad).OnComplete(() => BossdialogueBox.gameObject.SetActive(false));
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
