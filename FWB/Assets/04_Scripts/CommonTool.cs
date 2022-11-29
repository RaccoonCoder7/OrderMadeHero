using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommonTool : SingletonMono<CommonTool>
{
    public GameObject alertPanel;
    public GameObject confirmPanel;
    public Text alertText;
    public Text confirmText;
    public Button alertDodgeBtn;
    public Button confirmBtn;
    public Button cancelBtn;
    public Image fadeImage;
    public float fadeSpeed;

    private Canvas canvas;


    void Start()
    {
        canvas = GetComponent<Canvas>();

        fadeImage.gameObject.SetActive(false);
        alertPanel.SetActive(false);
        confirmPanel.SetActive(false);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));
    }

    public void OpenConfirmPanel(string text, Action OnConfirm, Action OnCancel)
    {
        confirmPanel.SetActive(true);
        confirmText.text = text;
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(() =>
            {
                confirmPanel.SetActive(false);
                OnConfirm.Invoke();
            });
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(() =>
            {
                confirmPanel.SetActive(false);
                OnCancel.Invoke();
            });
    }

    public void OpenAlertPanel(string alertText)
    {
        alertPanel.SetActive(true);
        this.alertText.text = alertText;
    }

    public IEnumerator FadeIn()
    {
        float fadeValue = 1;
        float actualSpeed = fadeSpeed * 0.01f;
        while (fadeValue > 0)
        {
            fadeValue -= actualSpeed;
            fadeImage.color = new Color(0, 0, 0, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float fadeValue = 0;
        float actualSpeed = fadeSpeed * 0.01f;
        while (fadeValue < 1)
        {
            fadeValue += actualSpeed;
            fadeImage.color = new Color(0, 0, 0, fadeValue);
            yield return new WaitForSeconds(actualSpeed);
        }
    }

    public IEnumerator AsyncChangeScene(string sceneName)
    {
        Debug.Log("1-1");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("1-2");
        asyncLoad.allowSceneActivation = false;
        Debug.Log("1-3");

        yield return StartCoroutine(FadeOut());
        Debug.Log("1-4");

        while (asyncLoad.progress < 0.9f)
        {
        Debug.Log("1-5");
            yield return null;
        }
        Debug.Log("1-6");
        asyncLoad.allowSceneActivation = true;
        canvas.worldCamera = Camera.main;
        Debug.Log("1-7");
    }
}
