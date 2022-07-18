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
    [Tooltip("Total amount of gold gained throughout the run")]
    public int totalGold = 0;
    [Tooltip("Number of loops completed throughout the run")]
    public int loops = 0;
    [Tooltip("Rate at which passenger happiness decreases. measured in %/sec")]
    public float happinessDecayRate = 1f;
    [Tooltip("modifier to apply to happinessDecayRate when jabberwock eats a passenger")]
    public float happinessDecayEatMod = 50f;
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
    public bool mouthNoises = false;
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
        //sets the target framerate to avoid frame-dependent issues
        Application.targetFrameRate = targetFR;
        //destroy any duplicate gamemanagers
        if (Instance != this)
        {
            Destroy(this.gameObject);
            Destroy(this);
            return;
        }
        //if this is the only instance, set it as the instance and initialize this gamemanager to last through transitions
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
        //if the current scene is the train scene, decrease all passengers' happiness every frame
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            foreach (GameObject pass in passengers)
            {
                if (pass != null)
                {
                    //Debug.Log(happinessDecayRate / 100f / (float)targetFR);
                    pass.GetComponent<Passenger>().OnTrainMove(happinessDecayRate / 100f / (float)targetFR, passengerCount);
                }
            }
        }
    }

    /*
     * sets the train's current position
     */
    public void SetTrainPosition(Vector3 pos)
    {
        trainPosition = pos;
    }

    /*
     * gets the train's current position
     */
    public Vector3 GetTrainPosition()
    {
        return trainPosition;
    }

    /*
     * sets the train's rotation
     */
    public void SetTrainRotation(Vector3 rot)
    {
        trainRotation = rot;
    }

    /*
     * gets the train's current rotation
     */
    public Vector3 GetTrainRotation()
    {
        return trainRotation;
    }

    /*
     * sets the passerby's current stop
     */
    public void SetCurrentStop(string stop)
    {
        currentStop = stop;
    }

    /*
     * gets the name of the current stop
     */
    public string GetCurrentStop()
    {
        return currentStop;
    }

    /*
     * gets the current gold
     */
    public int GetGold()
    {
        return gold;
    }

    /*
     * gets the total gold gained this run
     */
    public int GetTotalGold()
    {
        return totalGold;
    }

    /*
     * gets the number of loops completed
     */
    public int GetLoopCount()
    {
        return loops;
    }

    /*
     * gets the current number of passenger cars
     */
    public int GetNumCar()
    {
        return carCount;
    }

    /*
     * sets the number of passenger cars
     */
    public void SetNumCar(int num)
    {
        carCount = num;
    }

    /*
     * increase the amount of gold you have, as well as the total gold gained
     */
    public void AddGold(int amt)
    {
        gold += amt;
        totalGold += amt;
    }

    /*
     * returns an array of all current passengers
     */
    public GameObject[] GetPassengers()
    {
        return passengers;
    }

    /*
     * runs when a new scene loads to prevent duplicate gameManagers
     */
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 && this != null)
        {
            Destroy(this.gameObject);
        }
    }

    /*
     * clears all of the passenger cars
     */
    public void ClearFollowTrains()
    {
        trainCarPos.Clear();
        trainCarRots.Clear();
        trainCarStops.Clear();
    }

    /*
     * adds a passenger car to follow the train
     */
    public void AddFollowTrain(FollowTrain follow)
    {
        trainCarPos.Add(follow.transform.position);
        trainCarRots.Add(follow.transform.eulerAngles);
        trainCarStops.Add(follow.GetNextPoint());
    }

    /*
     * loads a following passenger car
     */
    public void LoadFollowTrain(List<FollowTrain> followers)
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].transform.position = trainCarPos[i];
            followers[i].transform.eulerAngles = trainCarRots[i];
            followers[i].SetNextPoint(trainCarStops[i]);
        }
    }

    /*
     * checks the toll against the current gold, 
     * returns true if gold > toll
     * returns false otherwise
     */
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

    /*
     * pays the toll and increases its cost
     */
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

    /*
     * pays the wonderland toll, increasing the current loop total
     */
    public async Task TollPassWL()
    {
        await TollPass();
        NewLoop();

    }

    /*
     * pays the toll, reducing gold by the current toll price
     */
    public void PayToll()
    {
        int oldGold = gold;
        gold -= tollPrice;
        if (TollChangeEvent != null)
            TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
    }

    /*
     * Increases toll price by the toll modifer value
     */
    public void IncreaseToll()
    {
        int oldToll = tollPrice;
        tollPrice = (int)(tollPrice * tollMod);
        jabberwockyPrice = (int)(tollPrice * jabberwockyMod);
        if (TollChangeEvent != null)
            TollChangeEvent(oldToll, tollPrice, gold, gold, jabberwockyPrice);
    }

    /*
     * gets the current price of the toll
     */
    public int GetToll()
    {
        return tollPrice;
    }

    /*
     * gets the current price of the jabberwocky toll
     */
    public int GetJabberwockyPrice()
    {
        return jabberwockyPrice;
    }

    /*
     * Pay the jabberwocky's toll.
     * decreases gold by the toll's cost and plays an animation
     */
    public async Task PayJabberwocky()
    {
        int oldGold = gold;
        gold -= jabberwockyPrice;
        if (TollChangeEvent != null)
            await TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
    }

    /*
     * decrement the passenger count by 1
     */
    public void RemovePassenger()
    {
        passengerCount -= 1;
    }

    /*
     * increment the passenger count by 1
     */
    public void AddPassenger()
    {
        passengerCount += 1;
    }

    /*
     * return the current number fo passengers on the passerby
     */
    public int GetPassengerCount()
    {
        return passengerCount;
    }

    /*
     * Run when the jabberwocky eats a passenger.
     * Removes that passenger from the train, and lowers the happiness of all other passengers
     */
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
        //DECREASE ALL PASSENGER HAPPINESS WHEN ONE IS EATEN. NOT YET TESTED
        //foreach(GameObject passenger in passengers)
        //{
        //    if(pass != null)
        //    {
        //        passengers[i].GetComponent<Passenger>().DecreaseHappiness(happinessDecayRate * happinessDecayEatMod / 100f);
        //    }
        //}
    }

    //methods to do with unique passengers

    /*
     * checks if the unique 'regular' passenger has spawned yet
     */
    public bool IsRegular()
    {
        return upi.RegularSpawned();
    }

    /*
     * checks if the current town was is the recorded location for the unique 'regular' passenger
     */
    public bool CheckRegularTown()
    {
        return currentStop == upi.GetRegularTown();
    }

    /*
     * gets the unique 'regular' passenger
     */
    public UniquePassengerInfo.UniquePass GetRegular()
    {
        return upi.GetRegular();
    }

    /*
     * Run when you first encounter a unique passenger. 
     * Initializes them to the current town
     */
    public void InitializeUPI(Passenger pass)
    {
        upi.InitializeUPI(pass, GetCurrentStop());
    }

    /*
     * run when you successfully drop off a unique passenger
     */
    public void DropOffUPI(Passenger pass)
    {
        upi.DropOffUPI(pass, GetCurrentStop());
    }

    /*
     * run when you kick a unique passenger off of the Passerby
     */
    public void KickOffUPI(Passenger pass)
    {
        upi.KickOffUPI(pass, GetCurrentStop());
    }

    /*
     * run when you ignore a unique passenger
     */
    public void IgnoreUPI(Passenger pass)
    {
        upi.IgnoreUPI(pass, GetCurrentStop());
    }

    /*
     * upgrade the train cars. 
     * Decreases the rate of happiness loss on the train scene and decreases gold by amount paid
     */
    public void UpgradeCars(int cost)
    {
        carLevel++;
        happinessDecayRate *= .9f;
        int oldGold = gold;
        gold -= cost;
        if (TollChangeEvent != null)
            TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
    }
    /*
     * Purchase a new train car. 
     *Increases number of passengers that can board and decreases gold by the amount paid
     */
    public void BuyCar(int cost)
    {
        carCount++;
        GameObject[] newPass = new GameObject[carCount * 5];
        for(int i = 0;i < passengers.Length; i++)
        {
            newPass[i] = passengers[i];
        }
        passengers = newPass;
        int oldGold = gold;
        gold -= cost;
        if (TollChangeEvent != null)
            TollChangeEvent(tollPrice, tollPrice, oldGold, gold, jabberwockyPrice);
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

    /*
     * Increment the number of loops completed to mark the start of a new loop
     */
    public void NewLoop()
    {
        loops += 1;
    }

    public void SetMouthNoises(bool UseMouthNoises)
    {
        mouthNoises = UseMouthNoises;
        if (TrainAudioManager.Instance != null)
        {
            if (UseMouthNoises)
                TrainAudioManager.Instance.SwitchSound(1);
            else
                TrainAudioManager.Instance.SwitchSound(1);
        }
    }
}
