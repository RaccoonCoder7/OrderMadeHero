using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UGUI로 구현할 수 없는 기능을 구현하기 위한 커스텀 스크롤바
/// </summary>
public class CustomScrollBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollBar;
    public RectTransform rect;
    public float minRange;
    public float maxRange;
    public Canvas canvas;

    private bool buttonPressed = false;
    private float overValue;
    private float range;


    void Start()
    {
        range = maxRange - minRange;
    }

    void Update()
    {
        if (buttonPressed) return;

        Vector3 localPos = rect.localPosition;
        localPos.y = scrollBar.value * range + minRange;
        if (localPos.y < minRange) localPos.y = minRange;
        if (localPos.y > maxRange) localPos.y = maxRange;
        rect.localPosition = localPos;
    }

    public void AutoScrollToDown()
    {
        scrollBar.value = 0;
        rect.localPosition = new Vector3(rect.localPosition.x, minRange, rect.localPosition.z);
    }

    public IEnumerator DelayScroll()
    {
        yield return null;
        AutoScrollToDown();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        buttonPressed = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var tempVector = Vector3.zero;
        tempVector.y = eventData.delta.y / canvas.scaleFactor;
        if (overValue == 0)
        {
            rect.localPosition += tempVector;
            if (rect.localPosition.y > maxRange)
            {
                overValue = rect.localPosition.y - maxRange;
                tempVector = rect.localPosition;
                tempVector.y = maxRange;
                rect.localPosition = tempVector;
            }
            if (rect.localPosition.y < minRange)
            {
                overValue = rect.localPosition.y - minRange;
                tempVector = rect.localPosition;
                tempVector.y = minRange;
                rect.localPosition = tempVector;
            }
        }
        else
        {
            if (tempVector.y > 0)
            {
                if (overValue < 0)
                {
                    overValue += tempVector.y;
                    if (overValue > 0)
                    {
                        tempVector.y = overValue;
                        rect.localPosition += tempVector;
                        overValue = 0;
                    }
                }
                else
                {
                    overValue += tempVector.y;
                }
            }
            else
            {
                if (overValue < 0)
                {
                    overValue += tempVector.y;
                }
                else
                {
                    overValue += tempVector.y;
                    if (overValue < 0)
                    {
                        tempVector.y = overValue;
                        rect.localPosition += tempVector;
                        overValue = 0;
                    }
                }
            }
        }

        scrollBar.value = (rect.localPosition.y - minRange) / range;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        buttonPressed = false;
        overValue = 0;
    }
}
