using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 마우스가 스프라이트메쉬 위에 올려졌을 때 반응하도록 만듦
/// </summary>
public class InteractableImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 moveOffset;

    private Image img;
    private Tweener scaleTweener;
    private Tweener positionTweener;
    private Vector3 originPos;

    private void Start()
    {
        img = GetComponent<Image>();
        originPos = img.transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleTweener != null)
        {
            scaleTweener.Kill();
        }
        scaleTweener = img.transform.DOScale(Vector3.one * 1.25f, 0.5f);

        if (positionTweener != null)
        {
            positionTweener.Kill();
        }
        positionTweener = img.transform.DOMove(originPos + moveOffset, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleTweener != null)
        {
            scaleTweener.Kill();
        }
        scaleTweener = img.transform.DOScale(Vector3.one, 0.5f);

        if (positionTweener != null)
        {
            positionTweener.Kill();
        }
        positionTweener = img.transform.DOMove(originPos, 0.5f);
    }
}
