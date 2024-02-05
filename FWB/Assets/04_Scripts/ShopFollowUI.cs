using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopFollowUI : MonoBehaviour
{
    public CanvasScaler scaler;
    public RectTransform rect;
    public Text itemName;
    public Text itemData;
    public Text price;


    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Update();
    }

    private void Update()
    {
        var pos = new Vector2(Input.mousePosition.x * scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * scaler.referenceResolution.y / Screen.height);
        pos.y -= rect.sizeDelta.y + 35;
        pos.x += 25;
        rect.anchoredPosition = pos;
    }
}
