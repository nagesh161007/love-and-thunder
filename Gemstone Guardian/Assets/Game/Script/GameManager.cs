using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameUI_Manager gameUI_Manager;
    public Character playerCharacter;
    private bool gameIsOver;
    // Update is called once per frame

    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver()
    {
        gameUI_Manager.ShowGameOverUI();
        Debug.Log("GameOver");
    }

    public void GameIsFinished()
    {
        gameUI_Manager.ShowGameIsFinishedUI();
        Debug.Log("Game Finished");
    }

    void Update()
    {
        if (gameIsOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            gameUI_Manager.TogglePauseUI();

        if (playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            gameIsOver = true;
            GameOver();
        }
    }


    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
