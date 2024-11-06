using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventFlowDay8 : EventFlow
{
    private int progress = 0;
    private Coroutine blinkAnim;

    public override void StartFlow()
    {
        if (GameMgr.In.isEventOn == 1)
        {
            mgr.deskTr.gameObject.SetActive(false);
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
        hint.onClick.RemoveAllListeners();
        hint.gameObject.SetActive(true);
        hint.onClick.AddListener(() => { OnClickHintBtn(hint); });
        blinkAnim = StartCoroutine(BlinkHintButton(hint, 1));
    }

    private void EndNewsFlow()
    {
        mgr.EndText();
        mgr.deskTr.gameObject.SetActive(true);
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
        Debug.Log("Start News Order");
        StartCoroutine(mgr.StartNormalRoutine(6, mgr.EndNormalOrderRoutine));
        foreach (var btn in mgr.newsHintButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
        foreach (var btn in mgr.newsHintButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }
    
    private void OnClickHintBtn(Button hintBtn)
    {
        Image hintImage = hintBtn.GetComponent<Image>();
        Text hintText = hintBtn.GetComponentInChildren<Text>();
        switch (progress)
        {
            case 0:
                StartCoroutine(HintBtnAnim(hintBtn));
                StopCoroutine(blinkAnim);
                hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
                hintText.color = hintImage.color;
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day8_2", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "화재 사건의 피의자…죄인인가, 영웅인가?";
                break;
            case 1:
                StartCoroutine(HintBtnAnim(hintBtn));
                StopCoroutine(blinkAnim);
                hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
                hintText.color = hintImage.color;
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day8_3", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[2].GetComponentInChildren<Text>().text = "빌딩 하나와 맞바꾼 '10만 크레딧'";
                break;
            case 2:
                StartCoroutine(HintBtnDisableAnim(hintBtn));
                StopCoroutine(blinkAnim);
                hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
                hintText.color = hintImage.color;
                hintBtn.onClick.RemoveAllListeners();
                break;
            case 3:
                StartCoroutine(HintBtnAnim(hintBtn));
                StopCoroutine(blinkAnim);
                hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
                hintText.color = hintImage.color;
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day8_5", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "인기 히어로 버니 브레이브…'끈적한 게 싫어'";
                break;
            case 4:
                StartCoroutine(HintBtnAnim(hintBtn));
                StopCoroutine(blinkAnim);
                hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
                hintText.color = hintImage.color;
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day8_6", EndNewsFlow, EndNewsFlow);
                break;
        }
        progress++;
    }
    
    private IEnumerator HintBtnAnim(Button hintBtn)
    {
        var anim = hintBtn.GetComponent<Animator>();
        anim.SetBool("isClicked", true);
        yield return new WaitForSeconds(1.0f);
        anim.SetBool("isClicked", false);
    }
    
    private IEnumerator HintBtnDisableAnim(Button hintBtn)
    {
        yield return StartCoroutine(HintBtnAnim(hintBtn));
    
        foreach (var btn in mgr.newsHintButtons)
        {
            btn.gameObject.SetActive(false);
        }

        yield return new WaitForEndOfFrame(); 

        mgr.StartText("Day8_4", NewsHintProg, NewsHintProg);
        mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "마피아의 습격…'단단한 장비' 구비는 필수";
    }
    
    private IEnumerator BlinkHintButton(Button hintBtn, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Image hintImage = hintBtn.GetComponent<Image>();
        Text hintText = hintBtn.GetComponentInChildren<Text>();
        if (hintImage == null) yield break;

        while (true)
        {
            hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 0);
            hintText.color = hintImage.color;
            yield return new WaitForSeconds(0.5f);

            hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, 1);
            hintText.color = hintImage.color;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator DelayFlow()
    {
        mgr.inNews = true;
        mgr.newsPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        mgr.StartText("Day8_1", NewsHintProg, NewsHintProg);
        mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "야밤의 빌딩 전소…사상자 단 '0'명";
    }
}
