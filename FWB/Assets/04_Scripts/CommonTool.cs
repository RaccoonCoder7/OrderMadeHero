using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PuzzleMgr;
using static WeaponDataTable;

/// <summary>
/// 앱 전반적으로 흔히 사용 될만한 기능을 모아둔 공용 툴
/// </summary>
public class CommonTool : SingletonMono<CommonTool>
{
    public GameObject alertPanel;
    public GameObject confirmPanel;
    public GameObject focusPanel;
    public ShopFollowUI shopFollowUI;
    public RectTransform focusRectTr;
    public RectTransform focusMaskRectTr;
    public RectTransform focusMaskRectTr_Left;
    public RectTransform focusMaskRectTr_Right;
    public RectTransform focusMaskRectTr_Top;
    public RectTransform focusMaskRectTr_Bottom;
    public Text alertText;
    public Text confirmText;
    public Button alertDodgeBtn;
    public Button confirmBtn;
    public Button cancelBtn;
    public Text cancelText;
    public Image fadeImage;
    public float fadeSpeed;
    public string playerName;
    public string mascotName = "나비";
    public string pcMascotName = "PC나비";
    public List<Script> scriptList = new List<Script>();
    public Canvas canvas;

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

        alertPanel.SetActive(false);
        confirmPanel.SetActive(false);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));

        foreach (var script in scriptList)
        {
            // Save text to local and use local file
            // string fileName = script.key + ".txt";
            // var path = Path.Combine(Application.persistentDataPath, script.key + ".txt");
            // if (!File.Exists(path))
            // {
            //     File.WriteAllText(path, script.ta.text);
            // }
            // script.lines = File.ReadAllText(path).Split('\n').ToList();

            // Use TextAsset
            script.lines = script.ta.text.Split('\n').ToList();
        }
    }

    /// <summary>
    /// 사용자의 Yes/No 응답을 받는 패널을 열음
    /// </summary>
    /// <param name="text">패널에 띄울 메세지</param>
    /// <param name="OnConfirm">Yes 선택 시 동작할 Action</param>
    /// <param name="OnCancel">No 선택 시 동작할 Action</param>
    public void OpenConfirmPanel(string text, Action OnConfirm, Action OnCancel = null)
    {
        confirmPanel.SetActive(true);
        confirmText.text = text;
        confirmBtn.onClick.AddListener(() =>
        {
            confirmPanel.SetActive(false);
            OnConfirm.Invoke();
            confirmBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
        });
        cancelBtn.onClick.AddListener(() =>
        {
            confirmPanel.SetActive(false);
            OnCancel?.Invoke();
            confirmBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
        });
    }

    /// <summary>
    /// 알림 패널을 열음
    /// </summary>
    /// <param name="alertText">알림 메세지</param>
    /// <param name="OnDodge">창이 닫혔을 경우 동작할 Action</param>
    public void OpenAlertPanel(string alertText, Action OnDodge = null)
    {
        alertPanel.SetActive(true);
        this.alertText.text = alertText;

        if (OnDodge != null)
        {
            alertDodgeBtn.onClick.RemoveAllListeners();
            alertDodgeBtn.onClick.AddListener(() =>
            {
                OnDodge.Invoke();
                alertPanel.SetActive(false);
                alertDodgeBtn.onClick.RemoveAllListeners();
                alertDodgeBtn.onClick.AddListener(() => { alertPanel.SetActive(false); });
            });
        }
    }

    /// <summary>
    /// 스크립트(대화)를 List<string> 형태로 가져옴 
    /// </summary>
    /// <param name="key">스크립트 키</param>
    public List<string> GetText(string key)
    {
        return scriptList.Find(x => x.key.Equals(key))?.lines;
    }

    /// <summary>
    /// 화면에 포커스를 줌
    /// </summary>
    public void SetFocus(Vector2 pos, Vector2 size, params RectTransform[] excludeRects)
    {
        focusPanel.SetActive(true);

        var focusMaskLocalPos = focusMaskRectTr.anchoredPosition;
        focusMaskLocalPos.x = pos.x;
        focusMaskLocalPos.y = pos.y;
        focusMaskRectTr.anchoredPosition = focusMaskLocalPos;
        focusMaskRectTr.sizeDelta = size;

        var focusLocalPos = focusRectTr.anchoredPosition;
        focusLocalPos.x = -pos.x;
        focusLocalPos.y = -pos.y;
        focusRectTr.anchoredPosition = focusLocalPos;

        focusMaskRectTr_Left.sizeDelta = new Vector2(pos.x, 1080);
        focusMaskRectTr_Right.sizeDelta = new Vector2(1920 - (pos.x + size.x), 1080);
        focusMaskRectTr_Top.sizeDelta = new Vector2(1920, 1080 - (pos.y + size.y));
        focusMaskRectTr_Bottom.sizeDelta = new Vector2(1920, pos.y);

        if (excludeRects != null)
        {
            foreach (var excludeRect in excludeRects)
            {
                if (excludeRect != null)
                {
                    ExcludeRect(excludeRect);
                }
            }
        }
    }

    /// <summary>
    /// Exclude
    /// </summary>
    /// 

    private void ExcludeRect(RectTransform excludeRect)
    {
        Vector3[] corners = new Vector3[4];
        excludeRect.GetWorldCorners(corners);

        Vector2 excludeLeftBottomLocal;
        Vector2 excludeRightTopLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(focusMaskRectTr, corners[0], null, out excludeLeftBottomLocal);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(focusMaskRectTr, corners[2], null, out excludeRightTopLocal);

        focusMaskRectTr_Left.sizeDelta = new Vector2(Mathf.Min(focusMaskRectTr_Left.sizeDelta.x, excludeLeftBottomLocal.x), 1080);
        focusMaskRectTr_Right.sizeDelta = new Vector2(Mathf.Min(focusMaskRectTr_Right.sizeDelta.x, 1920 - excludeRightTopLocal.x), 1080);
        focusMaskRectTr_Top.sizeDelta = new Vector2(1920, Mathf.Min(focusMaskRectTr_Top.sizeDelta.y, 1080 - excludeRightTopLocal.y));
        focusMaskRectTr_Bottom.sizeDelta = new Vector2(1920, Mathf.Min(focusMaskRectTr_Bottom.sizeDelta.y, excludeLeftBottomLocal.y));
    }

    /// <summary>
    /// 포커스 효과를 없앰
    /// </summary>
    public void SetFocusOff()
    {
        focusPanel.SetActive(false);
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
        SoundManager.BGMPlayer(sceneName);
    }

    public Puzzle GetPuzzle(BluePrint bp = null)
    {
        Puzzle puzzle = new Puzzle();
        if (bp == null)
        {
            bp = GameMgr.In.currentBluePrint;
        }
        var lines = bp.puzzleCsv.text.Split('\n');
        var lineList = new List<string>();
        foreach (var line in lines)
        {
            var trimLine = line.Trim();
            if (!string.IsNullOrEmpty(trimLine))
            {
                lineList.Add(trimLine);
            }
        }
        puzzle.y = lineList.Count;
        puzzle.x = lineList[0].Split(',').Length;
        puzzle.frameDataTable = new PuzzleFrameData[puzzle.y, puzzle.x];
        for (int i = 0; i < lineList.Count; i++)
        {
            var elements = lineList[i].Split(',');
            if (i == 0) puzzle.x = elements.Length;

            for (int j = 0; j < elements.Length; j++)
            {
                int targetNum = 0;
                if (!Int32.TryParse(elements[j], out targetNum))
                {
                    Debug.Log("퍼즐조각정보 로드 실패. puzzle" + GameMgr.In.currentBluePrint.puzzleCsv.text + ": " + i + ", " + j);
                    return null;
                }
                puzzle.frameDataTable[i, j] = new PuzzleFrameData(targetNum, j, i);
            }
        }

        return puzzle;
    }
}
