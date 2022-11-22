using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class InitSceneMgr2 : MonoBehaviour
{
    public InputField inputField;
    public Button alertDodgeBtn;
    public Button backBtn;
    public Button confirmBtn;
    public Button cancelBtn;
    public Transform paper;
    public Text alertText;
    public Text confirmText;
    public GameObject alertPanel;
    public GameObject confirmPanel;
    public GameObject paws;
    public Image fadeImage;
    public float fadeSpeed;

    [SerializeField]
    private string confirmedPlayerName;

    private Regex regex = new Regex(@"^[가-힣a-zA-Z0-9\s]{2,12}$");
    private const string playerNameRule = "한글, 영어 / 공백포함 2자 이상 12자 이하로 설정 해주세요";

    void Start()
    {
        alertPanel.SetActive(false);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));
        backBtn.onClick.AddListener(OnClickBack);
        inputField.onEndEdit.AddListener(ValidateName);
        // TODO: 인트로

        // 테스트
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(PlayerNameRoutine());
    }

    private IEnumerator PlayerNameRoutine()
    {
        paws.SetActive(true);
        yield return StartCoroutine(FadeIn());
        paper.DOMove(new Vector3(0, 2, 0), 1);
    }

    private IEnumerator FadeIn()
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

    private IEnumerator FadeOut()
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

    private void ValidateName(string playerName)
    {
        if (playerName.Trim().Equals(string.Empty))
        {
            inputField.text = string.Empty;
            return;
        }

        if (!regex.IsMatch(playerName))
        {
            alertPanel.SetActive(true);
            alertText.text = playerNameRule;
            inputField.text = string.Empty;
            return;
        }

        var msg = "이 이름으로 하시겠습니까? [" + playerName + "]";
        OpenConfirmPanel(msg, () =>
            {
                confirmedPlayerName = playerName;
                // TODO: 도장, 페이드아웃 코루틴
            },
            () =>
            {
                inputField.text = string.Empty;
            });
    }

    private void OpenConfirmPanel(string text, Action OnConfirm, Action OnCancel)
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

    private void OnClickBack()
    {
        SceneManager.LoadScene("StartScene");
    }
}
