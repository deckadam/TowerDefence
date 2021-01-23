using System;
using System.Collections;
using System.Net;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : Menu
{
    [SerializeField] private int countDownBeginingNumber;
    [SerializeField] private float countDownDelay;
    [SerializeField] private float lastFadeDuration;
    [SerializeField] private TextMeshProUGUI countDownText;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] private RectTransform bottomPanel;
    [SerializeField] private float bottomPanelAnimationDuration;

    [SerializeField] private Button randomTowerGenerationButton;

    private Vector2 _bottomPanelStartingPosition;

    //Subscribe to necessary events and cache the components
    protected override void Awake()
    {
        base.Awake();

        randomTowerGenerationButton.interactable = false;
        randomTowerGenerationButton.onClick.AddListener(() => GameEvents.OnGenerateTowerButtonPressed?.Invoke());
        
        _bottomPanelStartingPosition = bottomPanel.anchoredPosition;

        GameEvents.OnGameStarted += StartCountDown;
        GameEvents.OnGameFailed += HideInGameUI;
    }

    //Revoke submission to necessary events 
    private void OnDestroy()
    {
        GameEvents.OnGameStarted -= StartCountDown;
        GameEvents.OnGameFailed -= HideInGameUI;
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

        countDownText.DOFade(0f, lastFadeDuration).OnComplete(() =>
        {
            
            GameEvents.OnCountDownCompleted?.Invoke();
            ShowInGameUI();
        });
    }

    //Count down text animation
    private void DoAnimation(object i)
    {
        countDownText.text = i.ToString();
        countDownText.alpha = 0.5f;
        countDownText.DOFade(1f, 0.8f);
        countDownText.transform.localScale = Vector3.one * 0.5f;
        countDownText.transform.DOScale(1f, 0.8f);
    }
    private void ShowInGameUI()
    {
        bottomPanel.DOAnchorPos(Vector2.zero, bottomPanelAnimationDuration).OnComplete(() => randomTowerGenerationButton.interactable = true);
    }

    private void HideInGameUI()
    {
        randomTowerGenerationButton.interactable = false;
        bottomPanel.DOAnchorPos(_bottomPanelStartingPosition, bottomPanelAnimationDuration);
    }
}