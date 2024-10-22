using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlueprintImgChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image bluePrintImg;
    public List<Image> backgroundImageList = new List<Image>();
    public List<Sprite> spriteList = new List<Sprite>();
    public bool changeImage = true;

    private bool[] puzzleFlag = new bool[54];

    public void SetBlueprintImage(WeaponDataTable.BluePrint weapon)
    {
        if (!changeImage)
        {
            return;
        }

        var puzzle = CommonTool.In.GetPuzzle(weapon);
        int i = 0;
        foreach (var frameData in puzzle.frameDataTable)
        {
            puzzleFlag[i] = frameData.patternNum == 0 ? false : true;
            i++;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!changeImage)
        {
            return;
        }

        bluePrintImg.gameObject.SetActive(false);
        for (int i = 0; i < backgroundImageList.Count; i++)
        {
            var sprite = puzzleFlag[i] ? spriteList[1] : spriteList[0];
            backgroundImageList[i].sprite = sprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!changeImage)
        {
            return;
        }

        bluePrintImg.gameObject.SetActive(true);
        for (int i = 0; i < backgroundImageList.Count; i++)
        {
            backgroundImageList[i].sprite = spriteList[0];
        }
    }
}
