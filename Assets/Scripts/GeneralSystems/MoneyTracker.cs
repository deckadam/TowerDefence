using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoneyTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayPanel;
    [SerializeField] private string header;


    public void Awake()
    {
        GameEvents.OnMoneyCountChanged += UpdateDisplay;
    }

    private void OnDestroy()
    {
        GameEvents.OnMoneyCountChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int amount)
    {
        Debug.LogError("updating text");
        displayPanel.text = header + amount;
    }
}