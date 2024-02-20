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
    [HideInInspector]
    public Col[] originRow;
    [HideInInspector]
    public Col[] currentRow;
    [HideInInspector]
    public List<ChipAbility> chipAbilityList = new List<ChipAbility>();
    [Header("UI")]
    public RawImage image;
    public RectTransform rectTr;
    public SpriteChange backgroundSC;

    private int size;


    [System.Serializable]
    public class Col
    {
        public bool[] col;
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
            row[i].col = new bool[colNum];
        }
    }

    public void SaveOriginRow()
    {
        if (originRow == null || originRow.Length == 0)
        {
            originRow = (Col[])row.Clone();
        }
    }

    public void SaveCurrentRow()
    {
        currentRow = (Col[])row.Clone();
    }

    public void RotateRight()
    {
        var newRow = new Col[colNum];
        for (int i = 0; i < colNum; i++)
        {
            newRow[i] = new Col();
            newRow[i].col = new bool[rowNum];
        }

        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                newRow[x].col[y] = row[rowNum - 1 - y].col[x];
            }
        }

        int tempRownum = rowNum;
        rowNum = colNum;
        colNum = tempRownum;
        row = newRow;
    }

    public void ResetColToOriginData()
    {
        if (originRow != null && originRow.Length > 0)
        {
            row = (Col[])originRow.Clone();
            rowNum = originRow.Length;
            colNum = originRow[0].col.Length;
        }
    }

    public void ResetColToCurrentData()
    {
        if (currentRow != null)
        {
            row = (Col[])currentRow.Clone();
            rowNum = currentRow.Length;
            colNum = currentRow[0].col.Length;
        }
    }

    public int GetChipSize()
    {
        if (size > 0) return size;

        for (int i = 0; i < rowNum; i++)
        {
            for (int j = 0; j < colNum; j++)
            {
                if (row[i].col[j]) size++;
            }
        }
        return size;
    }
}
