using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownUIManager : MonoBehaviour
{
    [Header("UI pieces")]

    [Tooltip("The Text space used for showing the passenger messages")]
    public Text passMessageText;

    [Tooltip("The Text space used for showing 'error' messages (train is full, must remove passenger, etc.)")]
    public Text errorText;

    [Tooltip("All of the remove buttons used for the passengers. Must be listed in order from right to left")]
    public List<GameObject> removeButtons;

    public void SetupButtons(int numPass)
    {
        int currentNum = 0;
        foreach(GameObject button in removeButtons)
        {
            button.SetActive(currentNum < numPass);
            currentNum++;
        }

    }

    public void DisplayText(string text)
    {
        passMessageText.text = text;
        errorText.text = "";
    }

    public void DisplayError(string text)
    {
        errorText.text = text;
    }


    //REMOVE FROM THIS OBJECT. PUT SOMEWHERE ELSE
    public void SwitchScene()
    {
        GameManager.Instance.load = true;
        SceneManager.LoadScene("ILikeTrains");
    }
}
