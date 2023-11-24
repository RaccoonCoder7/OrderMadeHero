using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시작 씬의 동작을 관리
/// </summary>
public class StartSceneMgr : MonoBehaviour
{
    public Button startBtn;
    public Button continueBtn;
    public Button bookBtn;
    public Button settingBtn;
    public Button quitBtn;


    void Start()
    {
        startBtn.onClick.AddListener(ChangeToInitScene);
        continueBtn.onClick.AddListener(OpenAlertPanel);
        bookBtn.onClick.AddListener(OpenAlertPanel);
        settingBtn.onClick.AddListener(OpenAlertPanel);
        quitBtn.onClick.AddListener(Quit);
        GameMgr.In.initDone = true;

        StartCoroutine(CommonTool.In.FadeIn());
    }

    private void ChangeToInitScene()
    {
        StartCoroutine(CommonTool.In.AsyncChangeScene("IntroScene"));
    }

    private void OpenAlertPanel()
    {
        CommonTool.In.OpenAlertPanel("현재 사용할 수 없는 기능입니다.");
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
