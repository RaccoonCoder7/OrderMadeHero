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

    [SerializeField] private Sprite autoTextOnActiveSprite;
    [SerializeField] private Sprite autoTextOnInactiveSprite;
    [SerializeField] private Sprite autoTextOffActiveSprite;
    [SerializeField] private Sprite autoTextOffInactiveSprite;

    [SerializeField] private Sprite textSpeedSlowActiveSprite;
    [SerializeField] private Sprite textSpeedSlowInactiveSprite;
    [SerializeField] private Sprite textSpeedMidActiveSprite;
    [SerializeField] private Sprite textSpeedMidInactiveSprite;
    [SerializeField] private Sprite textSpeedFastActiveSprite;
    [SerializeField] private Sprite textSpeedFastInactiveSprite;

    [SerializeField] private GameSceneMgr gameSceneMgr;

    void Start()
    {
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
            gameSceneMgr.textDelayTime = 0.06f;
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
            gameSceneMgr.textDelayTime = 0f;
            UpdateButtonStates();
        }
    }

    private void CloseSettings()
    {
        // 설정 메뉴 비활성화
        settingsMenu.SetActive(false);
    }

    private void UpdateButtonStates()
    {
        if (gameSceneMgr.autoTextSkip)
        {
            autoTextOnButton.image.sprite = autoTextOnActiveSprite;
            autoTextOffButton.image.sprite = autoTextOffInactiveSprite;
        }
        else
        {
            autoTextOnButton.image.sprite = autoTextOnInactiveSprite;
            autoTextOffButton.image.sprite = autoTextOffActiveSprite;
        }

        float delay = gameSceneMgr.textDelayTime;
        if (delay == 0.06f)
        {
            textSpeedSlowButton.image.sprite = textSpeedSlowActiveSprite;
            textSpeedMidButton.image.sprite = textSpeedMidInactiveSprite;
            textSpeedFastButton.image.sprite = textSpeedFastInactiveSprite;
        }
        else if (delay == 0.02f)
        {
            textSpeedSlowButton.image.sprite = textSpeedSlowInactiveSprite;
            textSpeedMidButton.image.sprite = textSpeedMidActiveSprite;
            textSpeedFastButton.image.sprite = textSpeedFastInactiveSprite;
        }
        else if (delay == 0f)
        {
            textSpeedSlowButton.image.sprite = textSpeedSlowInactiveSprite;
            textSpeedMidButton.image.sprite = textSpeedMidInactiveSprite;
            textSpeedFastButton.image.sprite = textSpeedFastActiveSprite;
        }
    }
}
