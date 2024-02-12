using DG.Tweening;
using UnityEngine;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay5 : EventFlow
{
    private GameObject puffet;

    public override void StartFlow()
    {
        mgr.StartText("Day5_1", EndDay5_1Routine, EndDay5_1Routine);
    }

    private void EndDay5_1Routine()
    {
        mgr.EndText();
        puffet = mgr.imageList.Find(x => x.key.Equals("화난퍼펫")).imageObj;
        puffet.SetActive(true);
        puffet.transform.DOLocalMoveX(-430, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            mgr.StartText("Day5_2", EndDay5_2Routine);
        });
    }

    private void EndDay5_2Routine()
    {
        mgr.EndText();

        var navi = mgr.imageList.Find(x => x.key.Equals("당황한나비")).imageObj;
        navi.transform.DOLocalMoveX(100, 1).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            navi.SetActive(false);
        });

        var puffetRectTr = puffet.GetComponent<RectTransform>();
        var size = puffetRectTr.sizeDelta;
        DOTween.To(() => size.x, x =>
        {
            size.x = x;
            puffetRectTr.sizeDelta = size;
        }, 1300, 1);
        puffet.transform.DOLocalMoveX(0, 1).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            mgr.StartText("Day5_3", EndDay5_3Routine);
        });
    }

    private void EndDay5_3Routine()
    {
        mgr.EndText();
        CommonTool.In.PlayOneShot("crash");
        puffet.transform.DOShakePosition(1, new Vector3(38, 25, 0), 50).OnComplete(() =>
        {
            mgr.StartText("Day5_4", EndDay5_4Routine);
        });
    }

    private void EndDay5_4Routine()
    {
        mgr.EndText();
        mgr.pcChatPanel.SetActive(false);

        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
