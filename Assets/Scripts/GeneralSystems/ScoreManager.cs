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


    public void AddScore(int moneyGained, int scoreGained)
    {
        _currentScore += scoreGained;
        displayPanel.text = header + _currentScore;
    }

    public int GetScore()
    {
        return _currentScore;
    }
}