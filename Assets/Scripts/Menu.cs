using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private TrainAudioManager train;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private Toggle mouthNoisesToggle;

    private void Start()
    {
        if (mouthNoisesToggle != null)
            mouthNoisesToggle.isOn = GameManager.Instance.mouthNoises;
    }
    //Resume game
    public void Resume()
    {
        Time.timeScale = 1;
        train?.ResumeSound();
        pauseMenu.SetActive(false);
    }
    //Return to main menu
    public void MainMenu()
    {
        GameManager.Instance.SaveGame();
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
