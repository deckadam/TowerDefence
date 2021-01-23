using System;
using UnityEditor.Presets;
using UnityEngine;

public class MoneyManager : MonoSingleton<MoneyManager>
{
    public int startingMoney;

    private int _currentMoney;

    private void Awake()
    {
        AddMoney(startingMoney);
    }

    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        RaiseMoneyCountChangedEvent();
    }

    public bool IsAffordable(int amount)
    {
        return amount < _currentMoney;
    }

    public void ReduceMoney(int amount)
    {
        _currentMoney -= amount;
        RaiseMoneyCountChangedEvent();
    }

    private void RaiseMoneyCountChangedEvent()
    {
        GameEvents.OnMoneyCountChanged?.Invoke(_currentMoney);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) AddMoney(100);
        if (Input.GetKeyDown(KeyCode.D)) ReduceMoney(100);
    }
}