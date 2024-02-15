using System;
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
    public Text creditText;
    public Text orderText;
    public Text bluePrintName;
    public Text currentAbilityText;
    public Text requiredAbilityText;
    public Text usedChipText;
    public Text selectedChipName;
    public Text selectedChipPrice;
    public Text selectedChipDesc;
    public int chipCountX;
    public int chipCountY;
    public float chipSize;
    public PuzzleFrame puzzleFrame;
    public List<Texture> textureList = new List<Texture>();
    // public List<ChipObj> chipList = new List<ChipObj>();
    public List<ChipObj> newChipList = new List<ChipObj>();
    public List<AbilityFilterUI> abilityFilterList = new List<AbilityFilterUI>();
    public List<AbilityFilterUI> abilityTagList = new List<AbilityFilterUI>();
    public Dictionary<string, int> chipInventory = new Dictionary<string, int>();
    public Button makingDone;
    public Button sortTargetBtn;
    public Button sortOrderBtn;
    public Button revertChips;
    public GameSceneMgr mgr2;
    public RequiredAbilityObject requiredAbilityObject;
    public Transform requiredAbilityTextParent;
    public Action OnMakingDone;
    [HideInInspector]
    public bool isTutorial = true;

    private RectTransform puzzleGridRectTr;
    private RectTransform dragImgRectTr;
    private RawImage background;
    private Puzzle currPuzzle;
    private ChipObj[,] chipFrameTable;
    private ChipObj currentSelectedChip;
    private Chip currentSelectedChipData;
    private CanvasScaler canvasScaler;
    private GraphicRaycaster ray;
    private PointerEventData ped;
    private bool isFromPuzzle;
    private bool isAscending;
    private Vector3 originDragImgRectTr;
    private SpriteChange sortOrderSC;
    private List<PuzzleFrame> puzzleFrameList = new List<PuzzleFrame>();
    private Dictionary<string, RequiredAbilityObject> requiredAbilityObjectDic = new Dictionary<string, RequiredAbilityObject>();
    private Dictionary<string, int> currentChipDataDic = new Dictionary<string, int>();
    private List<GameObject> instantiatedChipList = new List<GameObject>();
    private List<string> filterAbilityKeyList = new List<string>();
    private List<string> creatableChipKeyList = new List<string>();
    private Dictionary<Chip, int> currentChipInPuzzleDic = new Dictionary<Chip, int>();
    private Dictionary<Ability, int> currentAbilityInPuzzleDic = new Dictionary<Ability, int>();

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
        public ChipObj chip;

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

        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        ray = canvas.GetComponent<GraphicRaycaster>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        ped = new PointerEventData(es);
        sortOrderSC = sortOrderBtn.GetComponent<SpriteChange>();

        for (int i = 0; i < abilityFilterList.Count; i++)
        {
            int tempNum = i;
            abilityFilterList[tempNum].button.onClick.AddListener(() =>
            {
                var filter = abilityFilterList[tempNum];
                if (filter.isOn)
                {
                    filter.image.sprite = filter.offSprite;
                    filterAbilityKeyList.Remove(filter.abilityKey);

                    var tag = abilityTagList.Find(x => x.abilityKey.Equals(filter.abilityKey));
                    tag.abilityKey = string.Empty;
                    tag.transform.SetSiblingIndex(2);
                    tag.image.sprite = tag.offSprite;
                    tag.textForTag.text = string.Empty;
                    tag.button.interactable = false;
                    tag.button.onClick.RemoveAllListeners();
                }
                else
                {
                    if (filterAbilityKeyList.Count >= 3)
                    {
                        return;
                    }
                    filter.image.sprite = filter.onSprite;
                    filterAbilityKeyList.Add(filter.abilityKey);

                    var tag = abilityTagList.First(x => string.IsNullOrEmpty(x.abilityKey));
                    tag.abilityKey = filter.abilityKey;
                    tag.transform.SetSiblingIndex(filterAbilityKeyList.Count - 1);
                    tag.image.sprite = tag.onSprite;
                    tag.textForTag.text = GameMgr.In.GetAbility(filter.abilityKey).name;
                    tag.button.interactable = true;
                    tag.button.onClick.AddListener(() =>
                    {
                        filter.button.onClick.Invoke();
                    });
                }
                filter.isOn = !filter.isOn;

                foreach (var chip in newChipList)
                {
                    if (!creatableChipKeyList.Contains(chip.chipKey))
                    {
                        continue;
                    }

                    bool isActive = true;
                    var chipData = GameMgr.In.GetChip(chip.chipKey);
                    foreach (var filterAbilityKey in filterAbilityKeyList)
                    {
                        var targetAbility = chipData.abilityList.Find(x => x.abilityKey.Equals(filterAbilityKey));
                        if (targetAbility == null)
                        {
                            isActive = false;
                            break;
                        }
                    }
                    chip.backgroundSC.gameObject.SetActive(isActive);
                }
            });
        }
    }

    void Update()
    {
        if (currentSelectedChip != null)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                var angle = dragImgRectTr.localEulerAngles;
                var pos = dragImgRectTr.localPosition;
                if (angle.z < 1)
                {
                    angle.z = 270;
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2;
                }
                else if (angle.z < 91)
                {
                    angle.z = 0;
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2;
                }
                else if (angle.z < 181)
                {
                    angle.z = 90;
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2;
                }
                else if (angle.z < 271)
                {
                    angle.z = 180;
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2;
                }
                dragImgRectTr.localEulerAngles = angle;
                dragImgRectTr.localPosition = pos;

                currentSelectedChip.RotateRight();
            }
        }
    }

    public void StartPuzzle()
    {
        // TODO: 청사진 정보에 맞게 chipSize, currPuzzle 세팅하기
        chipSize = 64;
        creditText.text = GameMgr.In.credit.ToString();
        SetOrderDatas();
        SetBluePrintDatas();
        SetPuzzle();
        SetChipDatas();
    }

    public void OnClickMakingDone()
    {
        mgr2.popupChatPanel.SetActive(false);

        // mgr2.orderState = GetWeaponPowerResult() ? GameSceneMgr.OrderState.Succeed : GameSceneMgr.OrderState.Failed;
        if (OnMakingDone != null)
        {
            OnMakingDone.Invoke();
            OnMakingDone = null;
        }

        foreach (var key in requiredAbilityObjectDic.Keys)
        {
            Destroy(requiredAbilityObjectDic[key].gameObject);
        }
        requiredAbilityObjectDic.Clear();

        foreach (var obj in puzzleFrameList)
        {
            Destroy(obj.gameObject);
        }

        foreach (var obj in instantiatedChipList)
        {
            Destroy(obj);
        }

        puzzleFrameList.Clear();
        chipInventory.Clear();
        isTutorial = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

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
                    targetChip = result.gameObject.GetComponent<PuzzleFrame>().pfd.chip;
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
                    if (!isFromPuzzle)
                    {
                        selectedChipName.text = currentSelectedChipData.chipName;
                        selectedChipPrice.text = "개당 " + currentSelectedChipData.price + " c";
                        selectedChipDesc.text = currentSelectedChipData.desc;
                    }
                    return;
                }
            }
        }
        currentSelectedChipData = null;
        currentSelectedChip = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (currentSelectedChip != null)
        {
            if (!isFromPuzzle)
            {
                currentSelectedChip.SaveOriginRow();
            }
            dragImg.texture = currentSelectedChip.image.texture;
            if (currentSelectedChip.originRow.Length == currentSelectedChip.rowNum)
            {
                dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.rowNum, currentSelectedChip.colNum) * chipSize;
            }
            else
            {
                dragImgRectTr.sizeDelta = new Vector2(currentSelectedChip.colNum, currentSelectedChip.rowNum) * chipSize;
            }

            var offset = new Vector3(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, 0) / 2;
            dragImgRectTr.localPosition = (Vector3)eventData.position - offset;
            dragImgRectTr.localEulerAngles = currentSelectedChip.rectTr.localEulerAngles;
            var angle = dragImgRectTr.localEulerAngles;
            var pos = originDragImgRectTr = dragImgRectTr.localPosition;
            if (angle.z < 1)
            {
                pos.x = pos.x + (-dragImgRectTr.sizeDelta.x) / 2;
                pos.y = pos.y + (+dragImgRectTr.sizeDelta.y) / 2;
            }
            else if (angle.z < 91)
            {
                pos.x = pos.x + (-dragImgRectTr.sizeDelta.y) / 2;
                pos.y = pos.y + (-dragImgRectTr.sizeDelta.x) / 2;
            }
            else if (angle.z < 181)
            {
                pos.x = pos.x + (dragImgRectTr.sizeDelta.x) / 2;
                pos.y = pos.y + (-dragImgRectTr.sizeDelta.y) / 2;
            }
            else if (angle.z < 271)
            {
                pos.x = pos.x + (dragImgRectTr.sizeDelta.y) / 2;
                pos.y = pos.y + (dragImgRectTr.sizeDelta.x) / 2;
            }
            dragImgRectTr.localPosition = pos;

            dragImg.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (currentSelectedChip != null)
        {
            dragImgRectTr.localPosition += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        var angle = dragImgRectTr.localEulerAngles;
        if (currentSelectedChip != null)
        {
            dragImg.gameObject.SetActive(false);
            bool success = false;

            var x = (currentSelectedChip.rowNum - 1) / 2;
            var offset = (new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize) / 2;
            ped.position = Input.mousePosition + new Vector3(-offset.x, offset.y, 0);
            List<RaycastResult> results = new List<RaycastResult>();
            es.RaycastAll(ped, results);
            if (results.Count > 0)
            {
                bool isChipPanel = false;
                foreach (var result in results)
                {
                    if (result.gameObject.name.Contains("ChipPanel"))
                    {
                        isChipPanel = true;
                        break;
                    }
                }

                if (isChipPanel)
                {
                    if (isFromPuzzle)
                    {
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

                        // TODO: 클리어 여부 확인 + makingDone.gameObject.SetActive
                        if (isTutorial)
                        {
                            var result = GetWeaponPowerResult();
                            makingDone.gameObject.SetActive(result);
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
                                fittableFrames[i].chip = chipInstance;
                            }

                            chipInstance.transform.parent = frame.transform;
                            if (chipInstance.originRow.Length == currentSelectedChip.rowNum)
                            {
                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                            }
                            else
                            {
                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
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
                                RefreshWeaponPowerData();
                            }
                            else
                            {
                                DestroyImmediate(currentSelectedChip.gameObject);
                            }


                            // TODO: 클리어 여부 확인 + makingDone.gameObject.SetActive
                            if (isTutorial)
                            {
                                var result = GetWeaponPowerResult();
                                makingDone.gameObject.SetActive(result);
                            }
                            success = true;
                        }
                    }
                }
            }
            if (!success)
            {
                currentSelectedChip.ResetCol();
            }
            currentSelectedChip = null;
        }
        isFromPuzzle = false;
    }

    private void SetPuzzle()
    {
        currPuzzle = GetPuzzle();
        InstantiateFrames(currPuzzle.frameDataTable);
    }

    private Puzzle GetPuzzle()
    {
        Puzzle puzzle = new Puzzle();
        var lines = GameMgr.In.currentBluePrint.puzzleCsv.text.Split('\n');
        var lineList = new List<string>();
        foreach (var line in lines)
        {
            var trimLine = line.Trim();
            if (!string.IsNullOrEmpty(trimLine))
            {
                lineList.Add(trimLine);
            }
        }
        puzzle.y = lineList.Count;
        puzzle.x = lineList[0].Split(',').Length;
        puzzle.frameDataTable = new PuzzleFrameData[puzzle.y, puzzle.x];
        for (int i = 0; i < lineList.Count; i++)
        {
            var elements = lineList[i].Split(',');
            if (i == 0) puzzle.x = elements.Length;

            for (int j = 0; j < elements.Length; j++)
            {
                int targetNum = 0;
                if (!Int32.TryParse(elements[j], out targetNum))
                {
                    Debug.Log("퍼즐조각정보 로드 실패. puzzle" + GameMgr.In.currentBluePrint.puzzleCsv.text + ": " + i + ", " + j);
                    return null;
                }
                puzzle.frameDataTable[i, j] = new PuzzleFrameData(targetNum, j, i);
            }
        }

        return puzzle;
    }

    private void InstantiateFrames(PuzzleFrameData[,] frameDataTable)
    {
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
        var table = chip.row;
        for (int i = 0; i < table.Length; i++)
        {
            for (int j = 0; j < table[i].col.Length; j++)
            {
                if (!table[i].col[j])
                {
                    continue;
                }

                if (targetTable[y + j, x + i].patternNum == 0)
                {
                    return null;
                }

                if (x + i >= currPuzzle.x || y + j >= currPuzzle.y)
                {
                    return null;
                }

                if (targetTable[y + j, x + i].chip != null && !targetTable[y + j, x + i].chip.Equals(chip))
                {
                    return null;
                }

                pfdList.Add(targetTable[y + j, x + i]);
            }
        }
        return pfdList;
    }

    private bool GetWeaponPowerResult()
    {
        currentChipDataDic.Clear();
        foreach (var puzzleFrame in puzzleFrameList)
        {
            if (puzzleFrame.transform.childCount > 0)
            {
                var child = puzzleFrame.transform.GetChild(0);
                if (child)
                {
                    var chip = child.GetComponent<ChipObj>();
                    if (chip)
                    {
                        foreach (var ability in chip.chipAbilityList)
                        {
                            if (!currentChipDataDic.ContainsKey(ability.abilityKey))
                            {
                                currentChipDataDic.Add(ability.abilityKey, ability.count);
                                continue;
                            }
                            currentChipDataDic[ability.abilityKey] += ability.count;
                        }
                    }
                }
            }
        }

        bool success = true;
        foreach (var key in requiredAbilityObjectDic.Keys)
        {
            var splittedData = requiredAbilityObjectDic[key].text.text.Split('/');
            int targetNum = -1;
            if (!Int32.TryParse(splittedData[1], out targetNum))
            {
                Debug.Log("파싱 실패: " + targetNum);
                return false;
            }
            int result = 0;
            if (currentChipDataDic.ContainsKey(key))
            {
                result = currentChipDataDic[key];
                if (result < targetNum)
                {
                    success = false;
                }
            }
            else
            {
                result = 0;
                success = false;
            }
            requiredAbilityObjectDic[key].text.text = result + "/" + splittedData[1];
        }

        return success;
    }

    // private ChipObj GetChipAtPos(Vector2 pos)
    // {
    //     pos /= chipSize;
    //     return chipFrameTable[(int)-pos.y, (int)pos.x];
    // }

    private void SetOrderDatas()
    {
        var order = GameMgr.In.currentOrder;
        this.orderText.text = mgr2.mainChatText.text.Replace(" ▼", "").Replace("\n", " ");
        // TODO: 특수조건
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

    private void SetChipDatas()
    {
        var creatableChips = GameMgr.In.chipTable.chipList.FindAll(x => x.createEnable);
        creatableChipKeyList = new List<string>();
        foreach (var cc in creatableChips)
        {
            creatableChipKeyList.Add(cc.chipKey);
        }
        foreach (var chip in newChipList)
        {
            chip.backgroundSC.gameObject.SetActive(creatableChipKeyList.Contains(chip.chipKey));
        }
        SortChipList();
    }

    private void OnClickSortTarget()
    {
        isAscending = !isAscending;

        if (isAscending)
        {
            sortOrderSC.SetOnSprite();
        }
        else
        {
            sortOrderSC.SetOffSprite();
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
            orderedChipKeyList = chipList.OrderByDescending(x => x.price).ThenByDescending(x => x.chipKey).Select(x => x.chipKey).ToList();
        }

        for (int i = 0; i < orderedChipKeyList.Count; i++)
        {
            var matchedChip = newChipList.Find(x => x.chipKey.Equals(orderedChipKeyList[i]));
            if (matchedChip == null) continue;

            matchedChip.backgroundSC.transform.SetSiblingIndex(i);
        }
    }

    private void RefreshWeaponPowerData()
    {
        StringBuilder currAbilitySB = new StringBuilder();
        StringBuilder usedChipSB = new StringBuilder();

        foreach (var chip in currentChipInPuzzleDic.Keys)
        {
            usedChipSB.Append(chip.chipName).Append(" 칩셋 ").Append(currentChipInPuzzleDic[chip]).Append("개\n");

            currentAbilityInPuzzleDic = new Dictionary<Ability, int>();
            foreach (var ability in chip.abilityList)
            {
                var targetAbility = GameMgr.In.GetAbility(ability.abilityKey);
                if (currentAbilityInPuzzleDic.ContainsKey(targetAbility))
                {
                    currentAbilityInPuzzleDic[targetAbility] += 1;
                }
                else
                {
                    currentAbilityInPuzzleDic.Add(targetAbility, 1);
                }
            }
            foreach (var ability in currentAbilityInPuzzleDic.Keys)
            {
                currAbilitySB.Append(ability.name).Append("+").Append(currentAbilityInPuzzleDic[ability]).Append(" ");
            }
            if (currAbilitySB.Length > 0)
            {
                currAbilitySB.Length--;
            }
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
        foreach (var frame in puzzleFrameList)
        {
            foreach (Transform tr in frame.transform)
            {
                DestroyImmediate(tr.gameObject);
            }
        }

        currentChipInPuzzleDic.Clear();
        currentAbilityInPuzzleDic.Clear();
    }

    [ContextMenu("LogPuzzleFrameDatas")]
    private void LogPuzzleFrameDatas()
    {
        StringBuilder sb2 = new StringBuilder();
        int i = 0;
        foreach (var frame in currPuzzle.frameDataTable)
        {
            if (frame != null && frame.chip != null)
            {
                sb2.Append(frame.chip.chipKey).Append(", ");
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
