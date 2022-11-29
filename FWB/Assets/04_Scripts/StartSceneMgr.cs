using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneMgr : MonoBehaviour
{
    public Button startBtn;
    public Button continueBtn;
    public Button bookBtn;
    public Button settingBtn;
    public Button quitBtn;
    public Button alertDodgeBtn;
    public GameObject alertPanel;


    void Start()
    {
        alertPanel.SetActive(false);
        startBtn.onClick.AddListener(ChangeToInitScene);
        continueBtn.onClick.AddListener(OpenAlertPanel);
        bookBtn.onClick.AddListener(OpenAlertPanel);
        settingBtn.onClick.AddListener(OpenAlertPanel);
        quitBtn.onClick.AddListener(Quit);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));
    }

    private void ChangeToInitScene()
    {
        Debug.Log(1);
        StartCoroutine(CommonTool.In.AsyncChangeScene("InitScene"));
        Debug.Log(2);
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
