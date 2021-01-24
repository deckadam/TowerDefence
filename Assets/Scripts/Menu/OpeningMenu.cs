using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningMenu : Menu
{
    [SerializeField] private Button startTheGameButton;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnPathGenerationCompleted += EnableStartTheGameButton;
        startTheGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        GameEvents.OnPathGenerationCompleted -= EnableStartTheGameButton;
    }

    private void EnableStartTheGameButton(List<Transform> param)
    {
        startTheGameButton.interactable = true;
    }

    //Triggered from ui event
    public void StartTheGame()
    {
        Hide(GameEvents.OnGameStarted);
    }
}