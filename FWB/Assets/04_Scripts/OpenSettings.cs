using UnityEngine;
using UnityEngine.UI;

public class OpenSettings : MonoBehaviour
{
    [SerializeField] private GameObject exMenu;

    private Button settingsButton;

    void Start()
    {
        if (exMenu == null)
        {
            exMenu = GameObject.Find("EX_Menu");
        }

        settingsButton = GameObject.Find("SettingBtn")?.GetComponent<Button>();

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }
    }

    private void OpenSettingsMenu()
    {
        if (exMenu != null)
        {
            exMenu.SetActive(!exMenu.activeSelf);
            SoundManager.InitializeVolumeBars();
        }
    }
}
