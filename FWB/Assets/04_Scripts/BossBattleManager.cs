using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossBattleManager : MonoBehaviour
{
    [Header("Managers")]
    public PuzzleMgr puzzleMgr;
    public GameMgr gameMgr;

    [Header("UI")]
    public RectTransform gageRectTr;
    public RectTransform gage;
    public Text teamDialogue;
    public Image allyImage;
    public Button clearPuzzleButton;
    public Button failPuzzleButton;
    public GameObject screenShakeTarget;
    public RectTransform dialogueBox;
    public Transform screenTarget;
    public Transform chipsetPanel;
    public Canvas gameCanvas;

    [Header("Sprites")]
    public Sprite puppetSprite;
    public Sprite bunnyNormalSprite;
    public Sprite bunnyCrackedSprite;
    public Sprite bunnyBrokenSprite;

    [Header("Dialogue Pos")]
    public float dialogueBoxHiddenX = -1350f;
    public float dialogueBoxVisibleX = -590f;

    [Header("Game Setting")]
    public float maxTime = 60f;
    public int maxPuzzleCnt = 5;

    private bool isPuzzleCompleted;
    private bool isGamePlaying;
    private int succeedPuzzleCnt;
    private int failureCount;
    private float timer;
    private int currentPuzzleIndex;
    private bool isHero;
    private float initialGageWidth;
    private List<Color> originalChipColors;
    private List<Color> originalRawChipColors;
    private Vector2 initialGagePos;

    private bool lastWeekStatus;
    private bool isGameCanvasActive;

    private void Start()
    {
        Initialize();
        lastWeekStatus = gameMgr.lastweek;
        isGameCanvasActive = gameCanvas.enabled;
        SetUIActive(lastWeekStatus);
        if (lastWeekStatus && isGameCanvasActive)
        {
            StartCoroutine(StartBossBattle());
            Debug.Log("BossBattle Start");
        }
    }

    private void Update()
    {
        if (gameMgr.lastweek != lastWeekStatus)
        {
            lastWeekStatus = gameMgr.lastweek;
            SetUIActive(lastWeekStatus);
            if (lastWeekStatus && isGameCanvasActive)
            {
                Debug.Log("BossBattle Start");
                StartCoroutine(StartBossBattle());
            }
            else
            {
                Debug.Log("BossBattle Stop");
                StopAllCoroutines();
                ResetGameState();
            }
        }

        if (gameCanvas.enabled != isGameCanvasActive)
        {
            isGameCanvasActive = gameCanvas.enabled;
            if (isGameCanvasActive && lastWeekStatus)
            {
                SetUIActive(true);
                Debug.Log("GameCanvas activated, starting BossBattle");
                StartCoroutine(StartBossBattle());
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
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(3));
        failPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(1));
    }

    private void SetUIActive(bool isActive)
    {
        gage.gameObject.SetActive(isActive);
        clearPuzzleButton.gameObject.SetActive(isActive);
        failPuzzleButton.gameObject.SetActive(isActive);
        dialogueBox.gameObject.SetActive(isActive);
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

    private void DetermineBossAndAlly()
    {
        if (gameMgr.tendency >= 0)
        {
            isHero = true;
            allyImage.sprite = bunnyNormalSprite;
        }
        else
        {
            isHero = false;
            allyImage.sprite = puppetSprite;
        }
    }

    private void SetTableDatasForBossBattle()
    {
        foreach (var chip in gameMgr.chipTable.chipList)
        {
            chip.createEnable = true;
        }

        foreach (var bpc in gameMgr.weaponDataTable.bluePrintCategoryList)
        {
            if (bpc.categoryKey.Equals("t_sword") || bpc.categoryKey.Equals("t_blunt"))
            {
                foreach (var bp in bpc.bluePrintList)
                {
                    bp.createEnable = true;
                }
            }
        }
    }

    private IEnumerator StartBossBattle()
    {
        DetermineBossAndAlly();
        SetTableDatasForBossBattle();
        ResetGameState();

        puzzleMgr.isFeverMode = false;
        puzzleMgr.isTutorial = false;
        isGamePlaying = true;

        yield return ShowDialogue("���� ���� ����!");

        while (currentPuzzleIndex < maxPuzzleCnt)
        {
            StartNewBossBattlePuzzle();
            puzzleMgr.OnMakingDone += OnBossBattleMakingDone;

            isPuzzleCompleted = false;
            yield return new WaitUntil(() => isPuzzleCompleted);

            ApplyBossGimmick();
            yield return ShowGimmickDialogue();

            puzzleMgr.OnMakingDone -= OnBossBattleMakingDone;

            currentPuzzleIndex++;
        }

        EndBossBattle(true);
    }

    private void StartNewBossBattlePuzzle()
    {
        var blueprint = GetRandomBlueprint();
        gameMgr.currentBluePrint = blueprint;
    }

    private WeaponDataTable.BluePrint GetRandomBlueprint()
    {
        var orderableBlueprintList = new List<WeaponDataTable.BluePrint>();
        foreach (var bpc in gameMgr.weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in bpc.bluePrintList)
            {
                if (bp.createEnable)
                {
                    orderableBlueprintList.Add(bp);
                }
            }
        }

        int index = Random.Range(0, orderableBlueprintList.Count);
        return orderableBlueprintList[index];
    }

    private void ApplyBossGimmick()
    {
        Debug.Log($"Applying Boss Gimmick: currentPuzzleIndex = {currentPuzzleIndex}");
        if (isHero)
        {
            if (currentPuzzleIndex == 1)
            {
                Debug.Log("Hero: Puppet Gimmick - Hide Chipset Info");
                HideChipsetInfo();
            }
            else if (currentPuzzleIndex == 3)
            {
                Debug.Log("Hero: Puppet Gimmick - Invert Screen");
                InvertScreen();
            }
        }
        else
        {
            if (currentPuzzleIndex == 1)
            {
                Debug.Log("Villain: Bunny Gimmick - Reduce Time to 45s");
                maxTime = 45f;
                timer = maxTime;
                UpdateGage();
            }
            else if (currentPuzzleIndex == 3)
            {
                Debug.Log("Villain: Bunny Gimmick - Reduce Time to 30s");
                maxTime = 30f;
                timer = maxTime;
                UpdateGage();
            }
        }
    }

    private IEnumerator ShowGimmickDialogue()
    {
        if (isHero)
        {
            if (currentPuzzleIndex == 1)
            {
                yield return ShowDialogue("����: �̷� �þ߰� ��������!");
            }
            else if (currentPuzzleIndex == 3)
            {
                yield return ShowDialogue("����: ���Ʒ��� ���������� ������!");
            }
        }
        else
        {
            if (currentPuzzleIndex == 1)
            {
                yield return ShowDialogue("����: ���� �ð��� ���ҽ��ױ���..");
            }
            else if (currentPuzzleIndex == 3)
            {
                yield return ShowDialogue("����: �� �ٽ� �ð��� ���ҽ�Ų�ٶ�..");
            }
        }
    }

    private void OnBossBattleMakingDone(int result)
    {
        if (!isGamePlaying) return;

        if (result == 3)
        {
            succeedPuzzleCnt++;
            ScreenShake();
            StartCoroutine(ShowDialogue(isHero ? "����: ���� �ϳ� ȿ�����̾�!" : "����: �� ����� ȿ�����̱���.."));
        }
        else
        {
            failureCount++;
            UpdateCharacterPopup();
            StartCoroutine(ShowDialogue(isHero ? "����: �̷�...ȿ���� ����.. �ٽ� ����� �������!!" : "����: �̷� ���ߵ��� ���� ��...!! ��������������.."));
            if (failureCount >= 3)
            {
                EndBossBattle(false);
            }
        }
        isPuzzleCompleted = true;
    }

    private void UpdateCharacterPopup(bool reset = false)
    {
        if (!isHero) return;
        if (reset)
        {
            allyImage.sprite = bunnyNormalSprite;
        }
        else
        {
            switch (failureCount)
            {
                case 1:
                    allyImage.sprite = bunnyCrackedSprite;
                    break;
                case 2:
                    allyImage.sprite = bunnyBrokenSprite;
                    break;
            }
        }
    }

    private void ProcessPuzzleResult(int result)
    {
        if (!isGamePlaying) return;
        OnBossBattleMakingDone(result);
    }

    private void ScreenShake()
    {
        screenShakeTarget.transform.DOShakePosition(1, new Vector3(20, 20, 0), 20, 90, false, true);
    }

    private void EndBossBattle(bool success)
    {
        isGamePlaying = false;

        if (success)
        {
            teamDialogue.text = "���� óġ ����";
            CommonTool.In.OpenAlertPanel("���� óġ ����! ����: ��ȣ ����ġ 200, ����ġ 200, ũ���� 1��");

            gameMgr.fame += 200;
            gameMgr.credit += 10000;
            if (isHero)
            {
                gameMgr.tendency += 200;
            }
            else
            {
                gameMgr.tendency -= 200;
            }
        }
        else
        {
            teamDialogue.text = "���� óġ ����...";
            CommonTool.In.OpenAlertPanel("���� óġ ����... �������� �ٽ� �����մϴ�.");
        }

        ResetGameState();
        ResetScreen();
        StartCoroutine(StartBossBattle());
    }

    private void ResetGameState()
    {
        maxTime = 60f;
        timer = maxTime;
        failureCount = 0;
        succeedPuzzleCnt = 0;
        currentPuzzleIndex = 0;
        isPuzzleCompleted = false;
        RestoreOriginalChipColors();
        UpdateGage();
        UpdateCharacterPopup(true);
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

    private void UpdateGage()
    {
        float gageWidth = initialGageWidth * (timer / maxTime);
        Vector2 sizeDelta = gageRectTr.sizeDelta;
        sizeDelta.x = gageWidth;
        gageRectTr.sizeDelta = sizeDelta;
        gageRectTr.anchoredPosition = new Vector2(initialGagePos.x - (initialGageWidth - gageWidth) / 2, initialGagePos.y);
    }

    private IEnumerator ShowDialogue(string message)
    {
        isGamePlaying = false;
        ToggleCanvasInteractable(false);
        teamDialogue.text = message;
        ShowDialogueBox();
        yield return new WaitForSeconds(3f);
        HideDialogueBox();
        ToggleCanvasInteractable(true);
        isGamePlaying = true;
    }

    private void ToggleCanvasInteractable(bool isInteractable)
    {
        foreach (var button in gameCanvas.GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractable;
        }
    }

    private void ShowDialogueBox()
    {
        dialogueBox.DOLocalMoveX(dialogueBoxVisibleX, 1f).SetEase(Ease.OutQuad);
    }

    private void HideDialogueBox()
    {
        dialogueBox.DOLocalMoveX(dialogueBoxHiddenX, 1f).SetEase(Ease.InQuad);
    }

    private void ResetScreen()
    {
        screenTarget.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void InvertScreen()
    {
        screenTarget.rotation = Quaternion.Euler(0, 0, 180);
    }

    private void HideChipsetInfo()
    {
        foreach (Transform chipSlot in chipsetPanel.transform)
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

}