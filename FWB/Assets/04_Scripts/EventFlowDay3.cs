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

        mgr.shop.onClick.RemoveAllListeners();
        mgr.shop.onClick.AddListener(() =>
        {
            mgr.bgImg.sprite = mgr.bgImgList[1];
            mgr.chipSet.gameObject.SetActive(true);
            mgr.buy.gameObject.SetActive(true);
            mgr.exitStore.gameObject.SetActive(true);
            mgr.StartText("Day3_2", EndDay3_2Routine, EndDay3_2Routine);
            mgr.shop.onClick.RemoveAllListeners();
        });
    }

    private void EndDay3_2Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.chatTarget = GameSceneMgr.ChatTarget.None;
        //TODO: CommonTool.In.SetFocus();

        mgr.chipSet.onClick.RemoveAllListeners();
        mgr.chipSet.onClick.AddListener(() =>
        {
            CommonTool.In.SetFocusOff();
            mgr.StartText("Day3_3", EndDay3_3Routine, EndDay3_3Routine);
            mgr.chipSet.onClick.RemoveAllListeners();
        });
    }

    private void EndDay3_3Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.chatTarget = GameSceneMgr.ChatTarget.None;

        mgr.buy.onClick.RemoveAllListeners();
        mgr.buy.onClick.AddListener(() =>
        {
            mgr.saleStatus.text = "Sold Out";
            mgr.StartText("Day3_4", EndDay3_4Routine, EndDay3_4Routine);
            mgr.buy.onClick.RemoveAllListeners();
        });
    }

    private void EndDay3_4Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);

        mgr.exitStore.onClick.RemoveAllListeners();
        mgr.exitStore.onClick.AddListener(() =>
        {
            mgr.chipSet.gameObject.SetActive(false);
            mgr.buy.gameObject.SetActive(false);
            mgr.exitStore.gameObject.SetActive(false);
            mgr.bgImg.sprite = mgr.bgImgList[0];
            mgr.imageList.Find(x => x.key.Equals("매드")).imageObj.SetActive(false);
            StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
            mgr.exitStore.onClick.RemoveAllListeners();
        });
    }
}
