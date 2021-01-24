using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI displayPanel;
    [SerializeField] private string header;

    private int _currentScore;

    private void Awake()
    {
        GameEvents.OnTraverserDeath += AddScore;
    }

    private void OnDestroy()
    {
        GameEvents.OnTraverserDeath -= AddScore;
    }

    private void AddScore(int moneyGained)
    {
        _currentScore++;
        displayPanel.text = header + _currentScore;
    }

    public int GetScore() => _currentScore;
}