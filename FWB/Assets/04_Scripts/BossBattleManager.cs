using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager instance;
    [Header("Managers")]
    public PuzzleMgr puzzleMgr;
    public GameMgr gameMgr;
    public BossSkillManager bossSkillMgr;

    [Header("UI")]
    public RectTransform gageRectTr;
    public RectTransform gage;
    public Text teamDialogue;
    public Image allyImage;
    public Button clearPuzzleButton;
    public Button failPuzzleButton;
    public RectTransform dialogueBox;
    public Canvas gameCanvas;
    public GameObject screenShakeTarget;
    public Transform chipsetPanel;
    public Transform screenTarget;
    private List<Color> originalChipColors;
    private List<Color> originalRawChipColors;

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
    public int maxPuzzleCnt;

    private bool isPuzzleCompleted;
    private bool isGamePlaying;
    private int succeedPuzzleCnt;
    private int failureCount;
    public float timer;
    private int currentPuzzleIndex;
    private bool isHero;
    private float initialGageWidth;
    private Vector2 initialGagePos;

    public bool lastWeekStatus = false;
    private bool isBossBattleActive = false;
    private bool isGameCanvasActive;

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
        isGameCanvasActive = gameCanvas.enabled;
        SetUIActive(lastWeekStatus);
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
            }
            else
            {
                StopAllCoroutines();
                ResetGameState();
                isBossBattleActive = false;
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
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(3));

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

    //영웅/악당에 따라 이미지 바뀌는 거 필요
    private void DetermineBossAndAlly()
    {
        Debug.Log(gameMgr.tendency);
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

    private IEnumerator StartBossBattle()
    {
        ResetGameState();
        DetermineBossAndAlly();

        puzzleMgr.isFeverMode = false;
        puzzleMgr.isTutorial = false;
        isGamePlaying = true;

        puzzleMgr.OnMakingDone += OnBossBattleMakingDone;

        yield return ShowDialogue("보스 전투 시작!");

        while (currentPuzzleIndex < maxPuzzleCnt)
        {
            isPuzzleCompleted = false;
            yield return new WaitUntil(() => isPuzzleCompleted);

            ApplyBossGimmick();
            yield return ShowGimmickDialogue();
        }

        EndBossBattle(true);
    }

    //챕터에 따라 보스 기믹 설정 필요
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

    //시작 전 대사 기믹 - dialogue
    private IEnumerator ShowGimmickDialogue()
    {
        if (isHero)
        {
            if (currentPuzzleIndex == 1)
            {
                yield return ShowDialogue("버니: 이런 시야가 가려진다!");
            }
            else if (currentPuzzleIndex == 2)
            {
                yield return ShowDialogue("버니: 위아래가 뒤집혀진다 조심해!");
            }
        }
        else
        {
            if (currentPuzzleIndex == 1)
            {
                yield return ShowDialogue("퍼펫: 제작 시간을 감소시켰군요..");
            }
            else if (currentPuzzleIndex == 2)
            {
                yield return ShowDialogue("퍼펫: 또 다시 시간을 감소시킨다라..");
            }
        }
    }

    //결과에 따른 대사 기믹 - dialogue
    private void OnBossBattleMakingDone(int result)
    {
        if (!isGamePlaying) return;

        if (result == 3)
        {
            succeedPuzzleCnt++;
            ScreenShake();
            StartCoroutine(ShowDialogue(isHero ? "버니: 공격 꽤나 효과적이야!" : "퍼펫: 이 무기는 효과적이군요.."));
        }
        else
        {
            failureCount++;
            UpdateCharacterPopup();
            StartCoroutine(ShowDialogue(isHero ? "버니: 이런...효과가 없어.. 다시 제대로 만들어줘!!" : "퍼펫: 이런 쓰잘데기 없는 걸...!! 정신차리시지요.."));
            if (failureCount >= 3)
            {
                EndBossBattle(false);
            }
        }
        isPuzzleCompleted = true;
        currentPuzzleIndex++;
    }

    //캐릭터 이미지 분류 필요
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

    private void EndBossBattle(bool success)
    {
        isGamePlaying = false;

        if (success)
        {
            CommonTool.In.OpenAlertPanel("보스 처치 성공!");
            gameCanvas.gameObject.SetActive(false);
        }
        else
        {
            CommonTool.In.OpenAlertPanel("보스 처치 실패... 보스전을 다시 시작합니다.");
        }

        ResetGameState();

        clearPuzzleButton.onClick.RemoveAllListeners();
        failPuzzleButton.onClick.RemoveAllListeners();

        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(3));
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
        clearPuzzleButton.onClick.AddListener(() => ProcessPuzzleResult(3));
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
