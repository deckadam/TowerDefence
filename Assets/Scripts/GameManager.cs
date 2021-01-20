using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public static event Action OnGameStarted;
    public static event Action OnGameFailed;

    [SerializeField] private Button startTheGameButton;

    public InGameMenu inGamePanel;
    public FailMenu failMenu;
    public OpeningMenu openingMenu;

    //Disable the start the game button until preparations are completed
    //Submit to road generation completed event
    private void Awake()
    {
        startTheGameButton.interactable = false;
        RoadGenerator.OnRoadGenerationCompleted += OnPreparationCompleted;
        Traverser.OnFinalDestinationReached += RaiseOnGameFailed;
    }

    //Reveke submission from road generation completed event
    private void OnDestroy()
    {
        RoadGenerator.OnRoadGenerationCompleted -= OnPreparationCompleted;
        Traverser.OnFinalDestinationReached -= RaiseOnGameFailed;
    }

    //Set the start the game button interactable to make the game playable
    private void OnPreparationCompleted(Transform[] obj)
    {
        startTheGameButton.interactable = true;
    }

    //For testing purpose to see if the grid and road generation is working with different parameters
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //Event raiser for game started
    public void RaiseOnGameStarted()
    {
        openingMenu.Hide(OnGameStarted);
    }

    //Hide the menus other than fail menu 
    //Show fail menu
    //Raise the game failed event
    private void RaiseOnGameFailed()
    {
        OnGameFailed?.Invoke();
        inGamePanel.Hide();
        openingMenu.Hide();
        failMenu.Show();
    }
}