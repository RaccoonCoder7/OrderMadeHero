using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryUI : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panelBlind1;
    public GameObject panelBlind2;
    public GameObject folder1;
    public GameObject folder2;
    public GameObject popupPanel;
    public Button popupDodge;
    public Button goLeft;
    public Button goRight;
    public Sprite fileDisabledImg;
    public Sprite fileEnabledImg;
    public Text clueName;
    public Text clue;
    public Text nameUI;
    public GameObject gallery;
    public Button galleryDodge;
    public List<Sprite> galleryImages;
    public List<GameObject> pictureSelectObjects;
    public List<string> clues;
    public List<Sprite> profileSprites;
    private int characterNum;
    private WeekState weekState = WeekState.Week1;
    //private SlotState slotState = SlotState.First;
    private Image panelImg1;
    private Image panelImg2;
    private List<GameObject> files1;
    private List<GameObject> files2;
    
    private enum WeekState
    {
        Week1,
        Week2,
        Week3
    }
    
    /*
    private enum SlotState
    {
        First,
        Second,
        Third
    }
    */

    private void OnEnable()
    {
        nameUI.text = "캐 릭 터 를   클 릭 하 세 요";

        //slotState = SlotState.First;
        characterNum = 0;
        panelImg1 = panel1.GetComponent<Image>();
        panelImg2 = panel2.GetComponent<Image>();
        Button panelBtn1 = panel1.GetComponent<Button>();
        Button panelBtn2 = panel2.GetComponent<Button>();
        panelBtn1.onClick.RemoveAllListeners();
        panelBtn2.onClick.RemoveAllListeners();
        popupDodge.onClick.RemoveAllListeners();
        popupDodge.onClick.AddListener(OnClickPopupDodge);
        galleryDodge.onClick.AddListener(OnClickGalleryDodge);
        
        switch (GameMgr.In.newsProgress)
        {
            case 0:
                weekState = WeekState.Week1;
                break;
            case 1:
                weekState = WeekState.Week2;
                break;
            case 2:
                weekState = WeekState.Week3;
                break;
        }

        switch (weekState)
        {
            case WeekState.Week1:
                panelImg1.sprite = profileSprites[0];
                panelImg2.sprite = profileSprites[0];
                break;
            case WeekState.Week2:
                panelImg1.sprite = profileSprites[1];
                panelImg2.sprite = profileSprites[2];
                break;
            case WeekState.Week3:
                panelImg1.sprite = profileSprites[1];
                panelImg2.sprite = profileSprites[2];
                break;
        }

        if (weekState != WeekState.Week1)
        {
            panelBtn1.onClick.AddListener(OnClickPanel1);
            panelBtn2.onClick.AddListener(OnClickPanel2);
        }
        
        files1 = new List<GameObject>();
        files2 = new List<GameObject>();

        foreach(Transform child in folder1.transform)
        {
            files1.Add(child.gameObject);
        }

        foreach(Transform child in folder2.transform)
        {
            files2.Add(child.gameObject);
        }
        
        folder1.SetActive(false);
        folder2.SetActive(false);
        panelBlind1.SetActive(false);
        panelBlind2.SetActive(false);
        popupPanel.SetActive(false);
        gallery.SetActive(false);
    }

    private void OnClickPanel1()
    {
        folder1.SetActive(true);
        panelImg1.sprite = profileSprites[1];
        panelBlind1.SetActive(true);
        panelBlind2.SetActive(false);
        nameUI.text = "버니";
        characterNum = 1;
        
        if (panel2.activeSelf)
        {
            panelImg2.sprite = profileSprites[2];
            folder2.SetActive(false);
        }
        
        foreach (var file in files1)
        {
            Button btn = file.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OnClickFile1(btn); });
        }

        switch (weekState)
        {
            case WeekState.Week2:
                files1[0].GetComponent<Image>().sprite = fileEnabledImg;
                files1[1].GetComponent<Image>().sprite = fileDisabledImg;
                files1[2].GetComponent<Image>().sprite = fileDisabledImg;
                break;
            case WeekState.Week3:
                files1[0].GetComponent<Image>().sprite = fileEnabledImg;
                files1[1].GetComponent<Image>().sprite = fileEnabledImg;
                files1[2].GetComponent<Image>().sprite = fileDisabledImg;
                break;
        }
        files1[3].GetComponent<Image>().sprite = fileEnabledImg;
    }

    private void OnClickPanel2()
    {
        folder2.SetActive(true);
        panelImg2.sprite = profileSprites[2];
        panelBlind2.SetActive(true);
        panelBlind1.SetActive(false);
        nameUI.text = "퍼펫";
        characterNum = 2;
        
        if (panel1.activeSelf)
        {
            panelImg1.sprite = profileSprites[1];
            folder1.SetActive(false);
        }

        foreach (var file in files2)
        {
            Button btn = file.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { OnClickFile2(btn); });
        }
        
        switch (weekState)
        {
            case WeekState.Week2:
                files2[0].GetComponent<Image>().sprite = fileEnabledImg;
                files2[1].GetComponent<Image>().sprite = fileDisabledImg;
                files2[2].GetComponent<Image>().sprite = fileDisabledImg;
                break;
            case WeekState.Week3:
                files2[0].GetComponent<Image>().sprite = fileEnabledImg;
                files2[1].GetComponent<Image>().sprite = fileEnabledImg;
                files2[2].GetComponent<Image>().sprite = fileDisabledImg;
                break;
        }
        files2[3].GetComponent<Image>().sprite = fileEnabledImg;
    }

    private void OnClickFile1(Button btn)
    {
        popupPanel.SetActive(true);
        panelImg2.sprite = profileSprites[0];
        switch (weekState)
        { 
            case WeekState.Week2:
                if (btn.gameObject.name == "File1")
                {
                    ClueGalOff();
                    clue.gameObject.SetActive(true);
                    clueName.text = "버니 파일01";
                    clue.text = clues[0];
                }
                else if (btn.gameObject.name == "File4")
                {
                    ClueGalOff();
                    foreach (var obj in pictureSelectObjects)
                    {
                        obj.SetActive(true);
                    }
                    clueName.text = "버니 갤러리";
                    pictureSelectObjects[0].GetComponent<Text>().text = "버니 사진1";
                    Button pic1 = pictureSelectObjects[0].GetComponent<Button>();
                    pic1.onClick.RemoveAllListeners();
                    pic1.onClick.AddListener(() => { characterNum = 1; OnClickOpenGallery(); });
                }
                break;
            case WeekState.Week3:
                if (btn.gameObject.name == "File1")
                {
                    ClueGalOff();
                    clue.gameObject.SetActive(true);
                    clueName.text = "버니 파일01";
                    clue.text = clues[0];
                }
                else if (btn.gameObject.name == "File2")
                {
                    ClueGalOff();
                    clue.gameObject.SetActive(true);
                    clueName.text = "버니 파일02";
                    clue.text = clues[1];
                }
                else if (btn.gameObject.name == "File4")
                {
                    ClueGalOff();
                    foreach (var obj in pictureSelectObjects)
                    {
                        obj.SetActive(true);
                    }
                    clueName.text = "버니 갤러리";
                    pictureSelectObjects[0].GetComponent<Text>().text = "버니 사진1";
                    Button pic1 = pictureSelectObjects[0].GetComponent<Button>();
                    pic1.onClick.RemoveAllListeners();
                    pic1.onClick.AddListener(() => { characterNum = 1; OnClickOpenGallery(); });
                }
                break;
        }
    }

    private void OnClickFile2(Button btn)
    {
        popupPanel.SetActive(true);
        panelImg1.sprite = profileSprites[0];
        switch (weekState)
        {
            case WeekState.Week2:
                if (btn.gameObject.name == "File1")
                {
                     ClueGalOff();
                     clue.gameObject.SetActive(true);
                     clueName.text = "퍼펫 파일01";
                    clue.text = clues[2];
                }
                else if (btn.gameObject.name == "File4")
                {
                    ClueGalOff();
                    foreach (var obj in pictureSelectObjects)
                    {
                        obj.SetActive(true);
                    }
                    clueName.text = "퍼펫 갤러리";
                    pictureSelectObjects[0].GetComponent<Text>().text = "퍼펫 사진1";
                    Button pic1 = pictureSelectObjects[0].GetComponent<Button>();
                    pic1.onClick.RemoveAllListeners();
                    pic1.onClick.AddListener(() => { characterNum = 2; OnClickOpenGallery(); });
                }
                break;
            case WeekState.Week3:
                if (btn.gameObject.name == "File1")
                {
                    ClueGalOff();
                    clue.gameObject.SetActive(true);
                    clueName.text = "퍼펫 파일01";
                    clue.text = clues[2];
                }
                else if (btn.gameObject.name == "File2")
                {
                    ClueGalOff();
                    clue.gameObject.SetActive(true);
                    clueName.text = "퍼펫 파일02";
                    clue.text = clues[3];
                }
                else if (btn.gameObject.name == "File4")
                {
                    ClueGalOff();
                    foreach (var obj in pictureSelectObjects)
                    {
                        obj.SetActive(true);
                    }
                    clueName.text = "퍼펫 갤러리";
                    pictureSelectObjects[0].GetComponent<Text>().text = "퍼펫 사진1";
                    Button pic1 = pictureSelectObjects[0].GetComponent<Button>();
                    pic1.onClick.RemoveAllListeners();
                    pic1.onClick.AddListener(() => { characterNum = 2; OnClickOpenGallery(); });
                }
                break;
        }
    }

    private void ClueGalOff()
    {
        clue.gameObject.SetActive(false);
        foreach (var obj in pictureSelectObjects)
        {
            obj.SetActive(false);
        }
    }

    /*
     추후에 다른 캐릭터가 추가됐을때 좌우 슬롯 움직임 구현
    private void OnClickGoLeft()
    {
        SwitchToNextState(slotState, false);
    }

    private void OnClickGoRight()
    {
        SwitchToNextState(slotState, true);
    }
    
    private SlotState SwitchToNextState(SlotState currentState, bool isRightDirection)
    {
        switch (currentState)
        {
            case SlotState.First:
                return isRightDirection ? SlotState.Second : SlotState.Third;
            case SlotState.Second:
                return isRightDirection ? SlotState.Third : SlotState.First;
            case SlotState.Third:
                return isRightDirection ? SlotState.First : SlotState.Second;
            default:
                throw new InvalidOperationException("invalid state");
        }
    }
    */

    private void OnClickOpenGallery()
    {
        gallery.SetActive(true);
        switch (characterNum)
        {
            case 1:
                gallery.GetComponent<Image>().sprite = galleryImages[0];
                break;
            case 2:
                gallery.GetComponent<Image>().sprite = galleryImages[1];
                break;
        }
    }

    private void OnClickPopupDodge()
    {
        popupPanel.SetActive(false);
        panelImg1.sprite = profileSprites[1];
        panelImg2.sprite = profileSprites[2];
    }

    private void OnClickGalleryDodge()
    {
        gallery.SetActive(false);
    }
}
