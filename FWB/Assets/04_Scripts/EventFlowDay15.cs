using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventFlowDay15 : EventFlow
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
        hint.onClick.RemoveAllListeners();
        hint.gameObject.SetActive(true);
        hint.onClick.AddListener(() => { OnClickHintBtn(hint); });
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
        GameMgr.In.newsProgress = 2;
        StartCoroutine(CommonTool.In.FadeOut());
        mgr.newsPanel.SetActive(false);
        StartCoroutine(CommonTool.In.FadeIn());
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
        switch (progress)
        {
            case 0:
                StartCoroutine(HintBtnAnim(hintBtn));
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day15_2", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "버니 브레이브, 그의 치명적 약점 귀";
                break;
            case 1:
                StartCoroutine(HintBtnAnim(hintBtn));
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day15_3", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[2].GetComponentInChildren<Text>().text = "버니브레이브, 그의 힘이 담긴 목소리";
                break;
            case 2:
                StartCoroutine(HintBtnAnim(hintBtn));
                hintBtn.onClick.RemoveAllListeners();
                foreach (var btn in mgr.newsHintButtons)
                {
                    btn.gameObject.SetActive(false);
                }
                mgr.StartText("Day15_4", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "마피아의 수장 퍼펫, 괴로워하는 그의 모습";
                break;
            case 3:
                StartCoroutine(HintBtnAnim(hintBtn));
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day15_5", NewsHintProg, NewsHintProg);
                mgr.newsHintButtons[1].GetComponentInChildren<Text>().text = "마피아의 수장 퍼펫, 그가 소중히 하는 보석";
                break;
            case 4:
                StartCoroutine(HintBtnAnim(hintBtn));
                hintBtn.onClick.RemoveAllListeners();
                mgr.StartText("Day15_6", EndNewsFlow, EndNewsFlow);
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
    
    private IEnumerator DelayFlow()
    {
        mgr.inNews = true;
        mgr.newsPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        mgr.StartText("Day15_1", NewsHintProg, NewsHintProg);
        mgr.newsHintButtons[0].GetComponentInChildren<Text>().text = "다수를 위한 소수의 희생의 구제 방안";
    }
}
