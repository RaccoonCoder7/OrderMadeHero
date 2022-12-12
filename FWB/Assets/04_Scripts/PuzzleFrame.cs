using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleFrame : MonoBehaviour
{
    public RawImage image;
    public GameSceneMgr.PuzzleFrameData pfd;
    
    public void SetPuzzleFrameData(GameSceneMgr.PuzzleFrameData pfd)
    {
        this.pfd = pfd;
    }
}
