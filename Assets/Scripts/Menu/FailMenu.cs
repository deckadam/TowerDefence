using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailMenu : Menu
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button resetButton;

    [SerializeField] private string scoreHeader;
    [SerializeField] private float delayBeforeReset;

    //Overrided for setting the reset button not interactable
    //When showing operation is completed it is interactable again to prevent mid animation interaction
    protected override void Awake()
    {
        base.Awake();
        resetButton.interactable = false;
        OnShowStarted += () => scoreText.text = scoreHeader + ScoreManager.ins.GetScore();
        OnShowCompleted += () => resetButton.interactable = true;
    }

    //Triggered from ui event
    //Loads the scene again
    public void OnResetClicked()
    {
        StartCoroutine(DoDelayedReset());
    }

    //Giving a delay to reset after the button is clicked
    private IEnumerator DoDelayedReset()
    {
        yield return new WaitForSeconds(delayBeforeReset);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}