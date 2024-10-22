using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// day1의 이벤트를 제어
/// </summary>
public class EventFlowDay1 : EventFlow
{
    private Coroutine naviBlinkRoutine;
    private Coroutine blueprintButtonBlinkRoutine;

    public override void StartFlow()
    {
        GameMgr.In.isEventOn = 1;
        mgr.StartText("Tutorial", EndTutorialRoutine, SkipTutorialRoutine);
    }

    private void EndTutorialRoutine()
    {
        mgr.EndText(false);
        CommonTool.In.cancelText.color = new Color(30f / 255f, 30f / 255f, 30f / 255f, 1);

        mgr.yes.onClick.RemoveAllListeners();
        mgr.yes.onClick.AddListener(() =>
        {
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Tutorial_Explain", EndExplainRoutine, EndExplainRoutine);
        });

        mgr.no.onClick.RemoveAllListeners();
        mgr.no.onClick.AddListener(() =>
        {
            mgr.ActiveYesNoButton(false);
            mgr.StartText("Tutorial2", EndTutorial2Routine, EndTutorial2Routine);
        });

        mgr.ActiveYesNoButton(true, "좋아요", "괜찮아요");
    }

    private void SkipTutorialRoutine()
    {
        mgr.SkipToLastLine();
        EndTutorialRoutine();
    }

    private void EndExplainRoutine()
    {
        mgr.EndText();
        mgr.no.onClick.Invoke();
    }

    private void EndTutorial2Routine()
    {
        mgr.EndText(true);
        mgr.day.SetActive(true);
        mgr.goldText.text = GameMgr.In.credit.ToString();
        mgr.gold.SetActive(true);
        StartCoroutine(mgr.FadeInOutDateMessage());

        mgr.StartText("Tutorial3", EndTutorial3Routine, SkipTutorial3Routine);
    }

    private void EndTutorial3Routine()
    {
        mgr.EndText(false);
        mgr.yes.interactable = true;
        mgr.no.interactable = false;
        mgr.ActiveYesNoButton(true);
        CommonTool.In.SetFocus(new Vector2(1325, 555), new Vector2(155, 70));

        mgr.yes.onClick.AddListener(() =>
        {
            mgr.no.interactable = true;
            mgr.ActiveYesNoButton(false);
            mgr.popupPanel.SetActive(true);
            mgr.RefreshPopupPanel();
            SetWeaponDatasVisibility(false);
            var pos = mgr.popupChatPanelRect.anchoredPosition;
            pos.x = 150;
            mgr.popupChatPanelRect.anchoredPosition = pos;
            mgr.popupChatPanel.SetActive(true);
            CommonTool.In.SetFocusOff();
            mgr.StartText("Tutorial4", EndTutorial4Routine, EndTutorial4Routine);
        });
    }

    private void SkipTutorial3Routine()
    {
        mgr.chatTarget = GameSceneMgr.ChatTarget.Main;
        mgr.chatTargetText = mgr.mainChatText;
        mgr.popupChatPanel.SetActive(false);
        mgr.mainChatPanel.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatName.text = "손님";
        mgr.MobSpriteRandomChange();
        mgr.imageList.Find(x => x.key.Equals("손님")).imageObj.SetActive(true);
        mgr.SkipToLastLine();

        EndTutorial3Routine();
    }

