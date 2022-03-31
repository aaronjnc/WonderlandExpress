using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationPoint : TrackPoint
{
    public override void StopAction()
    {
        GameManager.Instance.SetTrainPosition(TrainMovement.Instance.gameObject.transform.position);
        GameManager.Instance.SetCurrentStop(gameObject.name);
        GameManager.Instance.SetTrainRotation(TrainMovement.Instance.gameObject.transform.eulerAngles);
        GameManager.Instance.load = true;
        TrainMovement.Instance.SaveFollowTrains();
        if (!GameManager.Instance.trainSceneTesting)
            SceneManager.LoadScene(2);
    }
}
