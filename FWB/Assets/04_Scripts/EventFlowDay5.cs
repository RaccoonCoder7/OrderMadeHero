using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// day2의 이벤트를 제어
/// </summary>
public class EventFlowDay5 : EventFlow
{
    private GameObject puffet;

    public override void StartFlow()
    {
        if (DataSaveLoad.dataSave.isLoaded == true)
        {
            StartCoroutine(mgr.StartNormalRoutine(GameMgr.In.dayCustomerCnt, mgr.EndNormalOrderRoutine));
            DataSaveLoad.dataSave.isLoaded = false;
        }
        else
        {
            mgr.StartText("Day5_1", EndDay5_1Routine, EndDay5_1Routine);
        }
    }

    private void EndDay5_1Routine()
    {
        mgr.EndText();
        puffet = mgr.imageList.Find(x => x.key.Equals("화난퍼펫")).imageObj;
        var navi = mgr.imageList.Find(x => x.key.Equals("당황한나비")).imageObj;
        puffet.SetActive(true);
        navi.transform.DOLocalMoveX(300, 1);
        puffet.transform.DOLocalMoveX(-550, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            mgr.StartText("Day5_2", EndDay5_2Routine);
        });
    }

    private void EndDay5_2Routine()
    {
        mgr.EndText();

        var navi = mgr.imageList.Find(x => x.key.Equals("당황한나비")).imageObj;
        var a = GameObject.Find("movingImg");
        a.GetComponent<Image>().enabled = true;
        
        navi.transform.DOLocalMoveX(100, 1).SetEase(Ease.OutCubic).OnComplete(() =>
        {
                navi.SetActive(false);
        });
        a.transform.DOLocalMoveX(0, 1).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            puffet.GetComponent<Mask>().enabled = false;
            a.SetActive(false);
        });

        var puffetRectTr = puffet.GetComponent<RectTransform>();
        var size = puffetRectTr.sizeDelta;
        DOTween.To(() => size.x, x =>
        {
            size.x = x;
            puffetRectTr.sizeDelta = size;
        }, 1920, 1);
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
        mgr.StartText("Day5_5", EndDay5_5Routine);
        mgr.ObjectBlinker(mgr.tendency, 10, 2);
    }

    private void EndDay5_5Routine()
    {
        mgr.EndText();
        mgr.mainChatPanel.SetActive(false);
        mgr.pcChatPanel.SetActive(false);
        StartCoroutine(mgr.StartNormalRoutine(8, mgr.EndNormalOrderRoutine));
    }
}
