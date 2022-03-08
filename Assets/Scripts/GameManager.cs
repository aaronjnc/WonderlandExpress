using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Current position of the train")]
    private Vector3 trainPosition;
    [Tooltip("Current Game Manager")]
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Game Manager is null");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    public void setTrainPosition(Vector3 pos)
    {
        trainPosition = pos;
    }
    public Vector3 getTrainPosition()
    {
        return trainPosition;
    }
}
