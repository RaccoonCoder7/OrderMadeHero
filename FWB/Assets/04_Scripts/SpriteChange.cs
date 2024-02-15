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
    public float secPerAutoChange;

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
        if (type == SpriteChangeType.OnFocus)
        {
            SetOffSprite();
        }
    }

    private void OnEnable()
    {
        if (type == SpriteChangeType.Auto)
        {
            StartAutoMove();
        }
    }

    private void OnDisable()
    {
        if (type == SpriteChangeType.Auto)
        {
            SetOnSprite();
            StopAutoMove();
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

    public void StartAutoMove()
    {
        routine = StartCoroutine(StartMove());
    }

    public void StopAutoMove()
    {
        StopCoroutine(routine);
    }

    private IEnumerator StartMove()
    {
        bool isOriginImg = true;
        while (true)
        {
            yield return new WaitForSeconds(secPerAutoChange);
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
