using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityFilterUI : MonoBehaviour
{
    public string abilityKey;
    public Button button;
    public Image image;
    public Sprite onSprite;
    public Sprite offSprite;
    public Text textForTag;
    [HideInInspector]
    public bool isOn = false;
}
