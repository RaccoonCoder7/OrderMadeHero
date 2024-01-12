using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpriteChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite onSprite;
    public Sprite offSprite;
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    private void OnDisable()
    {
        img.sprite = offSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = onSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = offSprite;
    }
}
