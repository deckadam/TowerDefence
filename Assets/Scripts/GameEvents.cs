using System;
using UnityEngine;

public class GameEvents
{
    public static Action OnGameStarted;
    public static Action OnGameFailed;
    public static Action OnFinalDestinationReached;
    public static Action<Transform[,]> OnGridGenerationCompleted;
    public static Action<Transform[]> OnRoadGenerationCompleted;
}