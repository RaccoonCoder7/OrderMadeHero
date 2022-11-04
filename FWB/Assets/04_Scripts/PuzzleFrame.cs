using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleFrame : MonoBehaviour
{
    public int patternNum = 0;
    public RawImage image;


    public void SetPuzzleFrameData(GameSceneMgr.PuzzleFrameData ppd)
    {
        patternNum = ppd.patternNum;
    }
}
