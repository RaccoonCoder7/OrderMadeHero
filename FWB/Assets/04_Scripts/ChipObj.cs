using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChipTable;

/// <summary>
/// 칩의 정보와 연결된 컴포넌트를 저장하는 매니저 컴포넌트
/// </summary>
public class ChipObj : MonoBehaviour
{
    [Header("Data")]
    public string chipKey;
    public int colNum;
    public int rowNum;
    public Col[] row;
    public int[] posOffset;
    public bool enableOnSpecificBlueprint;
    [HideInInspector]
    public Col[] originRow;
    [HideInInspector]
    public Col[] currentRow;
    [HideInInspector]
    public int[] originPosOffset;
    [HideInInspector]
    public int[] currentPosOffset;
    [HideInInspector]
    public int originRowNum;
    [HideInInspector]
    public int originColNum;
    [HideInInspector]
    public int currentRowNum;
    [HideInInspector]
    public int currentColNum;
    [HideInInspector]
    public List<ChipAbility> chipAbilityList = new List<ChipAbility>();
    [Header("UI")]
    public RawImage image;
    public Image parentImage;
    public RectTransform rectTr;

    private int size;


    [System.Serializable]
    public class Col
    {
        public int[] col;
    }

    /// <summary>
    /// 칩의 정보를 세팅하기 위한 ContextMenu
    /// </summary>
    [ContextMenu("Set Default Table Data")]
    public void SetDefaultTableData()
    {
        row = new Col[rowNum];
        for (int i = 0; i < rowNum; i++)
        {
            row[i] = new Col();
            row[i].col = new int[colNum];
        }
    }

    public void SaveOriginRow()
    {
        if (originRow == null || originRow.Length == 0)
        {
            originRow = (Col[])row.Clone();
            originPosOffset = (int[])posOffset.Clone();
            originRowNum = rowNum;
            originColNum = colNum;
        }
    }

    public void SaveCurrentRow()
    {
        currentRow = (Col[])row.Clone();
        currentPosOffset = (int[])posOffset.Clone();
        currentRowNum = rowNum;
        currentColNum = colNum;
    }

    public void RotateRight()
    {
        var newRow = new Col[row[0].col.Length];
        for (int i = 0; i < row[0].col.Length; i++)
        {
            newRow[i] = new Col();
            newRow[i].col = new int[row.Length];
        }

        for (int x = 0; x < row[0].col.Length; x++)
        {
            for (int y = 0; y < row.Length; y++)
            {
                newRow[x].col[y] = row[y].col[row[0].col.Length - 1 - x];
            }
        }

        int tempRownum = rowNum;
        rowNum = colNum;
        colNum = tempRownum;
        row = newRow;

        int temp = posOffset[3];
        posOffset[3] = posOffset[2];
        posOffset[2] = posOffset[1];
        posOffset[1] = posOffset[0];
        posOffset[0] = temp;
    }

    public void ResetColToOriginData()
    {
        if (originRow != null && originRow.Length > 0)
        {
            rowNum = originRowNum;
            colNum = originColNum;
            row = (Col[])originRow.Clone();
            posOffset = (int[])originPosOffset.Clone();
        }
    }

    public void ResetColToCurrentData()
    {
        if (currentRow != null)
        {
            rowNum = currentRowNum;
            colNum = currentColNum;
            row = (Col[])currentRow.Clone();
            posOffset = (int[])currentPosOffset.Clone();
        }
    }

    public int GetChipSize()
    {
        if (size > 0) return size;

        for (int i = 0; i < rowNum; i++)
        {
            for (int j = 0; j < colNum; j++)
            {
                if (row[i].col[j] == 1) size++;
            }
        }
        return size;
    }
}
