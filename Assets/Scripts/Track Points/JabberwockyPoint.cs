using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabberwockyPoint : TrackPoint
{
    [Tooltip("End game canvas"), SerializeField]
    private GameObject endGame;
    [SerializeField]
    private PassengerSheet sheet;
    public DialogueManager dm;
    public override bool StopAction()
    {
        if (GameManager.Instance.gold > GameManager.Instance.GetJabberwockyPrice())
        {
            dm.DisplayDialog("jwSuccess");
        }
        else if (GameManager.Instance.GetPassengerCount() > 0) 
        {
            dm.DisplayDialog("jwFailure");
            sheet.UpdatePassengers();
        }
        else
        {
            dm.DisplayDialog("jwGameOver");
        }
        return true;
    }
}
