using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollPoint : TrackPoint
{
    [Tooltip("Cost to pass through this toll")]
    public int tollCost;
    [Tooltip("End game canvas obect"), SerializeField]
    private GameObject endGame;
    public override void StopAction()
    {
        if (GameManager.Instance.GetGold() > tollCost)
        {
            GameManager.Instance.AddGold(-tollCost);
        }
        else
        {
            Time.timeScale = 0;
            endGame.SetActive(true);
        }
    }
}
