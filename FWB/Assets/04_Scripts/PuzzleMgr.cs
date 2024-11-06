using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AbilityTable;
using static ChipTable;

/// <summary>
/// 퍼즐 플레이에 관련된 기능을 관리
/// </summary>
public class PuzzleMgr : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GridLayoutGroup puzzleGrid;
    public RectTransform chipPanelRectTr;
    public EventSystem es;
    public Canvas canvas;
    public RawImage dragImg;
    public GameObject filterAndSortParent;
    public Text creditText;
    public Text orderText;
    public Text bluePrintName;
    public Text currentAbilityText;
    public Text requiredAbilityText;
    public Text usedChipText;
    public Text needCredit;
    public Text selectedChipName;
    public Text selectedChipPrice;
    public Text selectedChipDesc;
    public float chipSize;
    public PuzzleFrame puzzleFrame;
    public List<Texture> textureList = new List<Texture>();
    public List<ChipObj> chipList = new List<ChipObj>();
    public List<AbilityFilterUI> abilityFilterList = new List<AbilityFilterUI>();
    public List<AbilityFilterUI> abilityTagList = new List<AbilityFilterUI>();
    public Button makingDone;
    public Button sortTargetBtn;
    public Button sortOrderBtn;
    public Button revertChips;
    public GameSceneMgr mgr2;
    public bool isFeverMode;
    public Sprite chipBackgroundOnSprite;
    public Sprite chipBackgroundOffSprite;
    public Transform puzzleChipParent;
    public Texture frameBackgroundOffTexture;
    public Texture frameBackgroundOnTexture;
    public Texture framePreviewOffTexture;
    public Texture framePreviewOnTexture;
    public Action<int> OnMakingDone;
    [HideInInspector]
    public bool isTutorial = true;
    [HideInInspector]
    public List<string> creatableChipKeyList = new List<string>();
    public ChipObj[] chipArrForFeverMode = new ChipObj[6];

    private RectTransform dragImgRectTr;
    private Puzzle currPuzzle;
    private ChipObj currentSelectedChip;
    private Chip currentSelectedChipData;
    private ScrollText scrollText;
    private CanvasScaler canvasScaler;
    private PointerEventData ped;
    private bool isFromPuzzle;
    private bool isAscending = true;
    private int enabledFrameCnt;
    private int pressedChipIndex = -1;
    private int currentTagCnt;
    private Vector3 resolutionOffset;
    private Vector3 chipOffset;
    private Vector3 prevMousePos;
    private SpriteChange sortOrderSC;
    private List<PuzzleFrame> puzzleFrameList = new List<PuzzleFrame>();
    private List<string> filterAbilityKeyList = new List<string>();
    private List<string> etcAbilityKeyList = new List<string>();
    private Dictionary<Chip, int> currentChipInPuzzleDic = new Dictionary<Chip, int>();
    private Dictionary<Ability, int> currentAbilityInPuzzleDic = new Dictionary<Ability, int>();
    private Dictionary<ChipObj, int> chipObjDic = new Dictionary<ChipObj, int>();
    private List<PuzzleFrameData> previewTargetPfd = new List<PuzzleFrameData>();

    [Header("FeverMode")]
    public RectTransform gageRectTr;
    public float maxPosX;
    public float maxTime;
    public int maxPuzzleCnt;
    public int succeedPuzzleCnt;
    public GameObject gageParentObj;
    public GameObject particleParentObj;
    public GameObject normalParticleParentObj;
    public GameObject resultParticleParentObj;
    public GameObject greenParticleParentObj;
    public GameObject yellowParticleParentObj;
    public GameObject redParticleParentObj;
    public GameObject whiteParticleParentObj;
    public GameObject blueParticleParentObj;
    public GameObject purpleParticleParentObj;
    private Image gageImage;
    private bool isPuzzleCompleted;
    private bool isGamePlaying;
    private int feverModeFame;
    private int feverModeRevenue;
    private int feverModeCredit;
    private Coroutine resultParticleRoutine;
    private List<WeaponDataTable.BluePrint> orderableBlueprintList = new List<WeaponDataTable.BluePrint>();

    private float lastRotationTime = 0f;
    private float rotationDebounceTime = 0.01f; // 디바운싱 시간 설정
    public class Puzzle
    {
        public int x;
        public int y;
        public List<Ability> requiredChipAbilityList = new List<Ability>();
        public PuzzleFrameData[,] frameDataTable;
    }

    [System.Serializable]
    public class PuzzleFrameData
    {
        public int patternNum = 0;
        public int x;
        public int y;
        public List<ChipObj> chipList = new List<ChipObj>();
        public int chipType;
        public int previewTypeNum;

        public PuzzleFrameData(int patternNum, int x, int y)
        {
            this.patternNum = patternNum;
            this.x = x;
            this.y = y;
        }
    }

    void Awake()
    {
        makingDone.onClick.AddListener(OnClickMakingDone);
        sortTargetBtn.onClick.AddListener(OnClickSortTarget);
        sortOrderBtn.onClick.AddListener(OnClickSortTarget);
        revertChips.onClick.AddListener(OnClickRevertChips);

        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        ped = new PointerEventData(es);
        sortOrderSC = sortOrderBtn.GetComponent<SpriteChange>();
        scrollText = orderText.GetComponent<ScrollText>();
        gageImage = gageRectTr.GetComponent<Image>();

        resolutionOffset = new Vector3(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, 0) / 2;

        List<string> existAbilityList = new List<string>();
        foreach (var filter in abilityFilterList)
        {
            existAbilityList.Add(filter.abilityKey);
        }
        var abilityList = GameMgr.In.abilityTable.abilityList;
        foreach (var ability in abilityList)
        {
            if (!existAbilityList.Contains(ability.abilityKey))
            {
                etcAbilityKeyList.Add(ability.abilityKey);
            }
        }

        for (int i = 0; i < abilityFilterList.Count; i++)
        {
            int tempNum = i;
            abilityFilterList[tempNum].button.onClick.AddListener(() =>
            {
                var filter = abilityFilterList[tempNum];
                AbilityFilterUI tag;
                if (filter.isOn)
                {
                    currentTagCnt--;
                    filter.image.sprite = filter.offSprite;
                    if (!filter.abilityKey.Equals("ETC"))
                    {
                        filterAbilityKeyList.Remove(filter.abilityKey);
                    }
                    tag = abilityTagList.Find(x => x.abilityKey.Equals(filter.abilityKey));
                    tag.abilityKey = string.Empty;
                    tag.transform.SetSiblingIndex(2);
                    tag.image.sprite = tag.offSprite;
                    tag.textForTag.text = string.Empty;
                    tag.button.interactable = false;
                    tag.button.onClick.RemoveAllListeners();
                }
                else
                {
                    if (currentTagCnt >= 3)
                    {
                        return;
                    }
                    currentTagCnt++;
                    filter.image.sprite = filter.onSprite;

                    tag = abilityTagList.First(x => string.IsNullOrEmpty(x.abilityKey));
                    if (filter.abilityKey.Equals("ETC"))
                    {
                        tag.textForTag.text = "이능력";
                    }
                    else
                    {
                        filterAbilityKeyList.Add(filter.abilityKey);
                        tag.textForTag.text = GameMgr.In.GetAbility(filter.abilityKey).name;
                    }
                    tag.abilityKey = filter.abilityKey;
                    tag.transform.SetSiblingIndex(currentTagCnt - 1);
                    tag.image.sprite = tag.onSprite;
                    tag.button.interactable = true;
                    tag.button.onClick.AddListener(() =>
                    {
                        filter.button.onClick.Invoke();
                    });
                }
                filter.isOn = !filter.isOn;

                RefreshChips();
            });
        }
    }

    void Update()
    {
        if (currentSelectedChip != null)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.R) && Time.time - lastRotationTime > rotationDebounceTime)
            {
                lastRotationTime = Time.time;
                var angle = dragImgRectTr.localEulerAngles;
                chipOffset = Vector3.zero;
                if (angle.z < 1)
                {
                    angle.z = 270;
                    chipOffset = new Vector3((dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2,
                                             (-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2);
                }
                else if (angle.z < 91)
                {
                    angle.z = 0;
                    chipOffset = new Vector3((dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2,
                                             (dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2);
                }
                else if (angle.z < 181)
                {
                    angle.z = 90;
                    chipOffset = new Vector3((-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2,
                                             (dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2);
                }
                else if (angle.z < 271)
                {
                    angle.z = 180;
                    chipOffset = new Vector3((-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2,
                                             (-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2);
                }
                dragImgRectTr.localEulerAngles = angle;
                dragImgRectTr.localPosition += chipOffset;

                currentSelectedChip.RotateRight();
                VisualChipLocation(Input.mousePosition);
            }
        }

        if (isFeverMode)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
             || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            {
                if (pressedChipIndex == -1)
                {
                    int keyCnt = 0;
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        pressedChipIndex = 0;
                        keyCnt++;
                    }
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        pressedChipIndex = 1;
                        keyCnt++;
                    }
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        pressedChipIndex = 2;
                        keyCnt++;
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        pressedChipIndex = 3;
                        keyCnt++;
                    }
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        pressedChipIndex = 4;
                        keyCnt++;
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        pressedChipIndex = 5;
                        keyCnt++;
                    }

                    if (keyCnt == 1)
                    {
                        currentSelectedChip = null;

                        if (pressedChipIndex >= 0 && chipArrForFeverMode.Length > pressedChipIndex)
                        {
                            ChipObj targetChip = chipArrForFeverMode[pressedChipIndex];
                            if (targetChip == null || !targetChip.gameObject.activeInHierarchy)
                            {
                                currentSelectedChipData = null;
                                currentSelectedChip = null;
                                pressedChipIndex = -1;
                            }
                            else
                            {
                                isFromPuzzle = false;

                                currentSelectedChipData = GameMgr.In.GetChip(targetChip.chipKey);
                                currentSelectedChip = targetChip;

                                currentSelectedChip.SaveCurrentRow();

                                selectedChipName.text = currentSelectedChipData.chipName;
                                selectedChipPrice.text = "개당 " + currentSelectedChipData.price + " c";
                                selectedChipDesc.text = currentSelectedChipData.desc;

                                dragImg.texture = currentSelectedChip.image.texture;
                                if (currentSelectedChip.originRow.Length != currentSelectedChip.originRowNum)
                                {
                                    if (currentSelectedChip.rowNum == currentSelectedChip.originRowNum)
                                    {
                                        dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.rowNum, currentSelectedChip.colNum) * chipSize;
                                    }
                                    else
                                    {
                                        dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.colNum, currentSelectedChip.rowNum) * chipSize;
                                    }
                                }
                                else
                                {
                                    if (currentSelectedChip.originRow.Length == currentSelectedChip.rowNum)
                                    {
                                        dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.rowNum, currentSelectedChip.colNum) * chipSize;
                                    }
                                    else
                                    {
                                        dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.colNum, currentSelectedChip.rowNum) * chipSize;
                                    }
                                }

                                chipOffset = Vector3.zero;
                                dragImgRectTr.localEulerAngles = currentSelectedChip.rectTr.localEulerAngles;
                                var angle = dragImgRectTr.localEulerAngles;
                                if (angle.z < 1)
                                {
                                    chipOffset = new Vector3((-dragImgRectTr.sizeDelta.x) / 2, (dragImgRectTr.sizeDelta.y) / 2);
                                }
                                else if (angle.z < 91)
                                {
                                    chipOffset = new Vector3((-dragImgRectTr.sizeDelta.x) / 2, (-dragImgRectTr.sizeDelta.y) / 2);
                                }
                                else if (angle.z < 181)
                                {
                                    chipOffset = new Vector3((dragImgRectTr.sizeDelta.x) / 2, (-dragImgRectTr.sizeDelta.y) / 2);
                                }
                                else if (angle.z < 271)
                                {
                                    chipOffset = new Vector3((dragImgRectTr.sizeDelta.x) / 2, (dragImgRectTr.sizeDelta.y) / 2);
                                }
                                dragImgRectTr.localPosition = Input.mousePosition + chipOffset - resolutionOffset;

                                dragImg.gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        pressedChipIndex = -1;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnClickMakingDone();
            }

            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
             || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
            {
                var pressedChipIndexList = new List<int>();
                if (Input.GetKey(KeyCode.Q))
                {
                    pressedChipIndexList.Add(0);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    pressedChipIndexList.Add(1);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    pressedChipIndexList.Add(2);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    pressedChipIndexList.Add(3);
                }
                if (Input.GetKey(KeyCode.Z))
                {
                    pressedChipIndexList.Add(4);
                }
                if (Input.GetKey(KeyCode.X))
                {
                    pressedChipIndexList.Add(5);
                }

                if (pressedChipIndexList.Contains(pressedChipIndex) && currentSelectedChip != null)
                {
                    if (prevMousePos == Vector3.zero)
                    {
                        dragImgRectTr.localPosition = Input.mousePosition + chipOffset - resolutionOffset;
                    }
                    else
                    {
                        var delta = Input.mousePosition - prevMousePos;
                        dragImgRectTr.localPosition += delta / canvas.scaleFactor;
                    }
                    prevMousePos = Input.mousePosition;

                    VisualChipLocation(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                }
            }

            if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A)
             || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.X))
            {
                var pressedChipIndexList = new List<int>();
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    pressedChipIndexList.Add(0);
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    pressedChipIndexList.Add(1);
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    pressedChipIndexList.Add(2);
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    pressedChipIndexList.Add(3);
                }
                if (Input.GetKeyUp(KeyCode.Z))
                {
                    pressedChipIndexList.Add(4);
                }
                if (Input.GetKeyUp(KeyCode.X))
                {
                    pressedChipIndexList.Add(5);
                }

                if (pressedChipIndexList.Contains(pressedChipIndex) && currentSelectedChip != null)
                {
                    foreach (var pfd in previewTargetPfd)
                    {
                        pfd.previewTypeNum = 0;
                    }
                    RefreshPuzzleBackgroundImages();

                    var angle = dragImgRectTr.localEulerAngles;
                    if (currentSelectedChip != null)
                    {
                        dragImg.gameObject.SetActive(false);
                        bool success = false;

                        var offset = (new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize) / 2;
                        ped.position = Input.mousePosition + new Vector3(-offset.x, offset.y, 0);
                        List<RaycastResult> results = new List<RaycastResult>();
                        es.RaycastAll(ped, results);
                        if (results.Count > 0)
                        {
                            bool isChipPanel = false;

                            foreach (var result in results)
                            {
                                if (result.gameObject.name.Contains("ChipPanelRange"))
                                {
                                    isChipPanel = true;
                                    break;
                                }
                            }

                            if (!isChipPanel)
                            {
                                PuzzleFrame frame = null;
                                foreach (var result in results)
                                {
                                    frame = result.gameObject.GetComponent<PuzzleFrame>();
                                    if (frame != null) break;
                                }

                                if (frame != null)
                                {
                                    var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChip, frame.pfd.x, frame.pfd.y);
                                    if (fittableFrames != null)
                                    {
                                        var chipInstance = Instantiate(currentSelectedChip, chipPanelRectTr.transform);
                                        chipInstance.name = currentSelectedChip.name;
                                        for (int i = 0; i < fittableFrames.Count; i++)
                                        {
                                            fittableFrames[i].chipList.Add(chipInstance);
                                        }

                                        chipInstance.transform.parent = frame.transform;
                                        if (chipInstance.originRow.Length != chipInstance.originRowNum)
                                        {
                                            if (chipInstance.rowNum == chipInstance.originRowNum)
                                            {
                                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                                            }
                                            else
                                            {
                                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
                                            }
                                        }
                                        else
                                        {
                                            if (chipInstance.originRow.Length == chipInstance.rowNum)
                                            {
                                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                                            }
                                            else
                                            {
                                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
                                            }
                                        }
                                        chipInstance.rectTr.anchoredPosition = Vector3.zero;

                                        chipInstance.rectTr.localEulerAngles = angle;
                                        var pos = chipInstance.rectTr.localPosition;
                                        if (angle.z < 1)
                                        {
                                        }
                                        else if (angle.z < 91)
                                        {
                                            pos.y -= dragImgRectTr.rect.height * (chipInstance.colNum * 1f / chipInstance.rowNum);
                                        }
                                        else if (angle.z < 181)
                                        {
                                            pos.y -= dragImgRectTr.rect.height;
                                            pos.x += dragImgRectTr.rect.width;
                                        }
                                        else if (angle.z < 271)
                                        {
                                            pos.x += dragImgRectTr.rect.width * (chipInstance.rowNum * 1f / chipInstance.colNum);
                                        }
                                        chipInstance.rectTr.localPosition = pos;
                                        chipInstance.rectTr.parent = puzzleChipParent;

                                        if (!isFromPuzzle)
                                        {
                                            if (currentChipInPuzzleDic.ContainsKey(currentSelectedChipData))
                                            {
                                                currentChipInPuzzleDic[currentSelectedChipData] += 1;
                                            }
                                            else
                                            {
                                                currentChipInPuzzleDic.Add(currentSelectedChipData, 1);
                                            }
                                            currentSelectedChip.ResetColToOriginData();
                                            RefreshWeaponPowerData();
                                        }
                                        else
                                        {
                                            foreach (var fr in puzzleFrameList)
                                            {
                                                if (fr.pfd.chipList.Contains(currentSelectedChip))
                                                {
                                                    fr.pfd.chipList.Remove(currentSelectedChip);
                                                    if (fr.pfd.chipList.Count == 0)
                                                    {
                                                        fr.pfd.chipType = 0;
                                                        if (fr.pfd.patternNum != 0)
                                                        {
                                                            fr.SetBackgroundImage(frameBackgroundOffTexture);
                                                        }
                                                    }
                                                }
                                            }
                                            DestroyImmediate(currentSelectedChip.gameObject);
                                        }

                                        foreach (var fr in puzzleFrameList)
                                        {
                                            if (fr.pfd.chipList.Contains(chipInstance))
                                            {
                                                if (fr.pfd.patternNum != 0)
                                                {
                                                    fr.SetBackgroundImage(frameBackgroundOnTexture);
                                                }
                                            }
                                        }

                                        success = true;
                                    }
                                }
                            }
                        }
                        if (!success)
                        {
                            currentSelectedChip.ResetColToOriginData();
                        }
                        currentSelectedChip = null;
                    }
                    pressedChipIndex = -1;
                    prevMousePos = Vector3.zero;
                }
            }
        }
    }

    public void StartPuzzle()
    {
        if (BossBattleManager.Instance.lastWeekStatus == true)
        {
            chipSize = 48;
        }
        else
        {
            chipSize = 64;
        }
        creditText.text = GameMgr.In.credit.ToString();
        SetOrderDatas();
        SetBluePrintDatas();
        SetPuzzle();
        SetChipDatas();
        RefreshChips();
    }

    public void StartFeverModePuzzle()
    {
        chipSize = 64;
        creditText.text = GameMgr.In.credit.ToString();
        SetBluePrintDatas();
        SetPuzzle();
        SetFeverModeChipDatas();
    }

    public void OnClickMakingDone()
    {
        if (isFeverMode)
        {
            MakingDone();
            return;
        }

        CommonTool.In.OpenConfirmPanel("제작을 완료하시겠습니까?", () =>
        {
            MakingDone();
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
         || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
        {
            return;
        }

        isFromPuzzle = false;
        currentSelectedChip = null;
        ped.position = eventData.pressPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        if (results.Count > 0)
        {
            ChipObj targetChip = null;
            foreach (var result in results)
            {
                if (result.gameObject.name.Contains("PuzzleFrame"))
                {
                    isFromPuzzle = true;
                    var pfd = result.gameObject.GetComponent<PuzzleFrame>().pfd;
                    if (pfd.chipType == 2 || pfd.chipType == 0)
                    {
                        return;
                    }
                    targetChip = pfd.chipList[0];
                    break;
                }
            }

            if (targetChip == null)
            {
                targetChip = results[0].gameObject.GetComponent<ChipObj>();
            }

            if (targetChip == null)
            {
                if (results[0].gameObject.transform.childCount > 0)
                {
                    targetChip = results[0].gameObject.transform.GetChild(0).GetComponent<ChipObj>();
                }
            }

            if (targetChip != null)
            {
                if (targetChip.gameObject.activeInHierarchy)
                {
                    currentSelectedChipData = GameMgr.In.GetChip(targetChip.chipKey);
                    currentSelectedChip = targetChip;

                    currentSelectedChip.SaveCurrentRow();
                    if (!isFromPuzzle)
                    {
                        selectedChipName.text = currentSelectedChipData.chipName;
                        selectedChipPrice.text = "개당 " + currentSelectedChipData.price + " c";
                        selectedChipDesc.text = currentSelectedChipData.desc;
                        foreach (var chip in chipList)
                        {
                            var sprite = chip == currentSelectedChip ? chipBackgroundOnSprite : chipBackgroundOffSprite;
                            chip.parentImage.sprite = sprite;
                        }
                        currentSelectedChip.transform.parent.GetComponent<Image>().sprite = chipBackgroundOnSprite;
                    }
                    return;
                }
            }
        }
        isFromPuzzle = false;
        currentSelectedChipData = null;
        currentSelectedChip = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
         || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
        {
            return;
        }

        if (currentSelectedChip != null)
        {
            dragImg.texture = currentSelectedChip.image.texture;
            if (currentSelectedChip.originRow.Length != currentSelectedChip.originRowNum)
            {
                if (currentSelectedChip.rowNum == currentSelectedChip.originRowNum)
                {
                    dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.rowNum, currentSelectedChip.colNum) * chipSize;
                }
                else
                {
                    dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.colNum, currentSelectedChip.rowNum) * chipSize;
                }
            }
            else
            {
                if (currentSelectedChip.originRow.Length == currentSelectedChip.rowNum)
                {
                    dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.rowNum, currentSelectedChip.colNum) * chipSize;
                }
                else
                {
                    dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.colNum, currentSelectedChip.rowNum) * chipSize;
                }
            }


            chipOffset = Vector3.zero;
            dragImgRectTr.localEulerAngles = currentSelectedChip.rectTr.localEulerAngles;
            var angle = dragImgRectTr.localEulerAngles;
            if (angle.z < 1)
            {
                chipOffset = new Vector3((-dragImgRectTr.sizeDelta.x) / 2, (dragImgRectTr.sizeDelta.y) / 2);
            }
            else if (angle.z < 91)
            {
                chipOffset = new Vector3((-dragImgRectTr.sizeDelta.y) / 2, (-dragImgRectTr.sizeDelta.x) / 2);
            }
            else if (angle.z < 181)
            {
                chipOffset = new Vector3((dragImgRectTr.sizeDelta.x) / 2, (-dragImgRectTr.sizeDelta.y) / 2);
            }
            else if (angle.z < 271)
            {
                chipOffset = new Vector3((dragImgRectTr.sizeDelta.y) / 2, (dragImgRectTr.sizeDelta.x) / 2);
            }
            dragImgRectTr.localPosition = Input.mousePosition + chipOffset - resolutionOffset;

            dragImg.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
         || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
        {
            return;
        }

        if (currentSelectedChip != null)
        {
            dragImgRectTr.localPosition += (Vector3)eventData.delta / canvas.scaleFactor;
            VisualChipLocation(eventData.position);
        }
    }

    public void VisualChipLocation(Vector2 position)
    {
        var offset = (new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize) / 2;
        ped.position = position + new Vector2(-offset.x, offset.y);
        List<RaycastResult> results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        if (results.Count > 0)
        {
            PuzzleFrame frame = null;
            foreach (var result in results)
            {
                frame = result.gameObject.GetComponent<PuzzleFrame>();
                if (frame != null) break;
            }

            foreach (var pfd in previewTargetPfd)
            {
                pfd.previewTypeNum = 0;
            }

            bool isFittable = false;
            if (frame != null)
            {
                isFittable = SetPreviewTypeNum(currPuzzle.frameDataTable, currentSelectedChip, frame.pfd.x, frame.pfd.y);
            }
            var color = isFittable ? new Color(1, 1, 1, 0.65f) : new Color(0.9f, 0.25f, 0.25f, 0.65f);
            dragImg.color = color;
            RefreshPuzzleBackgroundImages();
        }
    }

    public void RefreshChips()
    {
        var etc = abilityTagList.Find(x => x.abilityKey.Equals("ETC"));
        foreach (var chip in chipList)
        {
            if (!creatableChipKeyList.Contains(chip.chipKey))
            {
                continue;
            }

            bool isActive = true;
            if (chip.enableOnSpecificBlueprint)
            {
                isActive = GameMgr.In.currentBluePrint.enableChipKeyList.Contains(chip.chipKey);
                if (!isActive)
                {
                    chip.transform.parent.gameObject.SetActive(false);
                    continue;
                }
            }

            var chipData = GameMgr.In.GetChip(chip.chipKey);
            if (etc != null)
            {
                bool isContains = false;
                foreach (var ability in chipData.abilityList)
                {
                    if (etcAbilityKeyList.Contains(ability.abilityKey))
                    {
                        isContains = true;
                        break;
                    }
                }
                if (!isContains)
                {
                    chip.transform.parent.gameObject.SetActive(false);
                    continue;
                }
            }

            foreach (var filterAbilityKey in filterAbilityKeyList)
            {
                var targetAbility = chipData.abilityList.Find(x => x.abilityKey.Equals(filterAbilityKey));
                if (targetAbility == null)
                {
                    isActive = false;
                    break;
                }
            }
            chip.transform.parent.gameObject.SetActive(isActive);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
         || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X))
        {
            return;
        }

        foreach (var pfd in previewTargetPfd)
        {
            pfd.previewTypeNum = 0;
        }
        RefreshPuzzleBackgroundImages();

        var angle = dragImgRectTr.localEulerAngles;
        if (currentSelectedChip != null)
        {
            dragImg.gameObject.SetActive(false);
            bool success = false;

            var offset = (new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize) / 2;
            ped.position = Input.mousePosition + new Vector3(-offset.x, offset.y, 0);
            List<RaycastResult> results = new List<RaycastResult>();
            es.RaycastAll(ped, results);
            if (results.Count > 0)
            {
                bool isChipPanel = false;

                foreach (var result in results)
                {
                    if (result.gameObject.name.Contains("ChipPanelRange"))
                    {
                        isChipPanel = true;
                        break;
                    }
                }

                if (isChipPanel)
                {
                    if (isFromPuzzle)
                    {
                        foreach (var fr in puzzleFrameList)
                        {
                            if (fr.pfd.chipList.Contains(currentSelectedChip))
                            {
                                fr.pfd.chipList.Remove(currentSelectedChip);
                                if (fr.pfd.chipList.Count == 0)
                                {
                                    fr.pfd.chipType = 0;
                                    if (fr.pfd.patternNum != 0)
                                    {
                                        fr.SetBackgroundImage(frameBackgroundOffTexture);
                                    }
                                }
                            }
                        }
                        DestroyImmediate(currentSelectedChip.gameObject);

                        if (currentChipInPuzzleDic[currentSelectedChipData] == 1)
                        {
                            currentChipInPuzzleDic.Remove(currentSelectedChipData);
                        }
                        else
                        {
                            currentChipInPuzzleDic[currentSelectedChipData] -= 1;
                        }
                        RefreshWeaponPowerData();

                        if (isTutorial)
                        {
                            makingDone.gameObject.SetActive(IsWeaponRequiredAbilitySatisfied());
                        }

                        success = true;
                    }
                }
                else
                {
                    PuzzleFrame frame = null;
                    foreach (var result in results)
                    {
                        frame = result.gameObject.GetComponent<PuzzleFrame>();
                        if (frame != null) break;
                    }

                    if (frame != null)
                    {
                        var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChip, frame.pfd.x, frame.pfd.y);
                        if (fittableFrames != null)
                        {
                            var chipInstance = Instantiate(currentSelectedChip, chipPanelRectTr.transform);
                            chipInstance.name = currentSelectedChip.name;
                            for (int i = 0; i < fittableFrames.Count; i++)
                            {
                                fittableFrames[i].chipList.Add(chipInstance);
                            }

                            chipInstance.transform.parent = frame.transform;
                            if (chipInstance.originRow.Length != chipInstance.originRowNum)
                            {
                                if (chipInstance.rowNum == chipInstance.originRowNum)
                                {
                                    chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                                }
                                else
                                {
                                    chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
                                }
                            }
                            else
                            {
                                if (chipInstance.originRow.Length == chipInstance.rowNum)
                                {
                                    chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                                }
                                else
                                {
                                    chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
                                }
                            }
                            chipInstance.rectTr.anchoredPosition = Vector3.zero;

                            chipInstance.rectTr.localEulerAngles = angle;
                            var pos = chipInstance.rectTr.localPosition;
                            if (angle.z < 1)
                            {
                            }
                            else if (angle.z < 91)
                            {
                                pos.y -= dragImgRectTr.rect.height * (chipInstance.colNum * 1f / chipInstance.rowNum);
                            }
                            else if (angle.z < 181)
                            {
                                pos.y -= dragImgRectTr.rect.height;
                                pos.x += dragImgRectTr.rect.width;
                            }
                            else if (angle.z < 271)
                            {
                                pos.x += dragImgRectTr.rect.width * (chipInstance.rowNum * 1f / chipInstance.colNum);
                            }
                            chipInstance.rectTr.localPosition = pos;
                            chipInstance.rectTr.parent = puzzleChipParent;

                            if (!isFromPuzzle)
                            {
                                if (currentChipInPuzzleDic.ContainsKey(currentSelectedChipData))
                                {
                                    currentChipInPuzzleDic[currentSelectedChipData] += 1;
                                }
                                else
                                {
                                    currentChipInPuzzleDic.Add(currentSelectedChipData, 1);
                                }
                                currentSelectedChip.ResetColToOriginData();
                                RefreshWeaponPowerData();
                            }
                            else
                            {
                                foreach (var fr in puzzleFrameList)
                                {
                                    if (fr.pfd.chipList.Contains(currentSelectedChip))
                                    {
                                        fr.pfd.chipList.Remove(currentSelectedChip);
                                        if (fr.pfd.chipList.Count == 0)
                                        {
                                            fr.pfd.chipType = 0;
                                            if (fr.pfd.patternNum != 0)
                                            {
                                                fr.SetBackgroundImage(frameBackgroundOffTexture);
                                            }
                                        }
                                    }
                                }
                                DestroyImmediate(currentSelectedChip.gameObject);
                            }

                            foreach (var fr in puzzleFrameList)
                            {
                                if (fr.pfd.chipList.Contains(chipInstance))
                                {
                                    if (fr.pfd.patternNum != 0)
                                    {
                                        fr.SetBackgroundImage(frameBackgroundOnTexture);
                                    }
                                }
                            }

                            if (isTutorial)
                            {
                                makingDone.gameObject.SetActive(IsWeaponRequiredAbilitySatisfied());
                            }

                            success = true;
                        }
                    }
                }
            }
            if (!success)
            {
                if (isFromPuzzle)
                {
                    currentSelectedChip.ResetColToCurrentData();
                }
                else
                {
                    currentSelectedChip.ResetColToOriginData();
                }
            }
            currentSelectedChip = null;
        }
        isFromPuzzle = false;
    }

    private void RefreshPuzzleBackgroundImages()
    {
        foreach (var pf in puzzleFrameList)
        {
            if (pf.pfd.patternNum == 0) continue;

            switch (pf.pfd.previewTypeNum)
            {
                case 0:
                    var texture = pf.pfd.chipList.Count > 0 ? frameBackgroundOnTexture : frameBackgroundOffTexture;
                    pf.SetBackgroundImage(texture);
                    break;
                case 1:
                    pf.SetBackgroundImage(framePreviewOffTexture);
                    break;
                case 2:
                    pf.SetBackgroundImage(framePreviewOnTexture);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetPuzzle()
    {
        currPuzzle = CommonTool.In.GetPuzzle();
        InstantiateFrames(currPuzzle.frameDataTable);
        enabledFrameCnt = 0;
        foreach (var frameData in currPuzzle.frameDataTable)
        {
            if (frameData.patternNum == 1) enabledFrameCnt++;
        }
    }

    private void InstantiateFrames(PuzzleFrameData[,] frameDataTable)
    {
        if (BossBattleManager.Instance.lastWeekStatus == true)
        {
            puzzleGrid.cellSize = new Vector2(48, 48);
        }
        else
        {
            puzzleGrid.cellSize = new Vector2(64, 64);
        }

        foreach (var frameData in frameDataTable)
        {
            var frame = Instantiate(puzzleFrame, puzzleGrid.transform);
            frame.SetPuzzleFrameData(frameData);
            puzzleFrameList.Add(frame);
            frame.image.texture = textureList[frameData.patternNum];
        }
    }

    private List<PuzzleFrameData> GetFittableFrameList(PuzzleFrameData[,] targetTable, ChipObj chip, int x, int y)
    {
        List<PuzzleFrameData> pfdList = new List<PuzzleFrameData>();
        List<int> chipTypeList = new List<int>();
        var table = chip.row;
        x += chip.posOffset[0];
        y += chip.posOffset[1];
        for (int i = 0; i < table.Length; i++)
        {
            for (int j = 0; j < table[i].col.Length; j++)
            {
                if (table[i].col[j] == 0)
                {
                    continue;
                }

                if (targetTable.GetLength(0) <= y + j) return null;
                if (targetTable.GetLength(1) <= x + i) return null;

                if (table[i].col[j] == 1)
                {
                    if (targetTable[y + j, x + i].patternNum == 0)
                    {
                        return null;
                    }
                }

                if (x + i >= currPuzzle.x || y + j >= currPuzzle.y || x + i < 0 || y + j < 0)
                {
                    return null;
                }

                if (targetTable[y + j, x + i].chipList.Count > 0)
                {
                    if (table[i].col[j] != 2 || targetTable[y + j, x + i].chipType != 2)
                    {
                        if (targetTable[y + j, x + i].chipList.Count != 1 || targetTable[y + j, x + i].chipList[0] != chip)
                        {
                            return null;
                        }
                    }
                }

                if (targetTable[y + j, x + i].chipList.Count > 0 && !targetTable[y + j, x + i].chipList.Contains(chip))
                {
                    if (table[i].col[j] != 2 || targetTable[y + j, x + i].chipType != 2)
                    {
                        return null;
                    }
                }

                chipTypeList.Add(table[i].col[j]);
                pfdList.Add(targetTable[y + j, x + i]);
            }
        }

        for (int i = 0; i < pfdList.Count; i++)
        {
            pfdList[i].chipType = chipTypeList[i];
        }

        return pfdList;
    }

    private bool SetPreviewTypeNum(PuzzleFrameData[,] targetTable, ChipObj chip, int x, int y)
    {
        foreach (var pfd in previewTargetPfd)
        {
            pfd.previewTypeNum = 0;
        }
        previewTargetPfd.Clear();
        bool isFittable = true;
        var table = chip.row;
        x += chip.posOffset[0];
        y += chip.posOffset[1];
        for (int i = 0; i < table.Length; i++)
        {
            for (int j = 0; j < table[i].col.Length; j++)
            {
                if (table[i].col[j] == 0)
                {
                    continue;
                }

                if (targetTable.GetLength(0) <= y + j)
                {
                    isFittable = false;
                    continue;
                }
                if (targetTable.GetLength(1) <= x + i)
                {
                    isFittable = false;
                    continue;
                }

                if (table[i].col[j] == 1)
                {
                    if (targetTable[y + j, x + i].patternNum == 0)
                    {
                        isFittable = false;
                        continue;
                    }
                }

                if (x + i >= currPuzzle.x || y + j >= currPuzzle.y || x + i < 0 || y + j < 0)
                {
                    isFittable = false;
                    continue;
                }

                if (targetTable[y + j, x + i].chipList.Count > 0)
                {
                    if (table[i].col[j] != 2 || targetTable[y + j, x + i].chipType != 2)
                    {
                        if (targetTable[y + j, x + i].chipList.Count != 1 || targetTable[y + j, x + i].chipList[0] != chip)
                        {
                            isFittable = false;
                            targetTable[y + j, x + i].previewTypeNum = 1;
                            previewTargetPfd.Add(targetTable[y + j, x + i]);
                            continue;
                        }
                    }
                }

                if (targetTable[y + j, x + i].chipList.Count > 0 && !targetTable[y + j, x + i].chipList.Contains(chip))
                {
                    if (table[i].col[j] != 2 || targetTable[y + j, x + i].chipType != 2)
                    {
                        isFittable = false;
                        targetTable[y + j, x + i].previewTypeNum = 1;
                        previewTargetPfd.Add(targetTable[y + j, x + i]);
                        continue;
                    }
                }

                targetTable[y + j, x + i].previewTypeNum = 2;
                previewTargetPfd.Add(targetTable[y + j, x + i]);
            }
        }
        return isFittable;
    }

    private void SetOrderDatas()
    {
        var order = GameMgr.In.currentOrder;
        this.orderText.text = mgr2.mainChatText.text.Replace(" ▼", "").Replace("\n", " ");
        scrollText.OnTextChanged();
    }

    private void SetBluePrintDatas()
    {
        var bp = GameMgr.In.currentBluePrint;
        bluePrintName.text = bp.name;

        StringBuilder sb = new StringBuilder();
        foreach (var requiredAbility in bp.requiredChipAbilityList)
        {
            var ability = GameMgr.In.GetAbility(requiredAbility.abilityKey);
            sb.Append(ability.name).Append("+").Append(requiredAbility.count).Append(" ");
        }
        sb.Length--;
        requiredAbilityText.text = sb.ToString();
    }

    private bool hasInit;
    private void SetChipDatas()
    {
        if (!hasInit)
        {
            foreach (var chip in chipList)
            {
                chip.SaveOriginRow();
            }
            SortChipList();
            hasInit = true;
        }

        var creatableChips = GameMgr.In.chipTable.chipList.FindAll(x => x.createEnable);
        creatableChipKeyList = new List<string>();
        foreach (var cc in creatableChips)
        {
            creatableChipKeyList.Add(cc.chipKey);
        }
        foreach (var chip in chipList)
        {
            chip.transform.parent.gameObject.SetActive(creatableChipKeyList.Contains(chip.chipKey));
        }
    }

    private void SetFeverModeChipDatas()
    {

        if (chipArrForFeverMode[0] == null)
        {
            chipArrForFeverMode[0] = chipList.Find(x => x.chipKey.Equals("c_attack"));
            chipArrForFeverMode[0].transform.parent.SetSiblingIndex(0);
            chipArrForFeverMode[1] = chipList.Find(x => x.chipKey.Equals("c_acuity"));
            chipArrForFeverMode[1].transform.parent.SetSiblingIndex(1);
        }
        for (int i = 2; i < 6; i++)
        {
            if (chipArrForFeverMode[i] != null)
            {
                chipArrForFeverMode[i].transform.parent.gameObject.SetActive(false);
                chipArrForFeverMode[i] = null;
            }
        }

        var tableChipList = GameMgr.In.chipTable.chipList;
        var requiredAbilityList = GameMgr.In.currentBluePrint.requiredChipAbilityList;
        int index = 2;
        foreach (var requiredAbility in requiredAbilityList)
        {
            var targetChips = tableChipList.FindAll(x =>
                x.abilityList.Find(x =>
                    x.abilityKey.Equals(requiredAbility.abilityKey)) != null);
            foreach (var targetChip in targetChips)
            {
                if (targetChip.chipKey.Equals("c_attack") || targetChip.chipKey.Equals("c_acuity")) continue;
                var matchedChip = chipList.Find(x => x.chipKey.Equals(targetChip.chipKey));
                if (matchedChip != null)
                {
                    if (index > 5)
                    {
                        Debug.LogError("활성화되는 칩셋이 6개 이상임: " + matchedChip.chipKey);
                        continue;
                    }
                    chipArrForFeverMode[index] = matchedChip;
                    chipArrForFeverMode[index].transform.parent.SetSiblingIndex(index);
                    index++;
                }
            }
        }

        foreach (var chip in chipList)
        {
            chip.SaveOriginRow();
            chip.transform.parent.gameObject.SetActive(false);
        }
        foreach (var chip in chipArrForFeverMode)
        {
            if (chip == null) break;
            chip.transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnClickSortTarget()
    {
        isAscending = !isAscending;

        if (isAscending)
        {
            sortOrderSC.SetOffSprite();
        }
        else
        {
            sortOrderSC.SetOnSprite();
        }

        SortChipList();
    }

    private void SortChipList()
    {
        var chipList = GameMgr.In.chipTable.chipList;
        var orderedChipKeyList = new List<string>();
        if (isAscending)
        {
            orderedChipKeyList = chipList.OrderBy(x => x.price).ThenBy(x => x.chipKey).Select(x => x.chipKey).ToList();
        }
        else
        {
            orderedChipKeyList = chipList.OrderByDescending(x => x.price).ThenBy(x => x.chipKey).Select(x => x.chipKey).ToList();
        }

        for (int i = 0; i < orderedChipKeyList.Count; i++)
        {
            var matchedChip = this.chipList.Find(x => x.chipKey.Equals(orderedChipKeyList[i]));
            if (matchedChip == null) continue;

            matchedChip.transform.parent.SetSiblingIndex(i);
        }
    }

    private void RefreshWeaponPowerData()
    {
        StringBuilder currAbilitySB = new StringBuilder();
        StringBuilder usedChipSB = new StringBuilder();

        currentAbilityInPuzzleDic.Clear();
        int price = 0;
        foreach (var chip in currentChipInPuzzleDic.Keys)
        {
            usedChipSB.Append(chip.chipName).Append(" 칩셋 ").Append(currentChipInPuzzleDic[chip]).Append("개\n");

            price += chip.price;

            foreach (var ability in chip.abilityList)
            {
                var targetAbility = GameMgr.In.GetAbility(ability.abilityKey);
                if (currentAbilityInPuzzleDic.ContainsKey(targetAbility))
                {
                    currentAbilityInPuzzleDic[targetAbility] += ability.count * currentChipInPuzzleDic[chip];
                }
                else
                {
                    currentAbilityInPuzzleDic.Add(targetAbility, ability.count * currentChipInPuzzleDic[chip]);
                }
            }
        }
        needCredit.text = price.ToString();

        foreach (var ability in currentAbilityInPuzzleDic.Keys)
        {
            currAbilitySB.Append(ability.name).Append("+").Append(currentAbilityInPuzzleDic[ability]).Append(" ");
        }

        if (currAbilitySB.Length > 0)
        {
            currAbilitySB.Length--;
        }
        if (usedChipSB.Length > 0)
        {
            usedChipSB.Length--;
        }

        currentAbilityText.text = currAbilitySB.ToString();
        usedChipText.text = usedChipSB.ToString();
    }

    private void OnClickRevertChips()
    {
        foreach (var fr in puzzleFrameList)
        {
            fr.pfd.chipList.Clear();
            fr.pfd.chipType = 0;
            if (fr.pfd.patternNum != 0)
            {
                fr.SetBackgroundImage(frameBackgroundOffTexture);
            }
        }

        for (int i = puzzleChipParent.childCount - 1; 0 <= i; i--)
        {
            DestroyImmediate(puzzleChipParent.GetChild(i).gameObject);
        }

        if (makingDone != null && isTutorial)
        {
            makingDone.gameObject.SetActive(false);
        }
        currentChipInPuzzleDic.Clear();
        currentAbilityInPuzzleDic.Clear();
        RefreshWeaponPowerData();
    }

    private void MakingDone()
    {
        int score = 0;
        if (IsWeaponRequiredAbilitySatisfied()) score++;
        if (IsOrderRequirementSatisfied()) score++;
        if (score == 2)
        {
            if (isPerfectState()) score++;
            if (score == 2 && isFeverMode)
            {
                if (hasBasicChipsets()) score++;
            }
        }

        int chipPrice = 0;
        foreach (var chip in currentChipInPuzzleDic)
        {
            chipPrice += chip.Key.price;
        }

        ClearPuzzle();

        int fame = 0;
        int tendency = 0;
        int credit = 0;
        int revenue = 0;
        int bonus = 0;
        switch (score)
        {
            case 0:
                fame -= 25;
                if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Hero)
                {
                    tendency -= 25;
                }
                else if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Villain)
                {
                    tendency += 25;
                }
                credit -= chipPrice;

                mgr2.orderState = GameSceneMgr.OrderState.Failed;
                break;
            case 1:
                fame -= 10;
                if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Hero)
                {
                    tendency -= 10;
                }
                else if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Villain)
                {
                    tendency += 10;
                }
                revenue += chipPrice;

                mgr2.orderState = GameSceneMgr.OrderState.Failed;
                break;
            case 2:
                fame += 10;
                int sellPrice1 = GameMgr.In.currentBluePrint.sellPrice;
                if (isFeverMode)
                {
                    sellPrice1 = (int)(sellPrice1 * 0.4f);
                }

                if (!isTutorial)
                {
                    if (!isFeverMode)
                    {
                        bonus = GetBonusCredit(sellPrice1);
                        sellPrice1 += bonus;
                        credit += sellPrice1;
                    }
                    else
                    {
                        credit += sellPrice1;
                    }
                }

                revenue += sellPrice1 + chipPrice;
                if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Hero)
                {
                    tendency += 10;
                }
                else if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Villain)
                {
                    tendency -= 10;
                }

                mgr2.orderState = GameSceneMgr.OrderState.Succeed;
                break;
            case 3:
                fame += 25;
                int sellPrice2 = GameMgr.In.currentBluePrint.sellPrice;
                if (isFeverMode)
                {
                    sellPrice2 = (int)(sellPrice2 * 0.4f) + 50;
                    bonus = 50;
                }

                if (!isFeverMode)
                {
                    bonus = GetBonusCredit(sellPrice2, 0.1f);
                    credit += sellPrice2 + bonus;
                    mgr2.goldText.text = GameMgr.In.credit.ToString();
                }
                else
                {
                    credit += sellPrice2;
                }

                revenue += sellPrice2 + chipPrice;

                if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Hero)
                {
                    tendency += 25;
                }
                else if (GameMgr.In.currentOrder.camp == OrderTable.Camp.Villain)
                {
                    tendency -= 25;
                }

                mgr2.orderState = GameSceneMgr.OrderState.Succeed;
                break;
        }

        if (isFeverMode)
        {
            feverModeFame += fame;
            feverModeRevenue += revenue;
            feverModeCredit += credit;
            GameMgr.In.dayChipUseCost -= chipPrice;
            GameMgr.In.dayBonusRevenue += bonus;
        }
        else
        {
            if (!isTutorial)
            {
                GameMgr.In.fame += fame;
                GameMgr.In.dayFame += fame;
                GameMgr.In.tendency += tendency;
                GameMgr.In.dayTendency += tendency;
                GameMgr.In.credit += credit;
                GameMgr.In.dayRevenue += revenue;
                GameMgr.In.dayChipUseCost -= chipPrice;
                GameMgr.In.dayBonusRevenue += bonus;
                mgr2.FameUIFill();
                mgr2.TendUIMove();
                mgr2.goldText.text = GameMgr.In.credit.ToString();
            }
        }

        if (GameMgr.In.week > 1)
        {
            GameMgr.In.AdjustFeverModeProbability(score);
        }

        if (isTutorial)
        {
            isTutorial = false;
        }

        if (OnMakingDone != null)
        {
            OnMakingDone.Invoke(score);
            OnMakingDone = null;
        }
    }

    private void ClearPuzzle()
    {
        for (int i = puzzleChipParent.childCount - 1; 0 <= i; i--)
        {
            DestroyImmediate(puzzleChipParent.GetChild(i).gameObject);
        }

        pressedChipIndex = -1;
        currentSelectedChip = null;
        currentSelectedChipData = null;
        prevMousePos = Vector3.zero;

        selectedChipName.text = string.Empty;
        selectedChipPrice.text = string.Empty;
        selectedChipDesc.text = string.Empty;
        needCredit.text = "0";
        foreach (var chip in chipList)
        {
            chip.parentImage.sprite = chipBackgroundOffSprite;
        }

        foreach (var obj in puzzleFrameList)
        {
            Destroy(obj.gameObject);
        }
        puzzleFrameList.Clear();

        currentChipInPuzzleDic.Clear();
        currentAbilityInPuzzleDic.Clear();
        chipObjDic.Clear();
        currentTagCnt = 0;
        filterAbilityKeyList.Clear();
        foreach (var filter in abilityFilterList)
        {
            filter.image.sprite = filter.offSprite;
            filter.isOn = false;
        }
        foreach (var tag in abilityTagList)
        {
            tag.abilityKey = string.Empty;
            tag.image.sprite = tag.offSprite;
            tag.textForTag.text = string.Empty;
            tag.button.interactable = false;
            tag.button.onClick.RemoveAllListeners();
        }
        if (!isAscending)
        {
            OnClickSortTarget();
        }

        currentAbilityText.text = string.Empty;
        usedChipText.text = string.Empty;
    }

    private bool IsWeaponRequiredAbilitySatisfied()
    {
        foreach (var requiredAbility in GameMgr.In.currentBluePrint.requiredChipAbilityList)
        {
            var targetAbility = currentAbilityInPuzzleDic.FirstOrDefault(x => x.Key.abilityKey.Equals(requiredAbility.abilityKey));
            if (targetAbility.Equals(default(KeyValuePair<Ability, int>)))
            {
                Debug.Log("무기 필수조건 불충족");
                return false;
            }

            if (targetAbility.Value < requiredAbility.count)
            {
                Debug.Log("무기 필수조건 불충족");
                return false;
            }
        }
        return true;
    }

    private bool IsOrderRequirementSatisfied()
    {
        if (isFeverMode) return true;

        // check blueprint
        if (GameMgr.In.currentOrder.requiredBlueprintKeyList.Count > 0)
        {
            if (!GameMgr.In.currentOrder.requiredBlueprintKeyList.Contains(GameMgr.In.currentBluePrint.bluePrintKey))
            {
                Debug.Log("청사진 요구조건 불충족");
                return false;
            }
        }

        // check ability
        if (GameMgr.In.currentOrder.gimmick != OrderTable.Gimmick.SatisfyOneRequest)
        {
            foreach (var requiredAbility in GameMgr.In.currentOrder.requiredAbilityList)
            {
                var targetAbility = currentAbilityInPuzzleDic.FirstOrDefault(x => x.Key.abilityKey.Equals(requiredAbility.abilityKey));
                if (targetAbility.Equals(default(KeyValuePair<Ability, int>)))
                {
                    Debug.Log("능력치 요구조건 불충족");
                    return false;
                }

                if (targetAbility.Value < requiredAbility.count)
                {
                    Debug.Log("능력치 요구조건 불충족");
                    return false;
                }
            }
        }


        // check condition
        if (!GameMgr.In.orderTable.IsConditionMatched(enabledFrameCnt, GetTotalSizeOfChips(), currentAbilityInPuzzleDic, GameMgr.In.currentOrder.condition))
        {
            Debug.Log("상태 요구조건 불충족");
            return false;
        }

        // check gimmick
        if (!GameMgr.In.orderTable.IsGimmickMatched(puzzleFrameList, currentAbilityInPuzzleDic))
        {
            Debug.Log("특수요구조건 불충족");
            return false;
        }

        return true;
    }

    private bool isPerfectState()
    {
        bool isPerfect = GetTotalSizeOfChips() == enabledFrameCnt;
        if (!isPerfect) Debug.Log("완벽한 상태가 아님");
        return isPerfect;
    }

    private bool hasBasicChipsets()
    {
        bool hasAttack = false;
        bool hasAcuity = false;
        foreach (var chip in currentChipInPuzzleDic.Keys)
        {
            if (!hasAttack && chip.chipKey.Equals("c_attack"))
            {
                hasAttack = true;
            }
            if (!hasAcuity && chip.chipKey.Equals("c_acuity"))
            {
                hasAcuity = true;
            }

            if (hasAttack && hasAcuity) return true;
        }
        return false;
    }

    private int GetBonusCredit(int sellPrice, float additionalBonus = -1f)
    {
        int bonus = 0;

        if (additionalBonus != -1f)
        {
            bonus += Mathf.FloorToInt(sellPrice * additionalBonus);
        }

        if (GameMgr.In.fame >= 801)
        {
            bonus += Mathf.FloorToInt(sellPrice * 0.1f);
        }
        else if (GameMgr.In.fame >= 601)
        {
            bonus += Mathf.FloorToInt(sellPrice * 0.06f);
        }
        else if (GameMgr.In.fame >= 401)
        {
            bonus += Mathf.FloorToInt(sellPrice * 0.04f);
        }
        else if (GameMgr.In.fame >= 201)
        {
            bonus += Mathf.FloorToInt(sellPrice * 0.02f);
        }

        return bonus;
    }

    private int GetTotalSizeOfChips()
    {
        int totalSize = 0;
        foreach (var frame in puzzleFrameList)
        {
            if (frame.pfd.patternNum == 0) continue;
            if (frame.pfd.chipList.Count == 0) continue;
            if (string.IsNullOrEmpty(frame.pfd.chipList[0].chipKey)) continue;

            totalSize++;
        }

        return totalSize;
    }

    private void EndFeverMode()
    {
        isFeverMode = false;
        isGamePlaying = false;
        GameMgr.In.feverModeProbability = 0;
        GameMgr.In.continuousSuccessCnt = 0;
        GameMgr.In.continuousPerfectCnt = 0;

        particleParentObj.SetActive(false);
        gageParentObj.SetActive(false);
        filterAndSortParent.SetActive(true);
        dragImg.gameObject.SetActive(false);

        GameMgr.In.fame += feverModeFame;
        GameMgr.In.dayFame += feverModeFame;
        GameMgr.In.credit += feverModeCredit;
        GameMgr.In.dayRevenue += feverModeRevenue;

        feverModeFame = 0;
        feverModeRevenue = 0;
        feverModeCredit = 0;

        ClearPuzzle();

        gameObject.SetActive(false);
    }

    private void StartNewFeverModePuzzle()
    {
        var index = UnityEngine.Random.Range(0, orderableBlueprintList.Count);

        var key = orderableBlueprintList[index].bluePrintKey;
        GameMgr.In.currentBluePrint = GameMgr.In.GetWeapon(key);
        StartFeverModePuzzle();
    }

    private void OnFeverModeMakingDone(int result)
    {
        isPuzzleCompleted = true;

        normalParticleParentObj.SetActive(false);
        resultParticleParentObj.SetActive(true);
        switch (result)
        {
            case 1:
                whiteParticleParentObj.SetActive(true);
                blueParticleParentObj.SetActive(false);
                purpleParticleParentObj.SetActive(false);
                break;
            case 2:
                whiteParticleParentObj.SetActive(false);
                blueParticleParentObj.SetActive(true);
                purpleParticleParentObj.SetActive(false);
                succeedPuzzleCnt++;
                break;
            case 3:
                whiteParticleParentObj.SetActive(false);
                blueParticleParentObj.SetActive(false);
                purpleParticleParentObj.SetActive(true);
                succeedPuzzleCnt++;
                break;
        }

        if (resultParticleRoutine != null)
        {
            StopCoroutine(resultParticleRoutine);
        }

        dragImg.gameObject.SetActive(false);

        resultParticleRoutine = StartCoroutine(ReturnToNormalParticles());
    }

    public IEnumerator StartFeverMode()
    {
        succeedPuzzleCnt = 0;
        isFeverMode = true;
        isTutorial = false;
        isGamePlaying = true;
        particleParentObj.SetActive(true);
        gageParentObj.SetActive(true);
        orderText.text = "제한시간 내에 최대한 많은 무기를 만드세요!";

        filterAndSortParent.SetActive(false);

        foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in bpc.bluePrintList)
            {
                if (bp.createEnable) orderableBlueprintList.Add(bp);
            }
        }

        StartCoroutine(StartTimeLimitGaging());

        for (int i = 0; i < maxPuzzleCnt; i++)
        {
            StartNewFeverModePuzzle();
            OnMakingDone += OnFeverModeMakingDone;

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

        EndFeverMode();
    }

    private IEnumerator StartTimeLimitGaging()
    {
        float gageWidth = gageRectTr.sizeDelta.x;
        float minPosX = maxPosX - Mathf.Abs(gageWidth);
        float currentPercent = 1;
        Vector2 currentPos = gageRectTr.anchoredPosition;
        currentPos.x = maxPosX;
        gageRectTr.anchoredPosition = currentPos;

        while (currentPos.x >= minPosX)
        {
            if (!isGamePlaying)
            {
                yield break;
            }

            float percentAtThisFrame = Time.deltaTime / maxTime;
            currentPos.x -= percentAtThisFrame * gageWidth;
            currentPercent -= percentAtThisFrame;
            gageRectTr.anchoredPosition = currentPos;

            if (currentPercent > 0.5f)
            {
                if (!greenParticleParentObj.activeSelf)
                {
                    greenParticleParentObj.SetActive(true);
                    yellowParticleParentObj.SetActive(false);
                    redParticleParentObj.SetActive(false);
                }
            }
            else if (currentPercent < 0.15f)
            {
                if (!redParticleParentObj.activeSelf)
                {
                    greenParticleParentObj.SetActive(false);
                    yellowParticleParentObj.SetActive(false);
                    redParticleParentObj.SetActive(true);
                }
            }
            else
            {
                if (!yellowParticleParentObj.activeSelf)
                {
                    greenParticleParentObj.SetActive(false);
                    yellowParticleParentObj.SetActive(true);
                    redParticleParentObj.SetActive(false);
                }
            }

            yield return null;
        }

        EndFeverMode();
    }

    private IEnumerator ReturnToNormalParticles()
    {
        yield return new WaitForSeconds(1);
        normalParticleParentObj.SetActive(true);
        resultParticleParentObj.SetActive(false);
        resultParticleRoutine = null;
    }

    [ContextMenu("LogPuzzleFrameDatas")]
    private void LogPuzzleFrameDatas()
    {
        StringBuilder sb2 = new StringBuilder();
        int i = 0;
        foreach (var frame in currPuzzle.frameDataTable)
        {
            if (frame != null && frame.chipList.Count > 0)
            {
                sb2.Append(frame.chipList[0].chipKey).Append(", ");
            }
            else
            {
                sb2.Append("0, ");
            }
            i++;
            if (i == 8)
            {
                sb2.Append("\n");
                i = 0;
            }
        }
        Debug.Log(sb2.ToString());
    }

}

