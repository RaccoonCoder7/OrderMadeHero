using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TendencyChangeMgr : MonoBehaviour
{
    public GameObject tendencyChangeBlind;
    public List<Image> animateImageList = new List<Image>();
    public List<Image> fakeAnimateImageList = new List<Image>();
    public List<Image> changeImageList = new List<Image>();
    public List<Image> fakeChangeImageList = new List<Image>();
    public List<SpriteChange> scList = new List<SpriteChange>();
    public List<SpriteChange> fakeScList = new List<SpriteChange>();


    public IEnumerator TendencyChangeAnimation(bool toHero)
    {
        tendencyChangeBlind.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < animateImageList.Count; i++)
        {
            animateImageList[i].enabled = false;
            fakeAnimateImageList[i].enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < animateImageList.Count; i++)
        {
            animateImageList[i].enabled = true;
            fakeAnimateImageList[i].enabled = false;
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < animateImageList.Count; i++)
        {
            animateImageList[i].enabled = false;
            fakeAnimateImageList[i].enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < animateImageList.Count; i++)
        {
            animateImageList[i].enabled = true;
            fakeAnimateImageList[i].enabled = false;
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < animateImageList.Count; i++)
        {
            Sprite temp = animateImageList[i].sprite;
            animateImageList[i].sprite = fakeAnimateImageList[i].sprite;
            fakeAnimateImageList[i].sprite = temp;
        }

        for (int i = 0; i < animateImageList.Count; i++)
        {
            fakeAnimateImageList[i].enabled = true;
        }

        float fadeValue = 1;
        float actualSpeed = 0.02f;
        while (fadeValue > 0)
        {
            fadeValue -= actualSpeed;
            for (int i = 0; i < animateImageList.Count; i++)
            {
                fakeAnimateImageList[i].color = new UnityEngine.Color(1, 1, 1, fadeValue);
            }
            yield return new WaitForSeconds(actualSpeed);
        }

        for (int i = 0; i < animateImageList.Count; i++)
        {
            fakeAnimateImageList[i].color = new UnityEngine.Color(1, 1, 1, 1);
            fakeAnimateImageList[i].enabled = false;
        }

        for (int i = 0; i < scList.Count; i++)
        {
            Sprite temp = scList[i].onSprite;
            scList[i].onSprite = fakeScList[i].onSprite;
            fakeScList[i].onSprite = temp;
        }

        for (int i = 0; i < changeImageList.Count; i++)
        {
            Sprite temp = changeImageList[i].sprite;
            changeImageList[i].sprite = fakeChangeImageList[i].sprite;
            fakeChangeImageList[i].sprite = temp;
        }

        tendencyChangeBlind.SetActive(false);
    }

    [ContextMenu("ToHero")]
    private void ToHero()
    {
        StartCoroutine(TendencyChangeAnimation(true));
    }

    [ContextMenu("ToVillain")]
    private void ToVillain()
    {
        StartCoroutine(TendencyChangeAnimation(false));
    }
}
