using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Application is quitting")]
    private static bool applicationIsQuitting = false;
    [Tooltip("Current position of the train")]
    private Vector3 trainPosition;
    [Tooltip("Current Game Manager")]
    private static GameManager _instance;
    [Tooltip("Name of current stop")]
    private string currentStop;
    [Tooltip("List of current Passengers")]
    public GameObject[] passengers;
    [Tooltip("Max number of passengers")]
    public int maxCap = 5;
    [Tooltip("Current amount of gold")]
    public int gold = 0;
    public bool load = false;
    public static GameManager Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        Application.quitting += () => applicationIsQuitting = true;
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
        _instance = this;
        if (passengers == null)
        {
            passengers = new GameObject[maxCap];
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
    public GameObject[] GetPassengers()
    {
        return passengers;
    }
}
