using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 프롤로그 씬의 동작을 관리
/// </summary>
public class IntroSceneMgr : MonoBehaviour, IDialogue
{
    public CustomScrollBar scrollBar;
    public InputField inputField;
    public Button autoBtn;
    public Button skipBtn;
    public Button historyBtn;
    public Button historyDodgeBtn;
    public Transform paper;
    public GameObject paws;
    public GameObject stamp;
    public GameObject multiply; //add
    public GameObject hardLight; //add
    public GameObject storyBackground; //add
    [Header("Conversation")]
    public GameObject textPanel;
    public GameObject historyPanel;
    public Text targetText;
    public Text historyText;
    public float textDelayTime { get; set; }
    public List<ImageData> imageList = new List<ImageData>();
    public List<Sprite> buttonImageList = new List<Sprite>();

    private bool isOnConversation;
    private bool isTextFlowing;
    private bool skipLine;
    public bool autoTextSkip { get; set; }
    private List<string> lines = new List<string>();
    private int lineCnt = -1;
    private float textSkipWaitTime = 1f;
    private string prevText;
    private TextFlowType textFlowType = TextFlowType.None;
    private IEnumerator onEndText;
    private Coroutine textFlowCoroutine;
    private Regex regex = new Regex(@"^[가-힣a-zA-Z0-9\s]{2,12}$");
    private const string playerNameRule = "한글, 영어 / 공백포함 2자 이상 12자 이하로 설정 해주세요";
    private float duration = 0.5f;

    private bool isNameBeingValidated = false;

    public enum TextFlowType
    {
        None,
        Auto,
        Fast
    }

    [System.Serializable]
    public class ImageData
    {
        public string key;
        public GameObject imageObj;
    }


    IEnumerator Start()
    {
        autoBtn.onClick.AddListener(OnClickAuto);
        skipBtn.onClick.AddListener(OnClickSkip);
        historyBtn.onClick.AddListener(OnClickHistory);
        historyDodgeBtn.onClick.AddListener(OnClickHistoryDodge);
        inputField.onEndEdit.AddListener(ValidateName);

        lines = CommonTool.In.GetText("Story1");
        yield return StartCoroutine(CommonTool.In.FadeIn());
        isOnConversation = true;
        onEndText = PlayerNameRoutine();
        textFlowCoroutine = StartCoroutine(StartTextFlow());
        StartNextLine();
        StartCoroutine(BlinkCoroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnClickChatBox();
        }
    }
    
    private IEnumerator BlinkCoroutine()
    {
        Color initialColor = inputField.colors.normalColor;
        float elapsedTime = 0;

        while (!CommonTool.In.confirmPanel.activeSelf)
        {
            Color targetColor = initialColor;

            targetColor.a = (float)(Mathf.PingPong(Time.time, 1) > 0.5 ? 0 : 0.4);

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            initialColor.a = Mathf.Lerp(initialColor.a, targetColor.a, t);

            ColorBlock colorBlock = inputField.colors;
            colorBlock.normalColor = initialColor;
            inputField.colors = colorBlock;

            yield return null;
        }
    }

    public void OnClickChatBox()
    {
        if (!isOnConversation) return;

        if (isTextFlowing)
        {
            // SkipCurrLine();
            // StartNextLine();
            return;
        }

        if (lines.Count <= lineCnt + 1)
        {
            isOnConversation = false;
            StartCoroutine(onEndText);
            return;
        }

        StartNextLine();
    }

