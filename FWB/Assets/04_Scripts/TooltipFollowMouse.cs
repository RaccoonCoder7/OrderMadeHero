using UnityEngine;
using UnityEngine.UI;

public class TooltipFollowMouse : MonoBehaviour
{
    public RectTransform canvasRect;
    public Camera uiCamera;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 screenPos = Input.mousePosition;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            uiCamera,
            out localPoint
        );

        rectTransform.anchoredPosition = localPoint;

        Vector2 clampedPos = localPoint;

        float tooltipWidth = rectTransform.rect.width;
        float tooltipHeight = rectTransform.rect.height;

        float halfCanvasW = canvasRect.rect.width / 2f;
        float halfCanvasH = canvasRect.rect.height / 2f;

        float offset = 10f;

        float minX = -halfCanvasW + tooltipWidth / 2 + offset;
        float maxX = halfCanvasW - tooltipWidth / 2 - offset;

        float minY = -halfCanvasH + tooltipHeight / 2 + offset;
        float maxY = halfCanvasH - tooltipHeight / 2 - offset;

        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);

        rectTransform.anchoredPosition = clampedPos;
    }
}
