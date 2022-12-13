using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chip : MonoBehaviour
{
    public int chipKey;
    public RawImage image;
    public RectTransform rectTr;
    public GameObject countPlate;
    public Text count;
    public int chipCount;
    public int colNum;
    public int rowNum;
    public Col[] row;

    [System.Serializable]
    public class Col
    {
        public bool[] col;
    }

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
}
