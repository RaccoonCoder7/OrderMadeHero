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
        SceneManager.LoadScene("InitScene");
    }

    private void ChangeToBookScene()
    {
        SceneManager.LoadScene("InitScene");
    }

    private void OpenAlertPanel()
    {
        alertPanel.SetActive(true);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator ChangeScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene2");
        asyncLoad.allowSceneActivation = false;
        yield return null;
        SceneManager.LoadScene("InitScene");
    }
}
