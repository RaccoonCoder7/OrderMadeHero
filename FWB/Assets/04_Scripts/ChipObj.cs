using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ChipTable;

/// <summary>
/// 칩의 정보와 연결된 컴포넌트를 저장하는 매니저 컴포넌트
/// </summary>
public class ChipObj : MonoBehaviour
{
    public int chipKey;
    public int chipCount;
    public int colNum;
    public int rowNum;
    public RawImage image;
    public RectTransform rectTr;
    public GameObject countPlate;
    public Text count;
    public List<ChipAbility> chipAbilityList = new List<ChipAbility>();
    public Col[] row;
    [HideInInspector]
    public Col[] originRow;


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
        originRow = (Col[])row.Clone();
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

    public void ResetCol()
    {
        if (originRow != null)
        {
            row = (Col[])originRow.Clone();
            rowNum = originRow.Length;
            colNum = originRow[0].col.Length;
        }
    }
}
