using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 시작 씬의 동작을 관리
/// </summary>
public class StartSceneMgr : MonoBehaviour
{
    public Button startBtn;
    public Button quitBtn;
    public SpriteAnimation sa;
    
    public GameObject saveLoadPanel;
    public GameObject slots1, slots2, slots3;
    public Button toLeft, toRight;
    public Camera mainCam;
    public Button load;
    public Button returnBtn;
    public Button popupYes;
    public Button popupNo;
    public List<Button> saveLoadButtons = new List<Button>();
    public Sprite selectedSaveSlot;
    public Sprite defaultSaveSlot;
    public GameObject loadPopup;
    public GameObject popUpDim;
    public GameObject noDataPopup;
    private string saveSlot;
    private Image lastSelectedSlotImage = null;
    public static bool isSavePopupActive = false;


    void Start()
    {
        string filePath = Application.persistentDataPath + "/saves";
        
        startBtn.onClick.AddListener(ChangeToInitScene);
        quitBtn.onClick.AddListener(Quit);
        GameMgr.In.initDone = true;

        StartCoroutine(CommonTool.In.FadeIn());
        StartCoroutine(sa.StartLoopAnim());
        SoundManager.BGMPlayer(SceneManager.GetActiveScene().name);

        if (Directory.Exists(filePath) && Directory.EnumerateFiles(filePath).Any())
        {
            ActiveLoadFeature();
        }
    }

    private void ActiveLoadFeature()
    {
        DataSaveLoad.dataSave.AssignSceneObjects(slots1, slots2, slots3, toLeft, toRight, mainCam);
        // continueBtn.gameObject.SetActive(true);
        // continueBtn.onClick.AddListener(OpenSavePanel);
        load.onClick.RemoveAllListeners();
        load.onClick.AddListener(OnClickDataLoad);
        returnBtn.onClick.AddListener(OnClickReturn);
        popupYes.onClick.RemoveAllListeners();
        popupYes.onClick.AddListener(OnClickPopupYes);
        popupNo.onClick.RemoveAllListeners();
        popupNo.onClick.AddListener(OnClickPopupNo);
        foreach (var btn in saveLoadButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OnClickSlot(btn); });
        }
    }

    private void ChangeToInitScene()
    {
        StartCoroutine(CommonTool.In.AsyncChangeScene("IntroScene"));
    }

    private void OnClickSlot(Button btn)
    {
        if (lastSelectedSlotImage != null)
        {
            lastSelectedSlotImage.sprite = defaultSaveSlot;
        }
        lastSelectedSlotImage = btn.GetComponent<Image>();
        lastSelectedSlotImage.sprite = selectedSaveSlot;

        saveSlot = btn.name;
        Debug.Log(saveSlot);
    }
    
    private void OpenSavePanel()
    {
        saveLoadPanel.SetActive(true);
        isSavePopupActive = true;
    }

    private void OnClickDataLoad()
    {
        if (File.Exists(Application.persistentDataPath + "/saves" + "/" + saveSlot + ".json"))
        {
            popUpDim.SetActive(true);
            loadPopup.SetActive(true);
        }
        else
        {
            popUpDim.SetActive(true);
            StartCoroutine(NoDataBlinker());
        }
    }
    
    private void OnClickPopupYes()
    {
        DataSaveLoad.dataSave.LoadData(saveSlot);
        loadPopup.SetActive(false);
        popUpDim.SetActive(false);
        isSavePopupActive = false;
        GameObject slotObj = GameObject.Find(saveSlot);
        slotObj.GetComponent<SaveSlot>().CallSlotInfo();
    }
    
    private void OnClickPopupNo()
    {
        popUpDim.SetActive(false);
        loadPopup.SetActive(false);
    }

    private void OnClickReturn()
    {
        saveLoadPanel.SetActive(false);
        isSavePopupActive = false;
    }
    
    private IEnumerator NoDataBlinker()
    {
        noDataPopup.SetActive(true);
        float elapsedTime = 0f;
        float blinkInterval = 1f / 2f;
        bool isVisible = true;

        while (elapsedTime < 2f)
        {
            isVisible = !isVisible;
            noDataPopup.SetActive(isVisible);
            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }
        popUpDim.SetActive(false);
        noDataPopup.SetActive(false);
    }
    
    private void OpenAlertPanel()
    {
        CommonTool.In.OpenAlertPanel("현재 사용할 수 없는 기능입니다.");
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
