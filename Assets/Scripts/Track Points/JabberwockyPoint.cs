using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabberwockyPoint : TrackPoint
{
    [Tooltip("End game canvas"), SerializeField]
    private GameObject endGame;
    [SerializeField]
    private PassengerSheet sheet;
    public override void StopAction()
    {
        if (GameManager.Instance.gold > GameManager.Instance.GetJabberwockyPrice())
        {
            GameManager.Instance.PayJabberwocky();
            Debug.Log("You survived");
        }
        else if (GameManager.Instance.GetPassengerCount() > 0) 
        {
            int random = Random.Range(0, GameManager.Instance.GetPassengerCount());
            GameManager.Instance.EatPassenger(random);
            sheet.UpdatePassengers();
        }
        else
        {
            Time.timeScale = 0;
            endGame.SetActive(true);
        }
    }
}
