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

    private IDialogue dialogue;

    void Start()
    {
        dialogue = FindObjectOfType<GameSceneMgr>() as IDialogue;
        if (dialogue == null)
        {
            dialogue = FindObjectOfType<IntroSceneMgr>() as IDialogue;
        }
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
        dialogue.autoTextSkip = true;
        UpdateButtonStates();
    }

    private void DisableAutoText()
    {
        dialogue.autoTextSkip = false;
        UpdateButtonStates();
    }

    private void SetTextSpeedSlow()
    {
        if (dialogue != null)
        {
            dialogue.textDelayTime = 0.06f;
            UpdateButtonStates();
        }
    }

    private void SetTextSpeedMid()
    {
        if (dialogue != null)
        {
            dialogue.textDelayTime = 0.02f;
            UpdateButtonStates();
        }
    }

    private void SetTextSpeedFast()
    {
        if (dialogue != null)
        {
            dialogue.textDelayTime = 0f;
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
        if (dialogue.autoTextSkip)
        {
            autoTextOnButton.image.sprite = autoTextOnActiveSprite;
            autoTextOffButton.image.sprite = autoTextOffInactiveSprite;
        }
        else
        {
            autoTextOnButton.image.sprite = autoTextOnInactiveSprite;
            autoTextOffButton.image.sprite = autoTextOffActiveSprite;
        }

        float delay = dialogue.textDelayTime;
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
