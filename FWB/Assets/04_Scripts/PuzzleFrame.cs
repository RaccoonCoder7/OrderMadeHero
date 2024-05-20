using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퍼즐 정보와 연결된 컴포넌트를 저장하는 매니저 컴포넌트
/// </summary>
public class PuzzleFrame : MonoBehaviour
{
    public RawImage image;
    public PuzzleMgr.PuzzleFrameData pfd;

    public void SetPuzzleFrameData(PuzzleMgr.PuzzleFrameData pfd)
    {
        this.pfd = pfd;
    }
    public void SetHighlight(bool isActive, Color color)
    {
        image.color = isActive ? color : Color.white;
    }

}
