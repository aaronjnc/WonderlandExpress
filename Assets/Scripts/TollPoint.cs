using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollPoint : TrackPoint
{
    //[Tooltip("Cost to pass through this toll")]
    //public int tollCost = 50;
    [Tooltip("the value the toll is multiplied by at every sucessful pass")]
    public float tollMod = 1.2f;
    [Tooltip("End game canvas obect"), SerializeField]
    private GameObject endGame;
    public override void StopAction()
    {
        Debug.Log("TollPoint activation");
        if (GameManager.Instance.CheckToll())
        {
            GameManager.Instance.IncreaseToll(tollMod);
        }
        else
        {
            Time.timeScale = 0;
            endGame.SetActive(true);
        }
    }
}
