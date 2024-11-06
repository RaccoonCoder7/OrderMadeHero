using UnityEngine;
using UnityEngine.UI;

public class OpenSettings : MonoBehaviour
{
    [SerializeField] private GameObject exMenu;
    [SerializeField] private Button settingsButton;

    void Start()
    {
        if (exMenu == null)
        {
            exMenu = GameObject.Find("EX_Menu");
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }
        else
        {
            Debug.LogError("Settings button is null even after assigning manually");
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
