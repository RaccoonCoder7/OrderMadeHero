using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Chip;

public class GameSceneMgr : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<TextAsset> csvList;
    public GridLayoutGroup puzzleGrid;
    public RectTransform chipPanelRectTr;
    public EventSystem es;
    public Canvas canvas;
    public RawImage dragImg;
    public int chipCountX;
    public int chipCountY;
    public float chipSize;
    public PuzzleFrame puzzleFrame;
    public List<Texture> textureList = new List<Texture>();
    public List<Chip> chipList = new List<Chip>();
    public Dictionary<int, int> chipInventory = new Dictionary<int, int>();
    public Button makingDone;
    public GameSceneMgr2 mgr2;
    public RequiredAbilityObject requiredAbilityObject;
    public Transform requiredAbilityTextParent;

    private RectTransform puzzleGridRectTr;
    private RectTransform dragImgRectTr;
    private RawImage background;
    private Puzzle currPuzzle;
    private Chip[,] chipFrameTable;
    private Chip currentSelectedChip;
    private CanvasScaler canvasScaler;
    private GraphicRaycaster ray;
    private PointerEventData ped;
    private bool isFromPuzzle;
    private Vector3 originDragImgRectTr;
    private List<PuzzleFrame> puzzleFrameList = new List<PuzzleFrame>();
    private Dictionary<string, RequiredAbilityObject> requiredAbilityObjectDic = new Dictionary<string, RequiredAbilityObject>();
    private Dictionary<string, int> currentChipDataDic = new Dictionary<string, int>();

    public class Puzzle
    {
        public int x;
        public int y;
        public List<ChipAbility> requiredChipAbilityList = new List<ChipAbility>();
        public PuzzleFrameData[,] frameDataTable;
    }

    [System.Serializable]
    public class PuzzleFrameData
    {
        public int patternNum = 0;
        public int x;
        public int y;
        public Chip chip;

        public PuzzleFrameData(int patternNum, int x, int y)
        {
            this.patternNum = patternNum;
            this.x = x;
            this.y = y;
        }
    }

    private bool temp;
    void Awake()
    {
        if (temp) return;
        temp = true;

        makingDone.onClick.AddListener(OnClickMakingDone);

        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        ray = canvas.GetComponent<GraphicRaycaster>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        ped = new PointerEventData(es);
        currPuzzle = GetPuzzle(0);
        currPuzzle.requiredChipAbilityList.Add(new ChipAbility("A", 1));
        currPuzzle.requiredChipAbilityList.Add(new ChipAbility("B", 1));
        currPuzzle.requiredChipAbilityList.Add(new ChipAbility("C", 1));
        InstantiateRequiredAbilityText();
        var width = puzzleGridRectTr.rect.width / currPuzzle.x;
        var height = puzzleGridRectTr.rect.height / currPuzzle.y;
        var size = width < height ? width : height;
        puzzleGrid.cellSize = new Vector2(size, size);
        puzzleGrid.constraintCount = currPuzzle.x;
        InstantiateFrames(currPuzzle.frameDataTable);
        background.texture = textureList[0];

        SetTestDataToChipInventory();
        RefreshChipPanel();

        // 테스트코드
        StringBuilder sb = new StringBuilder();
        int i = 0;
        foreach (var chipframe in chipFrameTable)
        {
            if (chipframe != null)
            {
                sb.Append(chipframe.chipKey).Append(", ");
            }
            else
            {
                sb.Append("0, ");
            }
            i++;
            if (i == 4)
            {
                sb.Append("\n");
                i = 0;
            }
        }
        Debug.Log(sb.ToString());
        // 테스트코드
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
                    pos.x += dragImgRectTr.rect.width;
                }
                else if (angle.z < 91)
                {
                    angle.z = 0;
                    pos.y += dragImgRectTr.rect.height;
                }
                else if (angle.z < 181)
                {
                    angle.z = 90;
                    pos.x -= dragImgRectTr.rect.width;
                }
                else if (angle.z < 271)
                {
                    angle.z = 180;
                    pos.y -= dragImgRectTr.rect.height;
                }
                dragImgRectTr.localEulerAngles = angle;
                dragImgRectTr.localPosition = pos;

                currentSelectedChip.RotateRight();
            }
        }
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

    private Puzzle GetPuzzle(int index)
    {
        Puzzle puzzle = new Puzzle();
        var lines = csvList[index].text.Split('\n');
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
                    Debug.Log("퍼즐조각정보 로드 실패. puzzle" + index + ": " + i + ", " + j);
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
            if (frame.pfd.patternNum == 0)
            {
                frame.image.enabled = false;
                continue;
            }
            frame.image.texture = textureList[frameData.patternNum];
            puzzleFrameList.Add(frame);
        }
    }

    private void SetTestDataToChipInventory()
    {
        //     chipInventory.Add(1, 1);
        //     chipInventory.Add(2, 1);
        //     chipInventory.Add(3, 2);
        //     chipInventory.Add(4, 1);
        //     chipInventory.Add(5, 1);
        //     chipInventory.Add(6, 1);
        //     chipInventory.Add(7, 2);
        chipInventory.Add(3, 1);
        chipInventory.Add(4, 1);
        chipInventory.Add(5, 1);
    }

    private void RefreshChipPanel()
    {
        var sizeX = chipSize * chipCountX;
        var sizeY = chipSize * chipCountY;
        chipPanelRectTr.sizeDelta = new Vector2(sizeX, sizeY);

        chipFrameTable = new Chip[chipCountY, chipCountX];
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                chipFrameTable[i, j] = new Chip();
            }
        }

        foreach (var key in chipInventory.Keys)
        {
            var chip = GetChipPrefab(key);
            if (chip)
            {
                var chipInstance = Instantiate(chip, chipPanelRectTr.transform);
                chipInstance.name = chip.name;
                SetChipToPanel(chipInstance, chipInventory[key]);
            }
        }
    }

    private Chip GetChipPrefab(int key)
    {
        return chipList.Find(x => x.chipKey.Equals(key));
    }

    private void SetChipToPanel(Chip chip, int chipCount)
    {
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                if (chipFrameTable[i, j] != null) continue;
                chipFrameTable[i, j] = chip;
                chip.count.text = chipCount.ToString();
                chip.chipCount = chipCount;
                chip.rectTr.sizeDelta = new Vector2(chipSize, chipSize);
                chip.rectTr.anchoredPosition = new Vector3(chipSize * j, -chipSize * i, 0);
                return;
            }
        }
        Destroy(chip.gameObject);
    }

    private List<PuzzleFrameData> GetFittableFrameList(PuzzleFrameData[,] targetTable, Chip chip, int x, int y)
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

                if (targetTable[y + j, x + i].chip != null && targetTable[y + j, x + i].chip.chipCount != 0)
                {
                    return null;
                }

                pfdList.Add(targetTable[y + j, x + i]);
            }
        }
        return pfdList;
    }

    private Chip GetChipAtPos(Vector2 pos)
    {
        pos /= chipSize;
        return chipFrameTable[(int)-pos.y, (int)pos.x];
    }

    public void OnBeginDrag(PointerEventData eventData)
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
            Chip targetChip = null;
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
                targetChip = results[0].gameObject.GetComponent<Chip>();
            }

            if (targetChip != null && targetChip.chipCount > 0)
            {
                targetChip.SaveOriginRow();
                dragImg.texture = targetChip.image.texture;
                dragImgRectTr.sizeDelta = new Vector2(targetChip.rowNum, targetChip.colNum) * chipSize;
                var offset = new Vector3(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, 0) / 2;
                offset -= new Vector3(-dragImgRectTr.sizeDelta.x, dragImgRectTr.sizeDelta.y, 0) / 2;
                dragImgRectTr.localPosition = (Vector3)eventData.position - offset;
                dragImgRectTr.localEulerAngles = targetChip.rectTr.localEulerAngles;
                var pos = originDragImgRectTr = dragImgRectTr.localPosition;
                if (dragImgRectTr.localEulerAngles.z < 1)
                {
                }
                else if (dragImgRectTr.localEulerAngles.z < 91)
                {
                    pos.y -= targetChip.rectTr.rect.height;
                }
                else if (dragImgRectTr.localEulerAngles.z < 181)
                {
                    pos.y -= targetChip.rectTr.rect.height;
                    pos.x += targetChip.rectTr.rect.width;
                }
                else if (dragImgRectTr.localEulerAngles.z < 271)
                {
                    pos.x += targetChip.rectTr.rect.width;
                }
                dragImgRectTr.localPosition = pos;

                currentSelectedChip = targetChip;
                dragImg.gameObject.SetActive(true);
                targetChip.chipCount--;
                if (targetChip.chipCount <= 0)
                {
                    targetChip.gameObject.SetActive(false);
                }
                else
                {
                    targetChip.count.text = targetChip.chipCount.ToString();
                }
            }
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

            var offset = new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize / 2;
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
                        foreach (var c in chipFrameTable)
                        {
                            if (c.chipKey.Equals(currentSelectedChip.chipKey))
                            {
                                c.chipCount++;
                                c.gameObject.SetActive(true);
                                c.count.text = c.chipCount.ToString();
                                c.ResetCol();
                                break;
                            }
                        }

                        if (currentSelectedChip.chipCount <= 0)
                        {
                            DestroyImmediate(currentSelectedChip.gameObject);
                        }
                        RefreshWeaponPowerData();
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
                    if (frame != null && frame.pfd.patternNum != 0)
                    {
                        var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChip, frame.pfd.x, frame.pfd.y);
                        if (fittableFrames != null)
                        {
                            var chipInstance = Instantiate(currentSelectedChip, chipPanelRectTr.transform);
                            chipInstance.name = currentSelectedChip.name;
                            chipInstance.gameObject.SetActive(true);
                            for (int i = 0; i < fittableFrames.Count; i++)
                            {
                                fittableFrames[i].chip = chipInstance;
                            }
                            chipInstance.countPlate.SetActive(false);
                            chipInstance.transform.parent = frame.transform;
                            chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                            chipInstance.rectTr.anchoredPosition = Vector3.zero;
                            chipInstance.chipCount = 1;

                            chipInstance.rectTr.localEulerAngles = angle;
                            var pos = chipInstance.rectTr.localPosition;
                            if (angle.z < 1)
                            {
                            }
                            else if (angle.z < 91)
                            {
                                pos.y -= dragImgRectTr.rect.height;
                            }
                            else if (angle.z < 181)
                            {
                                pos.y -= dragImgRectTr.rect.height;
                                pos.x += dragImgRectTr.rect.width;
                            }
                            else if (angle.z < 271)
                            {
                                pos.x += dragImgRectTr.rect.width;
                            }
                            chipInstance.rectTr.localPosition = pos;

                            if (currentSelectedChip.chipCount <= 0 && isFromPuzzle)
                            {
                                DestroyImmediate(currentSelectedChip.gameObject);
                            }
                            RefreshWeaponPowerData();
                            success = true;
                        }
                    }
                }
            }

            if (!success)
            {
                currentSelectedChip.chipCount++;
                if (currentSelectedChip.chipCount > 0)
                {
                    currentSelectedChip.gameObject.SetActive(true);
                }
                currentSelectedChip.count.text = currentSelectedChip.chipCount.ToString();
                currentSelectedChip.ResetCol();
            }
            currentSelectedChip = null;
        }
        isFromPuzzle = false;
    }

    private void InstantiateRequiredAbilityText()
    {
        foreach (var ability in currPuzzle.requiredChipAbilityList)
        {
            var obj = Instantiate(requiredAbilityObject, requiredAbilityTextParent);
            obj.text.text = "0/" + ability.value;
            requiredAbilityObjectDic.Add(ability.key, obj);
        }
    }

    private void RefreshWeaponPowerData()
    {
        currentChipDataDic.Clear();
        foreach (var puzzleFrame in puzzleFrameList)
        {
            if (puzzleFrame.transform.childCount > 0)
            {
                var child = puzzleFrame.transform.GetChild(0);
                if (child)
                {
                    var chip = child.GetComponent<Chip>();
                    if (chip)
                    {
                        foreach (var ability in chip.chipAbilityList)
                        {
                            if (!currentChipDataDic.ContainsKey(ability.key))
                            {
                                currentChipDataDic.Add(ability.key, ability.value);
                                continue;
                            }
                            currentChipDataDic[ability.key] += ability.value;
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
                return;
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

        makingDone.gameObject.SetActive(success);
    }

    public void OnClickMakingDone()
    {
        makingDone.gameObject.SetActive(false);
        mgr2.OnMakingDone();
    }
}
