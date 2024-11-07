using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class BossCutScene : MonoBehaviour
{
    [Header("Images")]
    public Image initialImage;
    public Image finalImage;

    [Header("Flag Animation")]
    public RectTransform flag;
    public Vector2 flagStartPosition = new Vector2(-100, 0);
    public Vector2 flagEndPosition = new Vector2(100, 0);
    public float flagMoveDuration = 2.0f;

    [Header("Flash Animation")]
    public Image flashPanel;
    public float flashDuration = 0.1f;

    [Header("Fade Settings")]
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float finalImageDelay = 0.5f;

    public Action OnCutSceneEnd;
    void Start()
    {
        SetActiveAll(false);
        flashPanel.color = new Color(1, 1, 1, 0);
    }

    public void StartCutScene()
    {
        StartCoroutine(CutSceneRoutine());
    }

    private IEnumerator CutSceneRoutine()
    {
        yield return FadeInImage(initialImage, fadeInDuration);
        yield return MoveFlag();

        TriggerFlashPanel();
        yield return new WaitForSeconds(flashDuration * 2 + finalImageDelay);
        yield return FadeOutImage(initialImage, fadeOutDuration);

        finalImage.gameObject.SetActive(false);

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
        yield return image.DOFade(0, duration).WaitForCompletion();
        image.gameObject.SetActive(false);
    }

    private IEnumerator MoveFlag()
    {
        flag.gameObject.SetActive(true);
        flag.anchoredPosition = flagStartPosition;
        yield return flag.DOAnchorPos(flagEndPosition, flagMoveDuration).SetEase(Ease.OutExpo).WaitForCompletion();
        flag.gameObject.SetActive(false);
    }

    private void TriggerFlashPanel()
    {
        flashPanel.gameObject.SetActive(true);
        Sequence flashSequence = DOTween.Sequence()
            .Append(flashPanel.DOFade(1, flashDuration))
            .AppendCallback(() =>
            {
                initialImage.gameObject.SetActive(false);
                finalImage.gameObject.SetActive(true);
            })
            .AppendInterval(flashDuration)
            .Append(flashPanel.DOFade(0, flashDuration))
            .OnComplete(() => flashPanel.gameObject.SetActive(false));

        flashSequence.Play();
    }


    private void SetActiveAll(bool active)
    {
        initialImage.gameObject.SetActive(active);
        finalImage.gameObject.SetActive(active);
        flag.gameObject.SetActive(active);
        flashPanel.gameObject.SetActive(active);
    }
}
