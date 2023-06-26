using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatBoxClickListener : MonoBehaviour, IPointerClickHandler
{
    public IntroSceneMgr manager;
    public GameSceneMgr2 manager2;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager != null) manager.OnClickChatBox();
        if (manager2 != null) manager2.OnClickChatBox();
    }
}
