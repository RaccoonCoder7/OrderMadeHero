using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public int fps = 24;
    public List<Sprite> textureList = new List<Sprite>();
    private Image image;
    private int textureIndex;


    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(StartLoopAnim());
    }

    public IEnumerator StartLoopAnim()
    {
        float startTime = Time.realtimeSinceStartup;
        float frameChangeTime = 1f / fps;
        float currentTime = 0f;
        while (true)
        {
            currentTime += (Time.realtimeSinceStartup - startTime);
            startTime = Time.realtimeSinceStartup;
            if (currentTime < frameChangeTime)
            {
                yield return null;
                continue;
            }
            currentTime -= frameChangeTime;
            textureIndex = textureIndex + 1 >= textureList.Count ? 0 : textureIndex + 1;
            image.sprite = textureList[textureIndex];
        }
    }
}
