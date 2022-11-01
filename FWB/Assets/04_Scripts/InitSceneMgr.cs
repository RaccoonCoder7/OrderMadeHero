using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class InitSceneMgr : MonoBehaviour
{
    public Text titleText;
    public Text subTitleText;
    public InputField inputField;
    public Button checkBtn;
    public Button backBtn;
    public Button alertDodgeBtn;
    public GameObject alertPanel;
    public List<string> titleNameList = new List<string>();

    [SerializeField]
    private string confirmedPlayerName;
    [SerializeField]
    private string confirmedStoreName;

    private InitState currState = InitState.None;
    private Regex regex = new Regex(@"^[가-힣a-zA-Z0-9\s]{2,12}$");

    private enum InitState
    {
        None = 0,
        PlayerNameSetted = 1,
        // StoreNameSetted = 2,
    }

    void Start()
    {
        alertPanel.SetActive(false);
        checkBtn.onClick.AddListener(OnClickCheck);
        backBtn.onClick.AddListener(OnClickBack);
        alertDodgeBtn.onClick.AddListener(() => alertPanel.SetActive(false));
        RefreshTexts();
    }

    private void OnClickCheck()
    {
        switch (currState)
        {
            case InitState.None:
                if (!regex.IsMatch(inputField.text))
                {
                    DOTween.Shake(() =>
                        subTitleText.transform.position, x => subTitleText.transform.position = x, 1, 8, 10, 0
                    );
                    return;
                }
                confirmedPlayerName = inputField.text;
                inputField.text = string.Empty;
                currState = InitState.PlayerNameSetted;
                RefreshTexts();
                break;
            case InitState.PlayerNameSetted:
                if (!regex.IsMatch(inputField.text))
                {
                    DOTween.Shake(() =>
                        subTitleText.transform.position, x => subTitleText.transform.position = x, 1, 8, 10, 0
                    );
                    return;
                }
                confirmedStoreName = inputField.text;
                // TODO: 데이터 셋, 씬이동
                break;
        }
    }

    private void OnClickBack()
    {
        switch (currState)
        {
            case InitState.None:
                ChangeToStartScene();
                break;
            case InitState.PlayerNameSetted:
                confirmedPlayerName = string.Empty;
                currState = InitState.None;
                RefreshTexts();
                break;
        }
    }

    private void RefreshTexts()
    {
        titleText.text = titleNameList[(int)currState];
    }

    private void ChangeToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

}
