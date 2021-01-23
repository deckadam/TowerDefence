using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private OpeningMenu openingMenu;
    [SerializeField] private InGameMenu inGameMenu;
    [SerializeField] private FailMenu failMenu;

    //Disable the start the game button until preparations are completed
    //Submit to road generation completed event
    private void Awake()
    {
        GameEvents.OnFinalDestinationReached += RaiseOnGameFailed;
    }

    //Reveke submission from road generation completed event
    private void OnDestroy()
    {
        GameEvents.OnFinalDestinationReached -= RaiseOnGameFailed;
    }

    //For testing purpose to see if the grid and road generation is working with different parameters
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //Hide the menus other than fail menu 
    //Show fail menu
    //Raise the game failed event
    private void RaiseOnGameFailed()
    {
        GameEvents.OnGameFailed?.Invoke();
        inGameMenu.Hide();
        openingMenu.Hide();
        failMenu.Show();
    }
}