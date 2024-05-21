using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverModeSampleSceneManager : MonoBehaviour
{
    public RectTransform gageRectTr;
    public float maxPosX;
    public float maxTime;

    private Image gageImage;

    private IEnumerator Start()
    {
        gageImage = gageRectTr.GetComponent<Image>();

        float gageWidth = gageRectTr.sizeDelta.x;
        float minPosX = maxPosX - Mathf.Abs(gageWidth);
        float currentPercent = 1;
        Vector2 currentPos = gageRectTr.anchoredPosition;
        currentPos.x = maxPosX;
        gageRectTr.anchoredPosition = currentPos;

        // TODO: 퍼즐 계속 나오게 만드는 IEnumerator 실행

        while (currentPos.x >= minPosX)
        {
            float percentAtThisFrame = Time.deltaTime / maxTime;
            currentPos.x -= percentAtThisFrame * gageWidth;
            currentPercent -= percentAtThisFrame;
            gageRectTr.anchoredPosition = currentPos;
            
            if (currentPercent > 0.5f)
            {
                gageImage.color = Color.green;
            }
            else if (currentPercent < 0.15f)
            {
                gageImage.color = Color.red;
            }
            else
            {
                gageImage.color = Color.yellow;
            }

            yield return null;
        }

        // TODO: 퍼즐 종료시키는 기능 실행
    }

    [ContextMenu("Test")]
    void Test()
    {
        Debug.Log(gageRectTr.anchoredPosition.x);
        Debug.Log(gageRectTr.anchoredPosition.y);
    }

    [ContextMenu("Test2")]
    void Test2()
    {
        float gageWidth = gageRectTr.sizeDelta.x;
        float minPosX = maxPosX - Mathf.Abs(gageWidth);
        Vector3 currentPos = gageRectTr.localPosition;
        currentPos.x = maxPosX;
        gageRectTr.localPosition = currentPos;
    }
}
