using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Current position of the train")]
    private Vector3 trainPosition;
    [Tooltip("Current Game Manager")]
    private static GameManager _instance;
    [Tooltip("Name of current stop")]
    private string currentStop;
    [Tooltip("List of current Passengers")]
    public List<GameObject> passengers;
    [Tooltip("Current amount of gold")]
    public int gold = 0;
    public bool load = false;
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
        DontDestroyOnLoad(this);
        _instance = this;
        if (passengers == null)
        {
            passengers = new List<GameObject>();
        }
    }
    public void setTrainPosition(Vector3 pos)
    {
        trainPosition = pos;
    }
    public Vector3 getTrainPosition()
    {
        return trainPosition;
    }
    public void SetCurrentStop(string stop)
    {
        currentStop = stop;
    }
    public string GetCurrentStop()
    {
        return currentStop;
    }
    public int GetGold()
    {
        return gold;
    }
    public void AddGold(int amt)
    {
        gold += amt;
    }
    public List<GameObject> GetPassengers()
    {
        return passengers;
    }
}
