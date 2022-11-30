using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class IntroSceneMgr : MonoBehaviour
{
    public InputField inputField;
    public Button backBtn;
    public Transform paper;
    public GameObject paws;
    public GameObject stamp;

    [SerializeField]
    private string confirmedPlayerName;

    private Regex regex = new Regex(@"^[가-힣a-zA-Z0-9\s]{2,12}$");
    private const string playerNameRule = "한글, 영어 / 공백포함 2자 이상 12자 이하로 설정 해주세요";

    void Start()
    {
        backBtn.onClick.AddListener(OnClickBack);
        inputField.onEndEdit.AddListener(ValidateName);
        // TODO: 인트로

        // 테스트
        StartCoroutine(PlayerNameRoutine());
    }

    private IEnumerator PlayerNameRoutine()
    {
        paws.SetActive(true);
        yield return StartCoroutine(CommonTool.In.FadeIn());
        paper.DOMove(new Vector3(0, 2, 0), 1);
    }

    private IEnumerator FinishIntroRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        stamp.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(CommonTool.In.AsyncChangeScene("ChipSampleScene"));
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
            inputField.text = string.Empty;
            CommonTool.In.OpenAlertPanel(playerNameRule);
            return;
        }

        var msg = "이 이름으로 하시겠습니까? [" + playerName + "]";
        CommonTool.In.OpenConfirmPanel(msg,
        () =>
        {
            confirmedPlayerName = playerName;
            StartCoroutine(FinishIntroRoutine());
        },
        () =>
        {
            inputField.text = string.Empty;
        });
    }

    private void OnClickBack()
    {
        StartCoroutine(CommonTool.In.AsyncChangeScene("StartScene"));
    }
}
