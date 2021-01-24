using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    //Raised when game is started
    public static Action OnGameStarted;

    //Raised when game has been failed
    public static Action OnGameFailed;

    //Raised when a traverser has reached the final tile of the raod which leads to game fail condition
    public static Action OnFinalDestinationReached;

    //Raised when grid generator has finished calculating grid
    public static Action<Transform[,]> OnGridGenerationCompleted;

    //Raised when road generator has finished calculating road
    public static Action<List<Transform>> OnPathGenerationCompleted;

    //Raised when road generator has finished calculating neighbours
    public static Action<List<Transform>> OnNeighboursGenerated;

    //Raised when a request to generating tower is made
    public static Action OnGenerateTowerButtonPressed;

    //In game menu count down completion event for starting the traverser generation and show operation of some menus
    public static Action OnCountDownCompleted;

    //Raised when a traverser is killed
    public static Action<int> OnTraverserDeath;

    //First parameter new tower level second one is the cost
    public static Action<int, int> OnTowerLevelChanged;

    //Raised when current money amount is changed
    public static Action<int> OnMoneyAmountChanged;
}