    private IEnumerator StartTextFlow()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = ReplaceKeyword(lines[i]);
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("!"))
            {
                var commands = lines[i].Split(' ');
                List<string> imageKeyList = new List<string>();
                foreach (var command in commands)
                {
                    string com = command.Trim();
                    if (com.Contains("!image"))
                    {
                        imageKeyList.Add(com.Split('_')[1]);
                    }
                    else if (com.Equals("!next"))
                    {
                        prevText = string.Empty;
                        historyText.text += "\n";
                    }
                }

                if (imageKeyList.Count > 0)
                {
                    foreach (var img in imageList)
                    {
                        img.imageObj.SetActive(imageKeyList.Contains(img.key));
                    }
                }

                lineCnt++;
                continue;
            }

            if (!string.IsNullOrEmpty(prevText))
            {
                prevText = prevText + "\n";
            }

            isTextFlowing = true;
            historyText.text += lines[i] + "\n";
            for (int j = 0; j < lines[i].Length; j++)
            {
                targetText.text = prevText + lines[i].Substring(0, j + 1);
                yield return new WaitForSeconds(textDelayTime);
                if (j == 0)
                {
                    StartCoroutine(scrollBar.DelayScroll());
                }

                if (skipLine)
                {
                    skipLine = false;
                    if (i + 1 >= lines.Count || lines[i + 1].Contains("!next"))
                    {
                        lineCnt--;
                    }
                    break;
                }
            }
            prevText = prevText + lines[i];
            targetText.text = prevText;
            isTextFlowing = false;

            while (i >= lineCnt)
            {
                if (autoTextSkip)
                {
                    yield return new WaitForSeconds(textSkipWaitTime);
                    if (autoTextSkip)
                    {
                        lineCnt++;
                        continue;
                    }
                }
                yield return new WaitForSeconds(textDelayTime);
            }
        }
    }

    private IEnumerator PlayerNameRoutine()
    {
        StopCoroutine(textFlowCoroutine);
        lineCnt = 0;
        prevText = string.Empty;
        targetText.text = string.Empty;

        yield return StartCoroutine(CommonTool.In.FadeOut());
        foreach (var img in imageList)
        {
            img.imageObj.SetActive(false);
        }
        autoBtn.gameObject.SetActive(false);
        skipBtn.gameObject.SetActive(false);
        historyBtn.gameObject.SetActive(false);
        historyPanel.gameObject.SetActive(false);
        storyBackground.SetActive(false);
        textPanel.SetActive(false);
        paws.SetActive(true);
        multiply.SetActive(true);
        hardLight.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(CommonTool.In.FadeIn());
        paper.DOMove(new Vector3(0, 1, 0), 1);
    }

    private IEnumerator FinishNamingRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        stamp.SetActive(true);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(CommonTool.In.FadeOut());

        autoBtn.gameObject.SetActive(true);
        skipBtn.gameObject.SetActive(true);
        historyBtn.gameObject.SetActive(true);
        textPanel.SetActive(true);
        paws.SetActive(false);
        multiply.SetActive(false);
        hardLight.SetActive(false);
        paper.gameObject.SetActive(false);
        historyText.text = string.Empty;
        lines = CommonTool.In.GetText("Story2");
        StartCoroutine(StartTextFlow());
        yield return new WaitForSeconds(0.75f);

        yield return StartCoroutine(CommonTool.In.FadeIn());
        isOnConversation = true;
        onEndText = CommonTool.In.AsyncChangeScene("GameScene");
        StartNextLine();
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
        if (isNameBeingValidated) return;

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

        isNameBeingValidated = true;
        StartCoroutine(SetInteractableDelayed(false));

        StopCoroutine(BlinkCoroutine());
        var msg = "이 이름으로 하시겠습니까? [" + playerName + "]";
        CommonTool.In.OpenConfirmPanel(msg,
        () =>
        {
            CommonTool.In.playerName = playerName;
            inputField.interactable = false;
            StartCoroutine(FinishNamingRoutine());
        },
        () =>
        {
            inputField.text = string.Empty;
            inputField.interactable = true;
            isNameBeingValidated = false;
            StartCoroutine(BlinkCoroutine());
        });
    }
    private IEnumerator SetInteractableDelayed(bool interactable)
    {
        yield return new WaitForEndOfFrame();
        inputField.interactable = interactable;
    }
    private void OnClickAuto()
    {
        switch (textFlowType)
        {
            case TextFlowType.None:
                autoTextSkip = true;
                autoBtn.image.sprite = buttonImageList[1];
                textFlowType = TextFlowType.Auto;
                break;
            case TextFlowType.Auto:
                textDelayTime /= 2;
                autoBtn.image.sprite = buttonImageList[2];
                textFlowType = TextFlowType.Fast;
                break;
            case TextFlowType.Fast:
                autoTextSkip = false;
                textDelayTime *= 2;
                autoBtn.image.sprite = buttonImageList[0];
                textFlowType = TextFlowType.None;
                break;
        }
    }

    private void OnClickSkip()
    {
        isOnConversation = false;
        historyPanel.SetActive(false);
        StartCoroutine(onEndText);
    }

    private void OnClickHistory()
    {
        StartCoroutine(scrollBar.DelayScroll());
        historyPanel.SetActive(!historyPanel.activeSelf);
    }

    private void OnClickHistoryDodge()
    {
        historyPanel.SetActive(false);
    }

    private string ReplaceKeyword(string line)
    {
        return line.Replace("{username}", CommonTool.In.playerName);
    }
}
