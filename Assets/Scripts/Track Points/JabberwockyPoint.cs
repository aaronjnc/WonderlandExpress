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
    public async override void StopAction()
    {
        if (GameManager.Instance.gold > GameManager.Instance.GetJabberwockyPrice())
        {
            await dm.DisplayDialog("jwSuccess");
        }
        else if (GameManager.Instance.GetPassengerCount() > 0) 
        {
            await dm.DisplayDialog("jwFailure");
            sheet.UpdatePassengers();
        }
        else
        {
            await dm.DisplayDialog("jwGameOver");
        }
    }
}
