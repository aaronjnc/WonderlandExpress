using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private TrainAudioManager train;
    //Resume game
    public void Resume()
    {
        Time.timeScale = 1;
        train?.ResumeSound();
        gameObject.SetActive(false);
    }
    //Return to main menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    //Exit the game
    public void Exit()
    {
        Application.Quit();
    }
    //load train scene

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
