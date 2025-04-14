using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("���� ������Ʈ(�г�)")]
    public GameObject tooltipPanel;

    [Header("������ ǥ���� �ؽ�Ʈ ������Ʈ")]
    public Text tooltipText;

    [Header("���� �� ��ũ��Ʈ�� ǥ���ؾ� �� Ÿ��")]
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
                tooltipText.text = $"���� ��ġ: {GameMgr.In.fame}";
                break;
            case StatType.Tendency:
                tooltipText.text = $"���� ����ġ: {GameMgr.In.tendency}";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
}
