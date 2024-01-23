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

    public enum SpriteChangeType
    {
        OnFocus,
        OnClick
    }

    private void Start()
    {
        img ??= GetComponent<Image>();
    }

    private void OnDisable()
    {
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
        img.sprite = onSprite;
    }

    public void SetOffSprite()
    {
        img.sprite = offSprite;
    }
}
