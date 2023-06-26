using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public string playerName;
    public string mascotName;
    public List<Script> scriptList = new List<Script>();
    public List<AudioClip> audioClipList = new List<AudioClip>();

    private Canvas canvas;
    private AudioSource audioSrc;


    [System.Serializable]
    public class Script
    {
        public string key;
        public TextAsset ta;
        [HideInInspector]
        public List<string> lines = new List<string>();
    }


    void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        audioSrc = GetComponent<AudioSource>();

        alertPanel.SetActive(false);
        confirmPanel.SetActive(false);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));

        foreach (var script in scriptList)
        {
            string fileName = script.key + ".txt";
            var path = Path.Combine(Application.persistentDataPath, script.key + ".txt");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, script.ta.text);
            }
            script.lines = File.ReadAllText(path).Split('\n').ToList();
        }
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

    public List<string> GetText(string key)
    {
        return scriptList.Find(x => x.key.Equals(key))?.lines;
    }

    public void PlayOneShot(string audioName)
    {
        var clip = audioClipList.Find(x => x.name.Equals(audioName));
        if (clip != null)
        {
            audioSrc.PlayOneShot(clip);
        }
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        yield return StartCoroutine(FadeOut());

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        canvas.worldCamera = Camera.main;
    }
}
