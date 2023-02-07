using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverCanvas : MonoBehaviour
{
    [Tooltip("The Text space used for showing current gold")]
    public TextMeshProUGUI goldText;
    [Tooltip("The Text space used for showing total gold")]
    public TextMeshProUGUI totalGoldText;
    [Tooltip("The Text space used for showing number of loops")]
    public TextMeshProUGUI loopsText;

    /*
     * sets up the game over canvas with the current data
     */
    public void Setup()
    {
        goldText.text = GameManager.Instance.GetGold() + "g";
        totalGoldText.text = GameManager.Instance.GetTotalGold() + "g";
        int loops = GameManager.Instance.GetLoopCount();
        if (loops == 1)
        {
            loopsText.text = loops + " loop";
        }
        else {
            loopsText.text = loops + " loops";
        }
    }
}