    private void EndTutorial4Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);
        CommonTool.In.SetFocus(new Vector2(285, 620), new Vector2(60, 60));
        blueprintButtonBlinkRoutine = StartCoroutine(BlinkBlueprintButton());
        mgr.bluePrintSlotList[0].button.onClick.AddListener(OnClickBlueprintSlot);
    }

    private void OnClickBlueprintSlot()
    {
        CommonTool.In.SetFocusOff();
        mgr.StartText("Tutorial5_1", EndTutorial5_1Routine, EndTutorial5_1Routine);
        StopCoroutine(blueprintButtonBlinkRoutine);
        SetWeaponDatasVisibility(true);
        mgr.bluePrintSlotList[0].image.color = Color.white;
        mgr.bluePrintSlotList[0].button.onClick.RemoveListener(OnClickBlueprintSlot);
    }

    private void EndTutorial5_1Routine()
    {
        mgr.EndText();
        mgr.popupChatPanel.SetActive(false);
        CommonTool.In.SetFocus(new Vector2(950, 170), new Vector2(280, 90));
        mgr.weaponCreate.onClick.AddListener(OnClickWeaponCreate);
    }

    private void OnClickWeaponCreate()
    {
        CommonTool.In.SetFocusOff();
        var confirmBtnPos = CommonTool.In.confirmBtn.transform.position;
        CommonTool.In.confirmBtn.transform.position = confirmBtnPos - new Vector3(13, 0, 0);
        CommonTool.In.cancelBtn.image.enabled = false;
        CommonTool.In.cancelText.enabled = false;
        CommonTool.In.confirmBtn.onClick.AddListener(OnClickConfirm);
        mgr.weaponCreate.onClick.RemoveListener(OnClickWeaponCreate);
    }

    private void OnClickConfirm()
    {
        mgr.StartText("Tutorial5_2", EndTutorial5_2Routine, EndTutorial5_2Routine);
        var confirmBtnPos = CommonTool.In.confirmBtn.transform.position;
        CommonTool.In.confirmBtn.transform.position = confirmBtnPos + new Vector3(13, 0, 0);
        CommonTool.In.cancelBtn.image.enabled = true;
        CommonTool.In.cancelText.enabled = true;
    }

    private void EndTutorial5_2Routine()
    {
        mgr.EndText();
        CommonTool.In.SetFocusOff();
        mgr.popupChatPanel.SetActive(false);
        mgr.puzzleMgr.OnMakingDone += OnMakingDone;
    }

    private void OnMakingDone(int result)
    {
        GameMgr.In.dayCustomerCnt = 1;
        StartCoroutine(ShowEmoji());
        mgr.StartText("Tutorial6", EndTutorial6Routine, SkipTutorial6Routine);
        mgr.puzzleMgr.OnMakingDone -= OnMakingDone;
    }

    private void EndTutorial6Routine() //청사진 획득 이벤트 
    {
        mgr.EndText();

        mgr.mainChatPanel.SetActive(false);
        mgr.alertPanel.SetActive(true);
        mgr.alertDodge.onClick.RemoveAllListeners();

        foreach (var category in GameMgr.In.weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in category.bluePrintList)
            {
                if (bp.orderEnable && bp.createEnable) continue;
                bool enable = bp.howToGet.Equals("튜토리얼");
                bp.orderEnable = enable;
                bp.createEnable = enable;
            }
        }

        foreach (var order in GameMgr.In.orderTable.orderList)
        {
            if (order.orderConditionDictionary.ContainsKey("튜토리얼"))
            {
                order.orderConditionDictionary["튜토리얼"] = true;
                if (!order.orderConditionDictionary.ContainsValue(false))
                {
                    order.orderEnable = true;
                }
            }
        }

        mgr.alertDodge.onClick.AddListener(() =>
        {
            mgr.alertPanel.SetActive(false);
            mgr.StartText("Tutorial7", EndTutorial7Routine, SkipTutorial7Routine);
        });
    }

    private void SkipTutorial6Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("손님")).imageObj.SetActive(false);
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(true);
        mgr.pcChatPanel.SetActive(false);
        mgr.chatName.text = "샤일로";
        EndTutorial6Routine();
    }

    private void EndTutorial7Routine()
    {
        mgr.EndText();

        naviBlinkRoutine = StartCoroutine(mgr.BlinkNavi());
        mgr.StartText("Tutorial8", EndTutorial8Routine, EndTutorial8Routine);
    }

    private void SkipTutorial7Routine()
    {
        mgr.imageList.Find(x => x.key.Equals("샤일로")).imageObj.SetActive(false);
        mgr.mainChatPanel.SetActive(false);
        EndTutorial7Routine();
    }

    private void EndTutorial8Routine()
    {
        mgr.EndText();
        mgr.prevChatTarget = GameSceneMgr.ChatTarget.None;
        mgr.pcChatPanel.SetActive(false);
        CommonTool.In.SetFocusOff();

        mgr.pc.image.raycastTarget = true;
        mgr.pc.onClick.RemoveAllListeners();
        mgr.pc.onClick.AddListener(() =>
        {
            StopCoroutine(naviBlinkRoutine);
            mgr.deskNavi.SetActive(true);
            mgr.RefreshCreditPanel();
            CommonTool.In.cancelText.color = Color.white;
            mgr.creditPanel.SetActive(true);
            mgr.creditDodge.onClick.RemoveAllListeners();
            mgr.creditDodge.onClick.AddListener(() =>
            {
                EndFlow();
            });
            mgr.pc.onClick.RemoveAllListeners();
            mgr.pc.image.raycastTarget = false;
        });
    }

    private void SetWeaponDatasVisibility(bool isShow)
    {
        mgr.weaponDatasBlock.SetActive(isShow);
    }

    private IEnumerator BlinkBlueprintButton()
    {
        var targetImg = mgr.bluePrintSlotList[0].image;
        bool isOn = false;
        while (true)
        {
            targetImg.color = isOn ? Color.black : Color.white;
            isOn = !isOn;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ShowEmoji()
    {
        SoundManager.PlayOneShot("success");
        mgr.emoji.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        mgr.emoji.gameObject.SetActive(false);
    }
}
