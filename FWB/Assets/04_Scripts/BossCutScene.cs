using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class BossCutScene : MonoBehaviour
{
    [System.Serializable]
    public class BossData
    {
        public Image initialImage;
        public Image finalImage;
        public GameObject specialEffect;
    }

    [Header("Boss Cut Scene Container")]
    public GameObject bossCutSceneContainer;
    public GameObject bunnyCutSceneObject;
    public GameObject puppetCutSceneObject;


    [Header("Boss Configurations")]
    public BossData puppetData = new BossData();
    public BossData bunnyData = new BossData();

    [Header("Flag Animation")]
    public RectTransform flag;
    public Vector2 flagStartPosition = new Vector2(-100, 0);
    public Vector2 flagEndPosition = new Vector2(100, 0);
    public float flagMoveDuration = 2.0f;

    [Header("Effect Timings")]
    public float effectDuration = 0.1f;
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float finalImageDelay = 0.5f;

    public Action OnCutSceneEnd;

    void Start()
    {
        SetActiveAll(false);
    }

    public void StartPuppetCutScene()
    {
        bossCutSceneContainer.SetActive(true);
        puppetCutSceneObject.SetActive(true);
        bunnyCutSceneObject.SetActive(false);

        StartCoroutine(PuppetCutSceneRoutine());
    }

    public void StartBunnyCutScene()
    {
        bossCutSceneContainer.SetActive(true);
        puppetCutSceneObject.SetActive(false);
        bunnyCutSceneObject.SetActive(true);

        if (bunnyData.specialEffect != null) bunnyData.specialEffect.SetActive(false);

        StartCoroutine(BunnyCutSceneRoutine());
    }

    private IEnumerator PuppetCutSceneRoutine()
    {
        yield return FadeInImage(puppetData.initialImage, fadeInDuration);
        yield return new WaitForSeconds(2);
        yield return MoveFlag();
        TriggerSpecialEffect(puppetData);
        yield return new WaitForSeconds(effectDuration * 2 + finalImageDelay);

        puppetData.initialImage.gameObject.SetActive(false);
        yield return FadeOutImage(puppetData.finalImage, fadeOutDuration);
        puppetData.finalImage.gameObject.SetActive(false);
        SetActiveAll(false);
        OnCutSceneEnd?.Invoke();
    }

    private IEnumerator BunnyCutSceneRoutine()
    {
        yield return FadeInImage(bunnyData.initialImage, fadeInDuration);
        yield return new WaitForSeconds(2);
        TriggerSpecialEffect(bunnyData);
        yield return new WaitForSeconds(effectDuration * 2 + finalImageDelay);

        bunnyData.initialImage.gameObject.SetActive(false);
        yield return FadeOutImage(bunnyData.finalImage, fadeOutDuration);
        bunnyData.finalImage.gameObject.SetActive(false);
        SetActiveAll(false);
        OnCutSceneEnd?.Invoke();
    }

    private IEnumerator FadeInImage(Image image, float duration)
    {
        image.gameObject.SetActive(true);
        image.color = new Color(1, 1, 1, 0);
        yield return image.DOFade(1, duration).WaitForCompletion();
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        Color startColor = image.color;
        Color targetColor = new Color(0, 0, 0, 0);

        Sequence fadeSequence = DOTween.Sequence()
            .Join(image.DOFade(0, duration))
            .Join(image.DOColor(Color.black, duration));

        yield return fadeSequence.WaitForCompletion();
        image.gameObject.SetActive(false);
    }


    private IEnumerator MoveFlag()
    {
        flag.gameObject.SetActive(true);
        flag.anchoredPosition = flagStartPosition;
        yield return flag.DOAnchorPos(flagEndPosition, flagMoveDuration).SetEase(Ease.OutExpo).WaitForCompletion();
        flag.gameObject.SetActive(false);
    }

    private void TriggerSpecialEffect(BossData bossData)
    {
        bossData.specialEffect.SetActive(true);
        Sequence effectSequence = DOTween.Sequence()
            .Append(bossData.specialEffect.GetComponent<Image>().DOFade(1, effectDuration))
            .AppendCallback(() =>
            {
                bossData.initialImage.gameObject.SetActive(false);
                bossData.finalImage.gameObject.SetActive(true);
            })
            .AppendInterval(effectDuration)
            .Append(bossData.specialEffect.GetComponent<Image>().DOFade(0, effectDuration))
            .OnComplete(() => bossData.specialEffect.SetActive(false));
        effectSequence.Play();
    }

    private void SetActiveAll(bool active)
    {
        bossCutSceneContainer.SetActive(active);
        puppetData.initialImage.gameObject.SetActive(active);
        puppetData.finalImage.gameObject.SetActive(active);
        bunnyData.initialImage.gameObject.SetActive(active);
        bunnyData.finalImage.gameObject.SetActive(active);
        flag.gameObject.SetActive(active);
        if (puppetData.specialEffect != null) puppetData.specialEffect.SetActive(active);
        if (bunnyData.specialEffect != null) bunnyData.specialEffect.SetActive(active);
    }
}
