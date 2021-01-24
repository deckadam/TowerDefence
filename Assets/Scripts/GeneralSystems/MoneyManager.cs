using TMPro;
using UnityEngine;

public class MoneyManager : MonoSingleton<MoneyManager>
{
    [SerializeField] private TextMeshProUGUI displayPanel;
    [SerializeField] private string header;
    public int startingMoney;

    private int _currentMoney;

    //Submit to necessary events
    private void Awake()
    {
        GameEvents.OnTraverserDeath += AddMoney;
    }

    //Revoke submission from events
    private void OnDestroy()
    {
        GameEvents.OnTraverserDeath -= AddMoney;
    }

    //Set money to default value
    private void Start()
    {
        AddMoney(startingMoney);
    }

    //Adding money with the given amount
    private void AddMoney(int amount)
    {
        _currentMoney += amount;
        RaiseMoneyCountChangedEvent();
    }

    //Reducing money with the given amount
    public void ReduceMoney(int amount)
    {
        _currentMoney -= amount;
        RaiseMoneyCountChangedEvent();
    }

    //Raised when the currrent amount of money has changed 
    private void RaiseMoneyCountChangedEvent()
    {
        displayPanel.text = header + _currentMoney;
        GameEvents.OnMoneyAmountChanged?.Invoke(_currentMoney);
    }

    //Checks if the required amount of money is existing or not
    public bool IsAffordable(int amount) => amount <= _currentMoney;
}