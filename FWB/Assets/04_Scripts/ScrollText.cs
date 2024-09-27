using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScrollText : MonoBehaviour
{
    [Header("이동할 오브젝트")]
    [SerializeField]
    private Text text = null;

    [Header("이동 속도")]
    [SerializeField]
    private float moveSpeed = 3f;

    [Header("이동 방향( -> )")]
    [SerializeField]
    private Direction directionType = Direction.Left;

    [Header("Mask 영역")]
    [SerializeField]
    private RectTransform maskBackground = default;

    [SerializeField]
    private float spaceX = 100f;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 direction = Vector2.right;
    private Vector2 originPos;
    private Vector2 originSize;
    private Coroutine routine;

    public enum Direction
    {
        Left,
        Right,
    }

    private RectTransform _textRectTransform = null;
    private RectTransform textRectTransform
    {
        get
        {
            if (_textRectTransform == null)
                _textRectTransform = text.GetComponent<RectTransform>();

            return _textRectTransform;
        }
    }

    private void Start()
    {
        if (originPos.Equals(Vector2.zero))
        {
            originPos = textRectTransform.anchoredPosition;
            originSize = textRectTransform.sizeDelta;
        }
    }

    public void OnTextChanged()
    {
        Start();

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (!IsTextOverflowed())
        {
            textRectTransform.anchoredPosition = originPos;
            textRectTransform.sizeDelta = originSize;
            return;
        }

        float preferredWidth = LayoutUtility.GetPreferredWidth(text.rectTransform);
        textRectTransform.sizeDelta = new Vector2(preferredWidth / 2, text.rectTransform.rect.height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);

        var posX = preferredWidth;
        posX += spaceX;

        switch (directionType)
        {
            case Direction.Left:
                startPosition = new Vector2(posX, 0);
                direction = Vector2.left;
                break;
            case Direction.Right:
                startPosition = new Vector2(posX * -1, 0);
                direction = Vector2.right;
                break;
            default:
                return;
        }
        endPosition = new Vector2((startPosition.x - (spaceX * 2)) * -1, startPosition.y);

        textRectTransform.anchoredPosition = startPosition;
        routine = StartCoroutine(CoroutineMoveAction());
    }

    private IEnumerator CoroutineMoveAction()
    {
        while (true)
        {
            textRectTransform.Translate(direction * moveSpeed * Time.deltaTime);

            if (IsPositionEnd())
            {
                textRectTransform.anchoredPosition = startPosition;
            }

            yield return null;
        }
    }

    private bool IsPositionEnd()
    {
        switch (directionType)
        {
            case Direction.Left:
                return endPosition.x > textRectTransform.anchoredPosition.x;
            case Direction.Right:
                return endPosition.x < textRectTransform.anchoredPosition.x;
            default:
                return false;
        }
    }

    private bool IsTextOverflowed()
    {
        float preferredWidth = LayoutUtility.GetPreferredWidth(text.rectTransform);
        float parentWidth = maskBackground.rect.width;
        return preferredWidth > parentWidth;
    }
}