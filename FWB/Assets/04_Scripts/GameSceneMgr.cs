using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private RectTransform puzzleGridRectTr;
    private RectTransform dragImgRectTr;
    private RawImage background;
    private Puzzle currPuzzle;
    private Chip[,] chipFrameTable;
    private Chip currentSelectedChip;
    private List<PuzzleFrameData> currMovingFrameList = new List<PuzzleFrameData>();

    private CanvasScaler canvasScaler;
    private GraphicRaycaster ray;
    private PointerEventData ped;

    public class Puzzle
    {
        public int x;
        public int y;
        public PuzzleFrameData[,] frameDataTable;
    }

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

    void Start()
    {
        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        ray = canvas.GetComponent<GraphicRaycaster>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        ped = new PointerEventData(es);
        currPuzzle = GetPuzzle(0);
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
        }
    }

    private void SetTestDataToChipInventory()
    {
        chipInventory.Add(1, 1);
        chipInventory.Add(2, 1);
        chipInventory.Add(3, 2);
        chipInventory.Add(4, 1);
        chipInventory.Add(5, 1);
        chipInventory.Add(6, 1);
        chipInventory.Add(7, 2);
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
                    Debug.Log(1);
                    return null;
                }

                if (x + i >= currPuzzle.x || y + j >= currPuzzle.y)
                {
                    Debug.Log(2);
                    return null;
                }

                if (targetTable[y + j, x + i].chip != null && targetTable[y + j, x + i].chip.chipCount != 0)
                {
                    Debug.Log(3);
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
        ped.position = eventData.pressPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                Debug.Log(result.gameObject.name);
            }
            
            List<Chip> chipList = new List<Chip>();
            foreach (var result in results)
            {
                Chip tempChip = result.gameObject.GetComponent<Chip>();
                if (tempChip != null)
                {
                    chipList.Add(tempChip);
                }
            }

            Chip targetChip = null;
            if (chipList.Count == 1)
            {
                targetChip = chipList[0];
            }
            else if (chipList.Count > 1)
            {
                foreach (var chipElement in chipList)
                {
                    // GetChipAtPos
                }
            }

            if (targetChip != null && targetChip.chipCount > 0)
            {
                Debug.Log(2);
                dragImg.texture = targetChip.image.texture;
                dragImgRectTr.sizeDelta = new Vector2(targetChip.rowNum, targetChip.colNum) * chipSize;
                var offset = new Vector3(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, 0) / 2;
                offset -= new Vector3(-dragImgRectTr.sizeDelta.x, dragImgRectTr.sizeDelta.y, 0) / 2;
                dragImgRectTr.localPosition = (Vector3)eventData.position - offset;
                currentSelectedChip = targetChip;
                dragImg.gameObject.SetActive(true);
                targetChip.chipCount--;
                if (targetChip.chipCount <= 0)
                {
                    if (results.Count > 1)
                    {
                        if (results[1].gameObject.name.Contains("PuzzleFrame"))
                        {
                            foreach (var frameData in currPuzzle.frameDataTable)
                            {
                                if (frameData.chip == null)
                                {
                                    continue;
                                }
                                if (frameData.chip.Equals(targetChip))
                                {
                                    frameData.chip = null;
                                    currMovingFrameList.Add(frameData);
                                }
                            }
                        }
                        // else if (results[1].gameObject.name.Contains("ChipPanel"))
                        // {
                        //     foreach (var chipFrame in chipFrameTable)
                        //     {
                        //         if (chipFrame.Equals(chip))
                        //         {

                        //         }
                        //     }
                        // }
                    }
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
        if (currentSelectedChip != null)
        {
            dragImgRectTr.localPosition += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSelectedChip != null)
        {
            dragImg.gameObject.SetActive(false);
            bool success = false;

            var offset = new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize / 2;
            ped.position = Input.mousePosition + new Vector3(-offset.x, offset.y, 0);
            List<RaycastResult> results = new List<RaycastResult>();
            ray.Raycast(ped, results);
            if (results.Count > 0)
            {
                var frame = results[0].gameObject.GetComponent<PuzzleFrame>();
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
                        success = true;
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

                foreach (var frame in currMovingFrameList)
                {
                    frame.chip = currentSelectedChip;
                }
            }
            currentSelectedChip = null;
            currMovingFrameList.Clear();
        }
    }
}
