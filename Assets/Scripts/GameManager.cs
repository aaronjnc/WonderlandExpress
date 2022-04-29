using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [Tooltip("Target Frame Rate - DO NOT CHANGE WITHOUT MENTIONING FIRST")]
    [SerializeField]
    private int targetFR = 30;
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
    [Tooltip("Max number of passengers per car")]
    public int maxCap = 5;
    [Tooltip("Number of passenger cars")]
    public int numCar = 1;
    [Tooltip("Current number of passengers")]
    private int passengerCount = 0;
    [Tooltip("Current amount of gold")]
    public int gold = 0;
    [Tooltip("Rate at which passenger happiness decreases. measured in %/sec")]
    public float happinessDecayRate = 1f;
    [Tooltip("Next toll price")]
    public int tollPrice = 50;
    [Tooltip("the value the toll is multiplied by at every sucessful pass")]
    public float tollMod = 1.2f;
    [Tooltip("Jabberwocky relative price"), SerializeField]
    private float jabberwockyMod = 1.5f;
    [Tooltip("Jabberwocky price")]
    private int jabberwockyPrice;
    [Tooltip("The unique passenger info component")]
    private UniquePassengerInfo upi;
    [Tooltip("The scene is being opened from passenger scene")]
    public bool load = false;
    [Tooltip("List of positions for follow cars")]
    private List<Vector3> trainCarPos = new List<Vector3>();
    [Tooltip("List of rotations for follow cars")]
    private List<Vector3> trainCarRots = new List<Vector3>();
    [Tooltip("List of follow points for follow cars")]
    private List<FollowPoint> trainCarStops = new List<FollowPoint>();
    [Tooltip("Disable passenger scene loading")]
    public bool trainSceneTesting = false;
    public delegate Task OnTollChange(int currentToll, int newToll, int currentGold, int newGold, int jabberwocky);
    public event OnTollChange TollChangeEvent;
    [HideInInspector]
    public int carCount = 1;
    [HideInInspector]
    public int carLevel = 0;
    [Tooltip("Linked list of train points head")]
    private FollowPoint head;
    [Tooltip("Linked list of train poitns tail")]
    private FollowPoint tail;
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
        Application.targetFrameRate = targetFR;
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
        jabberwockyPrice = (int)(jabberwockyMod * tollPrice);
        upi = gameObject.GetComponent<UniquePassengerInfo>();
        if(upi == null)
        {
            Debug.LogError("UPI NOT FOUND");
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            foreach (GameObject pass in passengers)
            {
                if (pass != null)
                {
                    Debug.Log(happinessDecayRate / 100f / (float)targetFR);
                    pass.GetComponent<Passenger>().OnTrainMove(happinessDecayRate / 100f / (float)targetFR, passengerCount);
                }
            }
        }
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
    public int GetNumCar()
    {
        return carCount;
    }
    public void SetNumCar(int num)
    {
        carCount = num;
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

    public void ClearFollowTrains()
    {
        trainCarPos.Clear();
        trainCarRots.Clear();
        trainCarStops.Clear();
    }

    public void AddFollowTrain(FollowTrain follow)
    {
        trainCarPos.Add(follow.transform.position);
        trainCarRots.Add(follow.transform.eulerAngles);
        trainCarStops.Add(follow.GetNextPoint());
    }

    public void LoadFollowTrain(List<FollowTrain> followers)
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].transform.position = trainCarPos[i];
            followers[i].transform.eulerAngles = trainCarRots[i];
            followers[i].SetNextPoint(trainCarStops[i]);
        }
    }

    //checks the toll against the current gold, 
    //returns true if gold > toll
    //returns false otherwise
    public bool CheckToll()
    {
        Debug.Log("Checking toll");
        if(gold >= tollPrice)
        {
            Debug.Log("Toll good");
            
            return true;
        }
        Debug.Log("Toll bad");
        return false;
    }

    //pays the toll and increases its value
    public async Task TollPass()
    {
        int oldGold = gold;
        gold -= tollPrice;
        int oldToll = tollPrice;
        tollPrice = (int)(tollPrice * tollMod);
        jabberwockyPrice = (int)(tollPrice * jabberwockyMod);
        if (TollChangeEvent != null)
            await TollChangeEvent(oldToll, tollPrice, oldGold, gold, jabberwockyPrice);
    }

    //pays the toll, reducing gold by the amount given by the current toll
    public void PayToll()
    {
        int oldGold = gold;
        gold -= tollPrice;
        if (TollChangeEvent != null)
            TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
    }

    //multiplies toll by the given value
    public void IncreaseToll()
    {
        int oldToll = tollPrice;
        tollPrice = (int)(tollPrice * tollMod);
        jabberwockyPrice = (int)(tollPrice * jabberwockyMod);
        if (TollChangeEvent != null)
            TollChangeEvent(oldToll, tollPrice, gold, gold, jabberwockyPrice);
    }

    public int GetToll()
    {
        return tollPrice;
    }

    public int GetJabberwockyPrice()
    {
        return jabberwockyPrice;
    }

    public async Task PayJabberwocky()
    {
        int oldGold = gold;
        gold -= jabberwockyPrice;
        if (TollChangeEvent != null)
            await TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
    }

    public void RemovePassenger()
    {
        passengerCount -= 1;
    }

    public void AddPassenger()
    {
        passengerCount += 1;
    }

    public int GetPassengerCount()
    {
        return passengerCount;
    }

    public async Task EatPassenger(int i, GameObject start, GameObject jw)
    {
        RemovePassenger();
        Passenger pass = passengers[i].GetComponent<Passenger>();
        Debug.Log(pass.firstName + " " + pass.lastName + " was eaten :(");
        pass.gameObject.SetActive(true);
        pass.Display(start.transform.position);
        await pass.MoveTo(jw.transform.position, true);
        Destroy(passengers[i]);
        passengers[i] = null;
    }

    //methods to do with unique passengers
    public bool IsRegular()
    {
        return upi.RegularSpawned();
    }

    public bool CheckRegularTown()
    {
        return currentStop == upi.GetRegularTown();
    }

    public UniquePassengerInfo.UniquePass GetRegular()
    {
        return upi.GetRegular();
    }

    public void InitializeUPI(Passenger pass)
    {
        upi.InitializeUPI(pass, GetCurrentStop());
    }

    public void DropOffUPI(Passenger pass)
    {
        upi.DropOffUPI(pass, GetCurrentStop());
    }

    public void KickOffUPI(Passenger pass)
    {
        upi.KickOffUPI(pass, GetCurrentStop());
    }

    public void IgnoreUPI(Passenger pass)
    {
        upi.IgnoreUPI(pass, GetCurrentStop());
    }

    public void UpgradeCars(int cost)
    {
        carLevel++;
        happinessDecayRate *= .9f;
        gold -= cost;
    }
    public void BuyCar(int cost)
    {
        carCount++;
        GameObject[] newPass = new GameObject[carCount * 5];
        for(int i = 0;i < passengers.Length; i++)
        {
            newPass[i] = passengers[i];
        }
        passengers = newPass;
        gold -= cost;
    }
    public void AddFollowPoint(Vector3 pos)
    {
        if (tail == null)
        {
            head = new FollowPoint(pos, null);
            tail = head;
            return;
        }
        FollowPoint newTail = new FollowPoint(pos, null);
        tail.SetNext(newTail);
        tail = newTail;
    }
    public FollowPoint RemoveHead()
    {
        head = head.GetNext();
        return head;
    }
    public FollowPoint GetHeadPoint()
    {
        return head;
    }
}
