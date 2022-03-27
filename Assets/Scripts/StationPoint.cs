using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationPoint : TrackPoint
{
    public override void StopAction()
    {
        GameManager.Instance.setTrainPosition(TrainMovement.Instance.gameObject.transform.position);
        GameManager.Instance.SetCurrentStop(gameObject.name);
        SceneManager.LoadScene(2);
    }
}
