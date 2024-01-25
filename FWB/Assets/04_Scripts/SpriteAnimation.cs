using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public int fps = 24;
    public List<Sprite> textureList = new List<Sprite>();
    [HideInInspector]
    public int textureIndex;
    private Image image;


    void Start()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator StartLoopAnim()
    {
        float startTime = Time.realtimeSinceStartup;
        float frameChangeTime = 1f / fps;
        float currentTime = 0f;
        textureIndex = 0;
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

    public IEnumerator StartAnim(bool reverse = false)
    {
        float startTime = Time.realtimeSinceStartup;
        float frameChangeTime = 1f / fps;
        float currentTime = 0f;
        int direction = 1;

        if (reverse)
        {
            textureIndex = textureList.Count - 1;
            direction = -1;
        }
        else
        {
            textureIndex = 0;
        }

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
            textureIndex = textureIndex + direction;

            if (reverse)
            {
                if (textureIndex < 0)
                {
                    yield break;
                }
            }
            else
            {
                if (textureIndex >= textureList.Count)
                {
                    yield break;
                }
            }

            image.sprite = textureList[textureIndex];
        }
    }
}
