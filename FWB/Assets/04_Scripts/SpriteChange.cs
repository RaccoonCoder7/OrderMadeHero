using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpriteChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite onSprite;
    public Sprite offSprite;
    public SpriteChangeType type = SpriteChangeType.OnFocus;
    public Image img;
    public float SecPerAutoChange;

    private Coroutine routine;

    public enum SpriteChangeType
    {
        OnFocus,
        OnClick,
        Auto
    }

    private void Start()
    {
        img ??= GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (type == SpriteChangeType.Auto)
        {
            routine = StartCoroutine(StartMove());
        }
    }

    private void OnDisable()
    {
        if (type == SpriteChangeType.Auto)
        {
            SetOnSprite();
            StopCoroutine(routine);
        }

        if (type == SpriteChangeType.OnFocus)
        {
            SetOffSprite();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (type == SpriteChangeType.OnFocus)
        {
            SetOnSprite();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (type == SpriteChangeType.OnFocus)
        {
            SetOffSprite();
        }
    }

    public void SetOnSprite()
    {
        if (!offSprite)
        {
            img.color = Vector4.one;
        }
        img.sprite = onSprite;
    }

    public void SetOffSprite()
    {
        if (!offSprite)
        {
            img.color = Vector4.zero;
            return;
        }
        img.sprite = offSprite;
    }

    private IEnumerator StartMove()
    {
        bool isOriginImg = true;
        while (true)
        {
            yield return new WaitForSeconds(SecPerAutoChange);
            if (isOriginImg)
            {
                SetOffSprite();
            }
            else
            {
                SetOnSprite();
            }
            isOriginImg = !isOriginImg;
        }
    }
}
