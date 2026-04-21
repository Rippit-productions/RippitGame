using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class LeaderboardItem : MonoBehaviour
{
    private Rect _initRect;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;
    [SerializeField] private Gradient TextGradient;
    [SerializeField] private TMP_Text Text;

    [SerializeField] private TMP_Text SkaterNameText;

    [SerializeField] private AnimationCurve _AlphaCurve;

    private CanvasGroup _CanvasGroup;

    public float GetNormalisedPosition()
    {
        float childCount = transform.parent.childCount;
        return transform.GetSiblingIndex() / childCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentRectTransform = GetComponentInParent<RectTransform>();
        _initRect = _rectTransform.rect;
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.color = TextGradient.Evaluate(GetNormalisedPosition());
        Text.text = $"{transform.GetSiblingIndex() + 1}.";
        _CanvasGroup.alpha = _AlphaCurve.Evaluate(GetNormalisedPosition());

        Rect newSize = _initRect;
        newSize.x = _initRect.width * _AlphaCurve.Evaluate(GetNormalisedPosition());
        newSize.y = _initRect.height * _AlphaCurve.Evaluate(GetNormalisedPosition());
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);


        var board = RaceGameMode.Instance.GetLeaderboard();
        if (transform.GetSiblingIndex() < board.Length)
        {
            SkaterNameText.text = board[transform.GetSiblingIndex()].SkaterComponent.gameObject.name.ToUpper();
        }
        else
        {
            _CanvasGroup.alpha = 0;
        }
    }
}
