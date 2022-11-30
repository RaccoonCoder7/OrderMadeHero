using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;

public class IntroSceneMgr : MonoBehaviour
{
    public InputField inputField;
    public Button backBtn;
    public Transform paper;
    public GameObject paws;
    public GameObject stamp;
    [Header("Conversation")]
    public Text targetText;
    public TextAsset ta;
    public float textDelayTime;

    private string confirmedPlayerName;
    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    private List<string> lines = new List<string>();
    private int lineCnt = -1;
    private string prevText;
    private Regex regex = new Regex(@"^[가-힣a-zA-Z0-9\s]{2,12}$");
    private const string playerNameRule = "한글, 영어 / 공백포함 2자 이상 12자 이하로 설정 해주세요";

    IEnumerator Start()
    {
        backBtn.onClick.AddListener(OnClickBack);
        inputField.onEndEdit.AddListener(ValidateName);
        
        lines = targetText.text.Split('\n').ToList();
        yield return StartCoroutine(CommonTool.In.FadeIn());
        isOnConversation = true;
        StartCoroutine(StartTextFlow());
        StartNextLine();
    }

    void Update()
    {
        if (!isOnConversation) return;

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (isTextFlowing)
            {
                SkipCurrLine();
                StartNextLine();
                return;
            }

            if (lines.Count <= lineCnt + 1)
            {
                isOnConversation = false;
                StartCoroutine(PlayerNameRoutine());
                return;
            }

            StartNextLine();
        }
    }

    private IEnumerator StartTextFlow()
    {
        Debug.Log(1);
        for (int i = 0; i < lines.Count; i++)
        {
        Debug.Log(2);
            if (lines[i].Trim().Equals("/"))
            {
                prevText = string.Empty;
                lineCnt++;
                continue;
            }
        Debug.Log(3);

            if (!string.IsNullOrEmpty(prevText))
            {
                prevText = prevText + "\n";
            }
        Debug.Log(4);

            isTextFlowing = true;
            for (int j = 0; j < lines[i].Length; j++)
            {
                targetText.text = prevText + lines[i].Substring(0, j + 1);
                yield return new WaitForSeconds(textDelayTime);

                if (skipLine)
                {
                    skipLine = false;
                    if (i + 1 >= lines.Count || lines[i + 1].Trim().Equals("/"))
                    {
                        lineCnt--;
                    }
                    break;
                }
            }
        Debug.Log(5);
            prevText = prevText + lines[i];
            targetText.text = prevText;
            isTextFlowing = false;

        Debug.Log(6);
            while (i >= lineCnt)
            {
                yield return new WaitForSeconds(textDelayTime);
            }
        Debug.Log(7);
        }
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

    private void SkipCurrLine()
    {
        skipLine = true;
    }

    private void StartNextLine()
    {
        lineCnt++;
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
