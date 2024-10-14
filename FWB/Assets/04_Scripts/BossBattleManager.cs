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

    //����/�Ǵ翡 ���� �̹��� �ٲ�� �� �ʿ�
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

        yield return ShowDialogue("���� ���� ����!");

        while (currentPuzzleIndex < maxPuzzleCnt)
        {
            isPuzzleCompleted = false;
            yield return new WaitUntil(() => isPuzzleCompleted);

            ApplyBossGimmick();
            yield return ShowGimmickDialogue();
        }

        EndBossBattle(true);
    }

    //é�Ϳ� ���� ���� ��� ���� �ʿ�
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

    //���� �� ��� ��� - dialogue
    private IEnumerator ShowGimmickDialogue()
    {
        if (isHero)
        {
            if (currentPuzzleIndex == 1)
            {
                yield return ShowDialogue("����: �̷� �þ߰� ��������!");
            }
            else if (currentPuzzleIndex == 2)
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
            else if (currentPuzzleIndex == 2)
            {
                yield return ShowDialogue("����: �� �ٽ� �ð��� ���ҽ�Ų�ٶ�..");
            }
        }
    }

    //����� ���� ��� ��� - dialogue
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
        currentPuzzleIndex++;
    }

    //ĳ���� �̹��� �з� �ʿ�
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
            CommonTool.In.OpenAlertPanel("���� óġ ����!");
            gameCanvas.gameObject.SetActive(false);
        }
        else
        {
            CommonTool.In.OpenAlertPanel("���� óġ ����... �������� �ٽ� �����մϴ�.");
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
