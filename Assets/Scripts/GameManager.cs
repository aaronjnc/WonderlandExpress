using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("Application is quitting")]
    private static bool applicationIsQuitting = false;
    [Tooltip("Current position of the train")]
    private Vector3 trainPosition;
    [Tooltip("Rotation of the train")]
    private Vector3 trainRotation;
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
        if (Instance != this)
        {
            Destroy(this.gameObject);
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        Application.quitting += () => applicationIsQuitting = true;
        SceneManager.sceneLoaded += OnSceneLoad;
        passengers = new GameObject[maxCap];
    }
    public void SetTrainPosition(Vector3 pos)
    {
        trainPosition = pos;
    }
    public Vector3 GetTrainPosition()
    {
        return trainPosition;
    }
    public void SetTrainRotation(Vector3 rot)
    {
        trainRotation = rot;
    }
    public Vector3 GetTrainRotation()
    {
        return trainRotation;
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
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
