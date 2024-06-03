using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button autoTextOnButton;
    [SerializeField] private Button autoTextOffButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private Button textSpeedSlowButton;
    [SerializeField] private Button textSpeedMidButton;
    [SerializeField] private Button textSpeedFastButton;

    private GameSceneMgr gameSceneMgr;
    void Start()
    {
        gameSceneMgr = GameSceneMgr.Instance;
        // 버튼에 이벤트 리스너 연결
        autoTextOnButton.onClick.AddListener(EnableAutoText);
        autoTextOffButton.onClick.AddListener(DisableAutoText);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        textSpeedSlowButton.onClick.AddListener(SetTextSpeedSlow);
        textSpeedMidButton.onClick.AddListener(SetTextSpeedMid);
        textSpeedFastButton.onClick.AddListener(SetTextSpeedFast);

        UpdateButtonStates();
    }

    private void EnableAutoText()
    {
        gameSceneMgr.autoTextSkip = true;
        UpdateButtonStates();
    }

    private void DisableAutoText()
    {
        gameSceneMgr.autoTextSkip = false;
        UpdateButtonStates();
    }

    private void SetTextSpeedSlow()
    {
        if (gameSceneMgr != null)
        {
            gameSceneMgr.textDelayTime = 0.2f;
            UpdateButtonStates();
        }
    }

    private void SetTextSpeedMid()
    {
        if (gameSceneMgr != null)
        {
            gameSceneMgr.textDelayTime = 0.02f;
            UpdateButtonStates();
        }
    }

    private void SetTextSpeedFast()
    {
        if (gameSceneMgr != null)
        {
            gameSceneMgr.textDelayTime = 0.002f;
            UpdateButtonStates();
        }
    }

    private void UpdateButtonStates()
    {
        if (gameSceneMgr != null)
        {
            float delay = gameSceneMgr.textDelayTime;
            textSpeedSlowButton.interactable = delay != 0.2f;
            textSpeedMidButton.interactable = delay != 0.02f;
            textSpeedFastButton.interactable = delay != 0.002f;

        }
    }

    private void CloseSettings()
    {
        // 설정 메뉴 비활성화
        settingsMenu.SetActive(false);
    }
}
