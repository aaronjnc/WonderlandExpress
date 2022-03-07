using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Vector3 trainPosition;
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
