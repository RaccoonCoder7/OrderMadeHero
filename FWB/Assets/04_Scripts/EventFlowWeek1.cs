using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventFlowWeek1 : EventFlow
{
    private int progress = 0;
    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            mgr.day.SetActive(false);
            mgr.gold.SetActive(false);
            mgr.renom.SetActive(false);
            mgr.tendency.SetActive(false);
            CommonTool.In.fadeImage.gameObject.SetActive(true);
            CommonTool.In.fadeImage.color = new UnityEngine.Color(0, 0, 0, 1);
            StartCoroutine(CommonTool.In.FadeIn());
            StartCoroutine(DelayFlow());
        }
        else
        {
            StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        }
    }
    
    private void NewsHintProg()
    {
        int num = progress;
        if (progress > 2)
        {
            num -= 3;
        }
        var hint = mgr.newsHintButtons[num];
        hint.onClick.RemoveListener(OnClickHintBtn);
        hint.gameObject.SetActive(true);
        hint.onClick.AddListener(OnClickHintBtn);
    }

    private void EndNewsFlow()
    {
        mgr.EndText();
        mgr.day.SetActive(true);
        mgr.gold.SetActive(true);
        mgr.renom.SetActive(true);
        mgr.tendency.SetActive(true);
        mgr.inNews = false;
        mgr.historyText.text = string.Empty;
        GameMgr.In.newsProgress = 1;
        StartCoroutine(CommonTool.In.FadeOut());
        mgr.newsPanel.SetActive(false);
        StartCoroutine(CommonTool.In.FadeIn());
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        foreach (var btn in mgr.newsHintButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    
    private void OnClickHintBtn()
    {
        switch (progress)
        {
            case 0:
                mgr.StartText("Week1_2", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "화재 사건의 피의자…죄인인가, 영웅인가?";
                break;
            case 1:
                mgr.StartText("Week1_3", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[2].GetComponentInChildren<Text>().text = "빌딩 하나와 맞바꾼 '10만 크레딧'";
                break;
            case 2:
                foreach (var btn in mgr.newsHintButtons)
                {
                    btn.gameObject.SetActive(false);
                }
                mgr.StartText("Week1_4", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "마피아의 습격…'단단한 장비' 구비는 필수";
                break;
            case 3:
                mgr.StartText("Week1_5", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "인기 히어로 버니 브레이브…'끈적한 게 싫어'";
                break;
            case 4:
                mgr.StartText("Week1_6", EndNewsFlow, EndNewsFlow);
                break;
        }
        progress++;
    }
    
    private IEnumerator DelayFlow()
    {
        mgr.inNews = true;
        mgr.newsPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        mgr.StartText("Week1_1", NewsHintProg, NewsHintProg);
        mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "야밤의 빌딩 전소…사상자 단 '0'명";
    }
}