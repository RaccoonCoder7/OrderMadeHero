using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 사용자의 클릭을 입력받아 채팅을 진행시킴
/// </summary>
public class ChatBoxClickListener : MonoBehaviour, IPointerClickHandler
{
    public IntroSceneMgr manager;
    public GameSceneMgr manager2;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager2.isShopAnimating) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (manager != null) manager.OnClickChatBox();
            if (manager2 != null) manager2.OnClickChatBox();
        }
    }
}
