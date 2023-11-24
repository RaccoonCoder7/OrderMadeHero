using UnityEngine;

/// <summary>
/// 여러가지 개발시에 테스트용으로 사용되는 기능들을 모아 둔 스크립트
/// </summary>
public class Test : MonoBehaviour
{
    public float width;
    public float height;
    public float posX;
    public float posY;


    /// <summary>
    /// 화면 포커스 기능 테스트
    /// </summary>
    [ContextMenu("SetFocus")]
    public void SetFocus()
    {
        CommonTool.In.SetFocus(new Vector2(posX, posY), new Vector2(width, height));
    }
}
