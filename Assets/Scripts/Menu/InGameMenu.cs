using System.Collections;
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

    [SerializeField] private RectTransform bottomPanel;
    [SerializeField] private float bottomPanelAnimationDuration;

    [SerializeField] private Button randomTowerGenerationButton;
    [SerializeField] private TextMeshProUGUI towerCostText;
    [SerializeField] private string towerGenerateTextHeader;
    [SerializeField] private string onTowersMaxedText;

    private Vector2 _bottomPanelStartingPosition;
    private int _currentTowerCost;

    //Subscribe to necessary events and cache the components
    protected override void Awake()
    {
        base.Awake();

        randomTowerGenerationButton.interactable = false;
        randomTowerGenerationButton.onClick.AddListener(() => GameEvents.OnGenerateTowerButtonPressed?.Invoke());

        _bottomPanelStartingPosition = bottomPanel.anchoredPosition;

        GameEvents.OnGameStarted += StartCountDown;
        GameEvents.OnGameFailed += HideInGameUI;
        GameEvents.OnTowerLevelChanged += SetCurrentCost;
        GameEvents.OnMoneyAmountChanged += OnMoneyAmountChanged;
    }

    //Revoke submission to necessary events 
    private void OnDestroy()
    {
        GameEvents.OnGameStarted -= StartCountDown;
        GameEvents.OnGameFailed -= HideInGameUI;
        GameEvents.OnTowerLevelChanged -= SetCurrentCost;
        GameEvents.OnMoneyAmountChanged -= OnMoneyAmountChanged;
    }

    //Sets the current cost of the tower to create
    //-1 indicates towers are maxed out so the displayed text changed accordingly
    private void SetCurrentCost(int level, int cost)
    {
        if (level == -1)
        {
            towerCostText.text = onTowersMaxedText;
        }
        else
        {
            _currentTowerCost = cost;
            towerCostText.text = towerGenerateTextHeader + cost;
        }
    }

    //Sets interactable status according to tower to be created to be affordable or not
    private void OnMoneyAmountChanged(int amount)
    {
        var status = MoneyManager.ins.IsAffordable(_currentTowerCost);
        randomTowerGenerationButton.interactable = status;
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

    //Opening animation for bottom panel
    private void ShowInGameUI()
    {
        bottomPanel.DOAnchorPos(Vector2.zero, bottomPanelAnimationDuration).OnComplete(() => randomTowerGenerationButton.interactable = true);
    }

    //Closing animation for bottom panel
    private void HideInGameUI()
    {
        randomTowerGenerationButton.interactable = false;
        bottomPanel.DOAnchorPos(_bottomPanelStartingPosition, bottomPanelAnimationDuration);
    }
}