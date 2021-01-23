using System;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;

public class MoneyManager : MonoSingleton<MoneyManager>
{
    [SerializeField] private TextMeshProUGUI displayPanel;
    [SerializeField] private string header;
    public int startingMoney;

    private int _currentMoney;

    private void Awake()
    {
        GameEvents.OnTraverserDeath += AddMoney;
    }

    private void OnDestroy()
    {
        GameEvents.OnTraverserDeath -= AddMoney;
    }

    private void Start()
    {
        AddMoney(startingMoney, 0);
    }

    private void AddMoney(int moneyGained, int scoreGained)
    {
        _currentMoney += moneyGained;
        RaiseMoneyCountChangedEvent();
    }


    public void ReduceMoney(int amount)
    {
        _currentMoney -= amount;
        RaiseMoneyCountChangedEvent();
    }

    private void RaiseMoneyCountChangedEvent()
    {
        displayPanel.text = header + _currentMoney;
    }
    
    public bool IsAffordable(int amount) => amount <= _currentMoney;
}