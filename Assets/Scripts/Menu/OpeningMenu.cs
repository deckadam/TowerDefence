using System;
using UnityEngine;
using UnityEngine.UI;

public class OpeningMenu : Menu
{
    [SerializeField] private Button startTheGameButton;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.OnRoadGenerationCompleted += EnableStartTheGameButton;
        startTheGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        GameEvents.OnRoadGenerationCompleted -= EnableStartTheGameButton;
    }

    private void EnableStartTheGameButton(Transform[] param)
    {
        startTheGameButton.interactable = true;
    }

    //Triggered from ui event
    public void StartTheGame()
    {
        Hide(GameEvents.OnGameStarted);
    }
}