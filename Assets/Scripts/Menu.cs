using System;
using DG.Tweening;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public float panelTransitionDuration;
    public float panelTransitionDelay;

    protected Action OnHideCompleted;
    protected Action OnShowCompleted;

    private RectTransform _rectTransform;

    //Caches the rect transform
    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    //Show implementation of IMenu
    public void Show(Action onComplete = null)
    {
        _rectTransform.DOAnchorPos(Vector2.zero, panelTransitionDuration)
            .SetDelay(panelTransitionDelay)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                OnShowCompleted?.Invoke();
            });
    }

    //Hide implementation of IMenu
    public void Hide(Action onComplete = null)
    {
        _rectTransform.DOAnchorPos(Vector2.left * 2500f, panelTransitionDuration)
            .SetDelay(panelTransitionDelay)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                OnHideCompleted?.Invoke();
            });
    }
}