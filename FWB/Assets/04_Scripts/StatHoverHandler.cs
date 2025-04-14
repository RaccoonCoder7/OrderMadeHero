using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("툴팁 오브젝트(패널)")]
    public GameObject tooltipPanel;

    [Header("툴팁에 표시할 텍스트 컴포넌트")]
    public Text tooltipText;

    [Header("현재 이 스크립트가 표시해야 할 타입")]
    public StatType statType;

    public enum StatType
    {
        Renom,
        Tendency
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPanel.SetActive(true);

        switch (statType)
        {
            case StatType.Renom:
                tooltipText.text = $"현재 명성치: {GameMgr.In.fame}";
                break;
            case StatType.Tendency:
                tooltipText.text = $"현재 성향치: {GameMgr.In.tendency}";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
}
