using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationPoint : TrackPoint
{
    [Tooltip("Text object for town name"), SerializeField]
    private TextMeshPro text;
    private void Awake()
    {
        base.Awake();
        text.text = gameObject.name;
    }

    public string GetTownName()
    {
        return text.text;
    }

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
