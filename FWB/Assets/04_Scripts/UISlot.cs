using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 청사진 슬롯의 정보와 연결된 컴포넌트를 저장하는 매니저 컴포넌트
/// </summary>
public class UISlot : MonoBehaviour
{
    public string key;
    public Button button;
    public Image image;
    public Image innerImage;
    public SpriteChange spriteChange;
}
