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
    private ChipFrameData[,] chipFrameTable;
    private ChipFrameData currentSelectedChipData;
    private GraphicRaycaster ray;
    private PointerEventData ped;

    public class Puzzle
    {
        public int x;
        public int y;
        public PuzzleFrameData[,] frameDataTable;
    }

    public class PuzzleFrameData : ChipFrameData
    {
        public int patternNum = 0;
        public int x;
        public int y;

        public PuzzleFrameData(int patternNum, int x, int y)
        {
            this.patternNum = patternNum;
            this.x = x;
            this.y = y;
        }
    }

    public class ChipFrameData
    {
        public Chip filledChip;
        public int chipCount;
    }

    void Start()
    {
        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        ray = canvas.GetComponent<GraphicRaycaster>();
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
            if (chipframe.filledChip != null)
            {
                sb.Append(chipframe.filledChip.chipKey).Append(", ");
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

        chipFrameTable = new ChipFrameData[chipCountY, chipCountX];
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                chipFrameTable[i, j] = new ChipFrameData();
            }
        }

        foreach (var key in chipInventory.Keys)
        {
            var chip = GetChipPrefab(key);
            if (chip)
            {
                var chipInstance = Instantiate(chip, chipPanelRectTr.transform);
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
                if (chipFrameTable[i, j].filledChip != null) continue;
                List<ChipFrameData> fittableFrames = new List<ChipFrameData>();
                fittableFrames.Add(chipFrameTable[i, j]);
                foreach (var frame in fittableFrames)
                {
                    frame.filledChip = chip;
                    frame.filledChip.count.text = chipCount.ToString();
                    frame.chipCount = chipCount;
                }
                chip.rectTr.sizeDelta = new Vector2(chipSize, chipSize);
                chip.rectTr.anchoredPosition = new Vector3(chipSize * j, -chipSize * i, 0);
                return;

                // var fittableFrames = GetFittableFrameList(chip, j, i);
                // if (fittableFrames != null)
                // {
                //     foreach (var frame in fittableFrames)
                //     {
                //         frame.filledChip = chip;
                //     }
                //     chip.rectTr.sizeDelta = new Vector2(chip.rowNum, chip.colNum) * chipSize;
                //     chip.rectTr.anchoredPosition = new Vector3(chipSize * j, -chipSize * i, 0);
                //     return;
                // }
            }
        }
        Destroy(chip.gameObject);
    }

    private List<ChipFrameData> GetFittableFrameList(PuzzleFrameData[,] targetTable, Chip chip, int x, int y)
    {
        List<ChipFrameData> cfdList = new List<ChipFrameData>();
        var table = chip.row;
        for (int i = 0; i < table.Length; i++)
        {
            for (int j = 0; j < table[i].col.Length; j++)
            {
                if (!table[i].col[j])
                {
                    continue;
                }

                if (targetTable[y + j, x + i].patternNum == 0) return null;

                if (x + i >= chipCountX || y + j >= chipCountY) return null;

                if (targetTable[y + j, x + i].filledChip != null) return null;

                cfdList.Add(targetTable[y + j, x + i]);
            }
        }
        return cfdList;
    }

    private ChipFrameData GetChipAtPos(Vector2 pos)
    {
        pos /= chipSize;
        return chipFrameTable[(int)-pos.y, (int)pos.x];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 clickedPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(chipPanelRectTr, eventData.pressPosition, eventData.pressEventCamera, out clickedPos))
        {
            return;
        }
        Debug.Log("start: " + clickedPos);
        var chipPos = clickedPos + new Vector2(chipPanelRectTr.sizeDelta.x, chipPanelRectTr.sizeDelta.y / -2);
        var chipFrameData = GetChipAtPos(chipPos);
        var chip = chipFrameData.filledChip;
        if (chip != null && chipFrameData.chipCount > 0)
        {
            dragImg.texture = chip.image.texture;
            dragImgRectTr.sizeDelta = new Vector2(chip.rowNum, chip.colNum) * chipSize;
            dragImgRectTr.localPosition = clickedPos + new Vector2(-dragImgRectTr.sizeDelta.x, dragImgRectTr.sizeDelta.y) / 2;
            currentSelectedChipData = chipFrameData;
            dragImg.gameObject.SetActive(true);
            chipFrameData.chipCount--;
            if (chipFrameData.chipCount <= 0)
            {
                chip.gameObject.SetActive(false);
            }
            else
            {
                chip.count.text = chipFrameData.chipCount.ToString();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSelectedChipData != null)
        {
            dragImgRectTr.localPosition += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSelectedChipData != null)
        {
            dragImg.gameObject.SetActive(false);
            bool success = false;

            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            ray.Raycast(ped, results);
            if (results.Count > 0)
            {
                var frame = results[0].gameObject.GetComponent<PuzzleFrame>();
                if (frame != null && frame.pfd.patternNum != 0)
                {
                    var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChipData.filledChip, frame.pfd.x, frame.pfd.y);
                    if (fittableFrames != null)
                    {
                        var chipInstance = Instantiate(currentSelectedChipData.filledChip, chipPanelRectTr.transform);
                        chipInstance.gameObject.SetActive(true);
                        foreach (var ff in fittableFrames)
                        {
                            ff.filledChip = chipInstance;
                        }
                        // TODO: 크기와 위치 변경
                        chipInstance.countPlate.SetActive(false);
                        chipInstance.transform.parent = puzzleGrid.transform;
                        chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                        chipInstance.rectTr.anchoredPosition = new Vector3(chipSize * frame.pfd.x, -chipSize * frame.pfd.y, 0);
                        Debug.Log("Good");
                        success = true;
                    }
                }
            }

            // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(puzzleGridRectTr, eventData.position, eventData.pressEventCamera, out clickedPos))
            // {
            //     Debug.Log("end: " + clickedPos);
            //     puzzleGrid.cellSize.x;

            //     if (0 < clickedPos.x && clickedPos.x < puzzleGridRectTr.rect.width && clickedPos.y < Mathf.Abs(puzzleGridRectTr.rect.height))
            //     {
            //         int x = 0, y = 0;
            //         var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChipData.filledChip, x, y);
            //         if (fittableFrames != null)
            //         {
            //             var chipInstance = Instantiate(currentSelectedChipData.filledChip, chipPanelRectTr.transform);
            //             foreach (var frame in fittableFrames)
            //             {
            //                 frame.filledChip = chipInstance;
            //             }
            //             chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
            //             chipInstance.rectTr.anchoredPosition = new Vector3(chipSize * x, -chipSize * y, 0);
            //             Debug.Log("Good");
            //             success = true;
            //         }
            //         Debug.Log(clickedPos);
            //     }
            // }

            if (!success)
            {
                currentSelectedChipData.chipCount++;
                if (currentSelectedChipData.chipCount > 0)
                {
                    currentSelectedChipData.filledChip.gameObject.SetActive(true);
                }
                currentSelectedChipData.filledChip.count.text = currentSelectedChipData.chipCount.ToString();
            }
            currentSelectedChipData = null;
        }
    }
}
