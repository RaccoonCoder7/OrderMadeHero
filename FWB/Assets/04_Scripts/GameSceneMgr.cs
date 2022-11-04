using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneMgr : MonoBehaviour
{
    public List<TextAsset> csvList;
    public GridLayoutGroup puzzleGrid;
    public RectTransform chipPanelRectTr;
    public int chipCountX;
    public int chipCountY;
    public float chipSize;
    public PuzzleFrame puzzleFrame;
    public List<Texture> textureList = new List<Texture>();
    public List<Chip> chipList = new List<Chip>();
    public Dictionary<int, int> chipInventory = new Dictionary<int, int>();

    private RectTransform puzzleGridRectTr;
    private RawImage background;
    private Puzzle currPuzzle;
    private ChipFrameData[,] chipFrameTable;

    public class Puzzle
    {
        public int x;
        public int y;
        public List<PuzzleFrameData> frameDataList = new List<PuzzleFrameData>();
    }

    public class PuzzleFrameData
    {
        public int patternNum = 0;

        public PuzzleFrameData(int patternNum)
        {
            this.patternNum = patternNum;
        }
    }

    public class ChipFrameData
    {
        public Chip filledChip;
    }

    void Start()
    {
        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        currPuzzle = GetPuzzle(0);
        var width = puzzleGridRectTr.rect.width / currPuzzle.x;
        var height = puzzleGridRectTr.rect.height / currPuzzle.y;
        var size = width < height ? width : height;
        puzzleGrid.cellSize = new Vector2(size, size);
        puzzleGrid.constraintCount = currPuzzle.x;
        InstantiateFrames(currPuzzle.frameDataList);
        background.texture = textureList[0];

        SetTestDataToChipInventory();
        RefreshChipPanel();
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
                puzzle.frameDataList.Add(new PuzzleFrameData(targetNum));
            }
        }

        return puzzle;
    }

    private void InstantiateFrames(List<PuzzleFrameData> frameDataList)
    {
        foreach (var frameData in frameDataList)
        {
            var frame = Instantiate(puzzleFrame, puzzleGrid.transform);
            frame.SetPuzzleFrameData(frameData);
            if (frame.patternNum == 0)
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
        chipInventory.Add(3, 1);
        chipInventory.Add(4, 1);
        chipInventory.Add(5, 1);
        chipInventory.Add(6, 2);
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
            var chip = GetChip(key);
            if (chip)
            {
                for (int i = 0; i < chipInventory[key]; i++)
                {
                    var chipInstance = Instantiate(chip, chipPanelRectTr.transform);
                    SetChipToPanel(chipInstance);
                }
            }
        }
    }

    private Chip GetChip(int key)
    {
        return chipList.Find(x => x.chipKey.Equals(key));
    }

    private void SetChipToPanel(Chip chip)
    {
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                var fittableFrames = GetFittableFrameList(chip, j, i);
                if (fittableFrames != null)
                {
                    foreach (var frame in fittableFrames)
                    {
                        frame.filledChip = chip;
                    }
                    chip.rectTr.sizeDelta = new Vector2(chip.colNum, chip.rowNum) * chipSize;
                    chip.rectTr.anchoredPosition = new Vector3(chipSize * j, -chipSize * i, 0);
                    return;
                }
            }
        }
    }

    private List<ChipFrameData> GetFittableFrameList(Chip chip, int x, int y)
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

                if (x + i >= chipCountX || y + j >= chipCountY) return null;

                if (chipFrameTable[y + j, x + i].filledChip != null) return null;

                cfdList.Add(chipFrameTable[y + j, x + i]);
            }
        }
        return cfdList;
    }
}
