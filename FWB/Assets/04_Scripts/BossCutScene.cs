using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class BossCutScene : MonoBehaviour
{
    public RectTransform flag;
    public GameObject attackEffect, flash, cutSceneBackground, bossImage;

    public Animator attackEffectAnimator;
    public Sprite heroSprite, villainSprite;
    public Vector2 flagStartPosition = new Vector2(-100, 0);
    public Vector2 flagEndPosition = new Vector2(100, 0);
    public float flagMoveDuration = 2.0f;
    public float effectDuration = 0.1f;
    public float flashDuration = 1.0f;

    public Action onEffectsComplete;

    void Start()
    {
        DeactivateAllEffects();
    }

    public void ActivateEffects()
    {
        cutSceneBackground.SetActive(true);
        flag.gameObject.SetActive(true);
        attackEffect.SetActive(true);
        flash.SetActive(true);
    }

    public void StartFlagAnimation(Action onComplete)
    {
        flag.gameObject.SetActive(true);
        flag.anchoredPosition = flagStartPosition;
        flag.DOAnchorPos(flagEndPosition, flagMoveDuration).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            flag.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void StartAttackEffect(Action onComplete)
    {
        attackEffect.SetActive(true);
        bossImage.SetActive(true);
        attackEffectAnimator.Play("BossEffect");

        StartCoroutine(AdjustAnimationSpeedAndComplete(effectDuration, onComplete));
    }

    private IEnumerator AdjustAnimationSpeedAndComplete(float targetDuration, Action onComplete)
    {
        yield return null;

        float currentClipLength = attackEffectAnimator.GetCurrentAnimatorStateInfo(0).length;
        attackEffectAnimator.speed = currentClipLength / targetDuration;

        yield return new WaitForSeconds(targetDuration + 0.6f);

        SpriteRenderer spriteRenderer = attackEffect.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0, effectDuration).OnComplete(() =>
            {
                attackEffect.SetActive(false);
                bossImage.SetActive(false);
                onComplete?.Invoke();
            });
        }
    }

    public void StartFlashEffect(Action onComplete)
    {
        bossImage.SetActive(true);
        Image flashImage = flash.GetComponent<Image>();
        flash.SetActive(true);
        flashImage.color = new Color(1, 1, 1, 1);
        flashImage.DOFade(0, flashDuration).OnComplete(() =>
        {
            flash.SetActive(false);
            bossImage.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private void DeactivateAllEffects()
    {
        cutSceneBackground.SetActive(false);
        flag.gameObject.SetActive(false);
        attackEffect.SetActive(false);
        flash.SetActive(false);
        bossImage.SetActive(false);
    }

    public void SetBossImage(bool isHero)
    {
        Image img = bossImage.GetComponent<Image>();
        img.sprite = isHero ? heroSprite : villainSprite;
    }
}
