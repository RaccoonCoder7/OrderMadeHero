using System.Collections;
using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay3 : EventFlow
{
    public override void StartFlow()
    {
        mgr.StartText("Day3_1", EndDay3_1Routine, EndDay3_1Routine);
    }

    private void EndDay3_1Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        mgr.SetShopButtonListener();
        mgr.shop.onClick.AddListener(OnClickShop);
    }

    private void OnClickShop()
    {
        mgr.shopControlBlockingPanel.SetActive(true);
        StartCoroutine(DelayFlow());
    }

    private void EndDay3_2Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(false);
        mgr.chatTarget = GameSceneMgr.ChatTarget.None;
        mgr.shopControlBlockingPanel.SetActive(false);
        CommonTool.In.SetFocus(new Vector2(875, 820), new Vector2(240, 70));
        mgr.shopChipsetTab.onClick.AddListener(OnClickShopChipsetTab);
    }

    private void OnClickShopChipsetTab()
    {
        CommonTool.In.SetFocusOff();
        mgr.StartText("Day3_3", EndDay3_3Routine, EndDay3_3Routine);
        mgr.shopChipsetTab.onClick.RemoveListener(OnClickShopChipsetTab);
    }

    private void EndDay3_3Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(false);
        // TODO: Focus
        mgr.chatTarget = GameSceneMgr.ChatTarget.None;
        mgr.shopPopupUI.no.interactable = false;
        mgr.shopPopupUI.yes.onClick.AddListener(OnClickPopupYes);
    }

    private void OnClickPopupYes()
    {
        mgr.shopPopupUI.no.interactable = true;
        mgr.StartText("Day3_4", EndDay3_4Routine, EndDay3_4Routine);
        mgr.shopPopupUI.yes.onClick.RemoveListener(OnClickPopupYes);
    }

    private void EndDay3_4Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(false);

        mgr.shopDodge.onClick.AddListener(OnClickShopDodge);
    }

    private void OnClickShopDodge()
    {
        mgr.shopDodge.onClick.AddListener(mgr.OnClickShopDodge);
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        mgr.shopDodge.onClick.RemoveListener(OnClickShopDodge);
    }

    private IEnumerator DelayFlow()
    {
        mgr.shop.onClick.RemoveListener(OnClickShop);
        yield return new WaitForSeconds(1.5f);
        mgr.StartText("Day3_2", EndDay3_2Routine, EndDay3_2Routine);
    }
}
