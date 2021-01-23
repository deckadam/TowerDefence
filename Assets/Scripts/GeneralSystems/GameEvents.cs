using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public static Action OnGameStarted;
    public static Action OnGameFailed;
    public static Action OnFinalDestinationReached;
    public static Action<Transform[,]> OnGridGenerationCompleted;
    public static Action<Transform[]> OnRoadGenerationCompleted;
    public static Action<List<Transform>> OnNeighboursGenerated;

    public static Action OnGenerateTowerButtonPressed;

    public static Action<int> OnMoneyCountChanged;

    public static Action OnCountDownCompleted;
}