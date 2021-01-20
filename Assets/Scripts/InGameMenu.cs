using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InGameMenu : Menu
{
    //On count down completed event used to start the traversers
    public static event Action OnCountDownCompleted;

    [SerializeField] private int countDownBeginingNumber;
    [SerializeField] private float countDownDelay;
    [SerializeField] private float lastFadeDuration;

    private TextMeshProUGUI _countDownText;

    //Subscribe to necessary events and cache the components
    protected override void Awake()
    {
        base.Awake();
        _countDownText = GetComponentInChildren<TextMeshProUGUI>();
        GameManager.OnGameStarted += StartCountDown;
        OnShowCompleted += () => GameManager.ins.openingMenu.Hide();
    }

    //Revoke submission to necessary events 
    private void OnDestroy()
    {
        GameManager.OnGameStarted -= StartCountDown;
    }

    //Start the count down coroutine
    private void StartCountDown()
    {
        StartCoroutine(DoCountDown());
    }

    //Do the count down
    //When finished raise the event
    private IEnumerator DoCountDown()
    {
        var waiter = new WaitForSeconds(1f);

        yield return new WaitForSeconds(countDownDelay);

        for (var i = countDownBeginingNumber; i >= 0; i--)
        {
            DoAnimation(i);
            yield return waiter;
        }

        _countDownText.DOFade(0f, lastFadeDuration).OnComplete(RaiseOnCountDownCompleted);
    }

    //Count down text animation
    private void DoAnimation(object i)
    {
        _countDownText.text = i.ToString();
        _countDownText.alpha = 0.5f;
        _countDownText.DOFade(1f, 0.8f);
        _countDownText.transform.localScale = Vector3.one * 0.5f;
        _countDownText.transform.DOScale(1f, 0.8f);
    }

    private void RaiseOnCountDownCompleted()
    {
        OnCountDownCompleted?.Invoke();
    }
}