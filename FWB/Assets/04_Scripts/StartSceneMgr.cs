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

    public Button continueBtn;
    public GameObject saveLoadPanel;
    public GameObject slots1, slots2, slots3;
    public Button toLeft, toRight;
    public Camera mainCam;
    public Button load;
    public Button delete;
    public Button returnBtn;
    public List<Button> popupYes = new List<Button>();
    public List<Button> popupNo = new List<Button>();
    public List<Button> saveLoadButtons = new List<Button>();
    public Sprite selectedSaveSlot;
    public Sprite defaultSaveSlot;
    public Sprite noDataSaveSlot;
    public GameObject loadPopup;
    public GameObject deletePopup;
    public GameObject popUpDim;
    public GameObject noDataPopup;
    private string saveSlot;
    private Image lastSelectedSlotImage = null;
    public static bool isSavePopupActive = false;
    private bool isLoading = false;
    private bool isDeleting = false;


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
        continueBtn.gameObject.SetActive(true);
        continueBtn.onClick.AddListener(OpenSavePanel);
        load.onClick.RemoveAllListeners();
        load.onClick.AddListener(OnClickDataLoad);
        delete.onClick.RemoveAllListeners();
        delete.onClick.AddListener(OnClickDelete);
        returnBtn.onClick.AddListener(OnClickReturn);foreach (var btn in popupYes)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickPopupYes);
        }
        foreach (var btn in popupNo)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickPopupNo);
        }
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
            isLoading = true;
            popUpDim.SetActive(true);
            loadPopup.SetActive(true);
        }
        else
        {
            popUpDim.SetActive(true);
            StartCoroutine(NoDataBlinker());
        }
    }
    
    public void OnClickDelete()
    {
        if (File.Exists(Application.persistentDataPath + "/saves" + "/" + saveSlot + ".json"))
        {
            isDeleting = true;
            popUpDim.SetActive(true);
            deletePopup.SetActive(true);
        }
        else
        {
            popUpDim.SetActive(true);
            StartCoroutine(NoDataBlinker());
        }
    }
    
    private void OnClickPopupYes()
    {
        if (isDeleting)
        {
            DataSaveLoad.dataSave.DeleteData(saveSlot);
            deletePopup.SetActive(false);
            DataSaveLoad.dataSave.DeleteSS(saveSlot);
            GameObject slotGo = GameObject.Find(saveSlot);
            Image prevImage = slotGo.transform.Find("Mask/Preview").GetComponent<Image>();
            prevImage.sprite = noDataSaveSlot;
        }
        else
        {
            DataSaveLoad.dataSave.LoadData(saveSlot);
            loadPopup.SetActive(false);
            isSavePopupActive = false;
        }
        popUpDim.SetActive(false);
        GameObject slotObj = GameObject.Find(saveSlot);
        slotObj.GetComponent<SaveSlot>().CallSlotInfo();
        isLoading = false;
        isDeleting = false;
    }
    
    private void OnClickPopupNo()
    {
        popUpDim.SetActive(false);
        loadPopup.SetActive(false);
        deletePopup.SetActive(false);
        isLoading = false;
        isDeleting = false;
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
