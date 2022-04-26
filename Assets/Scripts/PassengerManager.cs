using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PassengerManager : MonoBehaviour
{
    [Header("Manager stuff")]

    [Tooltip("UI manager for town passenger setting")]
    public TownUIManager uiMan;

    [Tooltip("The town dictionary. used for finding current town")]
    private TownDictionary townDict;

    [Tooltip("The passenger generator. used for generating passenger names and lines")]
    public PassengerGenerator passGen;

    [Tooltip("The audio manager. Used for playing audio")]
    public TownAudioManager audioMan;

    [Header("Passengers")]

    [Tooltip("The prefab used to create passengers")]
    public GameObject passPrefab;

    [Tooltip("The empty GameObject which shows where the first accepted passenger should spawn on the screen")]
    public GameObject passLoc;

    [Tooltip("The offset used to determine where other accepted passengers will spawn on screen")]
    public Vector3 passOffset = new Vector3(-1f, 0f, 0f);

    [Tooltip("The empty GameObejct which shows where the first waiting passenger should spawn on the screen")]
    public GameObject waitingLoc;

    [Tooltip("The offset used to determine where other waiting passengers will spawn on screen")]
    public Vector3 waitingOffset = new Vector3(-1f, -.5f, 0f);

    [Tooltip("The empty GameObject which shows where passengers should go when getting on the train")]
    public GameObject OnTrainLoc;

    [Tooltip("The empty GameObject which shows where passengers should go when leaving the platform")]
    public GameObject OffPlatformLoc;

    [Tooltip("The empty GameObject which shows where passengers should go when being kicked off of the train")]
    public GameObject RemovedLoc;

    [Tooltip("Passengers currently on the train")]
    public GameObject[] currentPass;

    [Tooltip("maximum number of passengers on the train")]
    public int trainCap = 5;

    [Tooltip("the current number of passengers on the train")]
    public int currentPassNum;

    [Tooltip("Passengers waiting to get on the train")]
    public List<GameObject> waitingPass;

    [Tooltip("The maximum number of passengers that can try to board at each town")]
    public int maxPassPerTown = 5;

    [Header("General Passenger Generation Stats")]

    [Tooltip("The maximum percentage deviation from the generated wealth value a passenger can have between 0 and .5. \neg. a passenger can be x% lower or x% higher than the general value for a town")]
    [Range(0f, .5f)]
    public float wealthDev = 0.3f;

    [Tooltip("The default happiness of a passenger from 0 to 1")]
    [Range(0f, 1f)]
    public float startHappiness = 0.7f;

    [Tooltip("The maximum percentage deviation from the generated happiness value a passenger can have between 0 and .5. \neg. a passenger can be x% lower or x% higher than the general start value")]
    [Range(0f, .5f)]
    public float happinessDev = 0.1f;

    [Header("modifiers for calculations")]

    [Tooltip("modifier for converting between town and passenger wealth. \nPassenger->Town: divide by modifer \nTown->Passenger: multiply by modifier ")]
    [Min(0f)]
    public float wealthMod = 0.2f;

    [Tooltip("modifier for converting between town rep and passenger happiness. \nPassenger->Town: divide by modifer \nTown->Passenger: multiply by modifier ")]
    [Min(0f)]
    public float repHapMod = 0.2f;

    [Tooltip("modifier for converting between town reputation and number of passengers \n rep * mod = num")]
    public float passRepMod = 0.05f;

    [Tooltip("maximum tip percentage")]
    public float maxTip = .25f;

    [Tooltip("the happiness percentage required to earn a tip")]
    public float tipThreshold = .6f;

    [Tooltip("Timing variables")]
    public float leaveDelay = 1f;

    [Header("Test variables")]
    public Town town;

    public Town dest;

    public GameObject testTownDictionary;

    public bool useTestDict = false;

    // Start is called before the first frame update
    void Start()
    {
        //currentPass = new List<GameObject>();
        if(GameManager.Instance != null)
        {
            currentPass = GameManager.Instance.GetPassengers();
            
        }
        if(GameManager.Instance.GetCurrentStop() == null)
        {
            GameManager.Instance.SetCurrentStop(town.GetName());
        }
        if (useTestDict)
        {
            testTownDictionary.SetActive(true);
            townDict = testTownDictionary.GetComponent<TownDictionary>();
        }
        else
        {
            townDict = TownDictionary.Instance;
        }
        townDict.FindCurrentTown(GameManager.Instance.GetCurrentStop());
        FindCurrentPassNum();
        waitingPass = new List<GameObject>();
        SetupTown();
    }

    List<GameObject> getWaitingPass()
    {
        return waitingPass;
    }

    //gets the currently speaking passenger
    GameObject GetCurrentWaitingPass()
    {
        //Debug.Log("array size: " + waitingPass.Count + ", " + waitingPass.ToString());
        if (waitingPass.Count <= 0)
        {
            return null;
        }
        return waitingPass[0];
    }

    //get rid of the current town's display
    void HideTown()
    {
        foreach(GameObject pass in currentPass)
        {
            if (pass != null)
            {
                pass.GetComponent<Passenger>().Hide();
            }
        }
        foreach(GameObject pass in waitingPass)
        {
            pass.GetComponent<Passenger>().Hide();
        }
    }

    //Sets up a new town's display
    async void SetupTown()
    {
        SetupTownUI();
        passGen.Initialize(currentPass);
        PassActive();
        GeneratePassengers();
        StartBGAudio();
        DisplayAllPass();
        await CheckPassengerDestination();
        //Debug.Log("list length: " + waitingPass.Count);
        await MoveWaitingPass();
        //Debug.Log("list length: " + waitingPass.Count);
        NewPassenger();
        //Debug.Log("list length: " + waitingPass.Count);
    }

    //sets up the town ui
    void SetupTownUI()
    {
        uiMan.DisplayTown(GetTown());
        uiMan.DisplayGold(GameManager.Instance.GetGold());
        uiMan.DisplayToll(GameManager.Instance.GetToll());
    }

    //starts playing bg audio
    void StartBGAudio()
    {
        if (GetTown().IsDestroyed())
        {
            audioMan.PlayBGAudio("destroyed");
        }
        else
        {
            audioMan.PlayBGAudio(waitingPass.Count - 1);
        }
       
    }

    //What to do when leaving the town
    public void LeaveTown()
    {
        audioMan.SelectAudio();
        Invoke("PassInactive",leaveDelay);
        uiMan.Invoke("SwitchScene",leaveDelay);
        
    }

    //Leaving town but fr this time
    //public void LeaveDelayed()
    //{
    //    uiMan.SwitchScene();
    //}

    //checks all current passengers and removes all passengers who have reached their destination
    async Task CheckPassengerDestination()
    {
        List<GameObject> removed = new List<GameObject>();
        Town town = GetTown();
        foreach (GameObject pass in currentPass)
        {
            if (pass != null)
            {
                string destination = pass.GetComponent<Passenger>().GetDestination();
                if (destination == town.GetName())
                {
                    removed.Add(pass);
                }
            }
        }
        foreach(GameObject pass in removed)
        {

            await DropOffSuccess(pass);
        }
    }

    //sets all current passengers to active
    void PassActive()
    {
        foreach (GameObject pass in currentPass)
        {
            if (pass != null)
            {
                pass.SetActive(true);
            }
        }
    }

    //set all current passengers to inactive
    public void PassInactive()
    {
        foreach (GameObject pass in currentPass)
        {
            if (pass != null)
            {
                pass.SetActive(false);
            }
        }
    }

    //displays all waiting and boarded passengers
    void DisplayAllPass()
    {
        DisplayPass();
        DisplayWaitingPass();
        
    }

    //displays all currently boarded passengers
    void DisplayPass()
    {
        //foreach (GameObject pass in currentPass)
        //{
        //    pass.GetComponent<Passenger>().Hide();
        //}
        Vector3 displayPos = passLoc.transform.position;
        foreach(GameObject pass in currentPass)
        {
            if (pass != null)
            {
                pass.GetComponent<Passenger>().Display(displayPos);
            }
            displayPos += passOffset;
        }
        uiMan.SetupButtons(currentPass);
    }

    //displays all currently waiting passengers
    void DisplayWaitingPass()
    {
        //foreach (GameObject pass in waitingPass)
        //{
        //    pass.GetComponent<Passenger>().Hide();
        //}
        Vector3 displayPos = waitingLoc.transform.position;
        foreach(GameObject pass in waitingPass)
        {
            displayPos += waitingOffset;
            pass.GetComponent<Passenger>().Display(displayPos);
            
        }
    }

    //moves current waiting passengers to their new positions
    async Task MoveWaitingPass()
    {
        uiMan.CanInteract(false);
        Vector3 displayPos = waitingLoc.transform.position;
        var t = new Task[waitingPass.Count];
        int counter = 0;
        foreach (GameObject pass in waitingPass)
        {
            t[counter] = pass.GetComponent<Passenger>().MoveTo(displayPos, false);
            displayPos = new Vector3(waitingOffset.x + displayPos.x, waitingOffset.y + displayPos.y, waitingOffset.z + displayPos.z);
            counter++;
        }
        await Task.WhenAll(t);
        uiMan.CanInteract(true);
    }

    //generates the passengers which will be available in the current town
    void GeneratePassengers()
    {
        Town town = GetTown();
        int numPass = GetPassNum(town);
        waitingPass = new List<GameObject>();
        for(int i = 0; i < numPass; i++)
        {
            waitingPass.Add(GeneratePassenger(town));
        }
    }

    //generates an individual passenger based on the stats of the town
    GameObject GeneratePassenger(Town town)
    {
        float startWealth = GetTownWealth(town);
        float wealth = UnityEngine.Random.Range(0f, 1f);
        Debug.Log(" min: " + (.5f - wealthDev) + " max: " + (.5f + wealthDev));
        float goldMod = wealth * 2 * wealthDev + (.5f - wealthDev);
        int gold = Mathf.Max((int)(goldMod * startWealth), 1);
        float happiness = UnityEngine.Random.Range(startHappiness - happinessDev, startHappiness + happinessDev);
        //string name = firstNames.ToArray()[UnityEngine.Random.Range(0, firstNames.ToArray().Length)] + " " + lastNames.ToArray()[UnityEngine.Random.Range(0, firstNames.ToArray().Length)];
        //string pm = pms.ToArray()[UnityEngine.Random.Range(0, pms.ToArray().Length)];
        //string am = ams.ToArray()[UnityEngine.Random.Range(0, ams.ToArray().Length)]; 
        //string dm = dms.ToArray()[UnityEngine.Random.Range(0, dms.ToArray().Length)];
        Town destination = GetDestination();
        GameObject newPass = Instantiate(passPrefab);
        newPass.GetComponent<Passenger>().Setup(gold, wealth, happiness, destination.GetName());
        passGen.SetupPassenger(newPass.GetComponent<Passenger>());
        return newPass;

    }

    //gets the default starting wealth for the given town
    float GetTownWealth(Town town)
    {
        float wealth = town.GetWealth();
        wealth *= wealthMod;
        return wealth;
        //return Mathf.Max(0f, Mathf.Min(1f, wealth));
    }

    //determines the number of passengers to come from a given town
    int GetPassNum(Town town)
    {
        if (town.IsDestroyed())
        {
            return 1;
        }
        float rep = town.GetRep();
        float comp = rep * passRepMod;
        return (int)Mathf.Max(1f, Mathf.Min(comp, maxPassPerTown));

    }

    //gets the current town
    Town GetTown()
    {
        return townDict.GetCurrentTown();
    }

    //gets a random town for a destination
    Town GetDestination()
    {
        return townDict.GenerateDestination();
    }

    //Sets up the new waiting passenger to ask for passage
    void NewPassenger()
    {
        
        if(GetCurrentWaitingPass() == null)
        {
            Debug.Log("NewPassenger Null");
            uiMan.DisplayError("No more passengers waiting");
            uiMan.DisplayText("");
            //Debug.Log("GameManager list size: " + currentPassNum);
            return;
        }
        Passenger pass = GetCurrentWaitingPass().GetComponent<Passenger>();
        uiMan.DisplayPassText(pass);
    }

    //Accepts the current waiting passenger. Used for button interaction
    public async void AcceptPass()
    {
        
        Debug.Log("accept: list length: " + waitingPass.Count);
        //Debug.Log("accept: list length: " + waitingPass.Count);
        GameObject pass = GetCurrentWaitingPass();
        if (pass == null)
        {
            audioMan.SelectAudio();
            uiMan.DisplayError("No passengers to accept");
            return;
        }
        if (currentPassNum >= trainCap)
        {
            audioMan.SelectAudio();
            uiMan.DisplayError("Tried to add past train max capacity. Should not add");
            return;
        }
        audioMan.AcceptAudio();
        uiMan.SetConductorImage(1);
        uiMan.CanInteract(false);
        //Debug.Log("accept: list length: " + waitingPass.Count);
        uiMan.DisplayText(pass.GetComponent<Passenger>().GetAccept());
        waitingPass.Remove(pass);
        await pass.GetComponent<Passenger>().MoveTo(OnTrainLoc.transform.position, false);
        AddPass(pass);
        DisplayPass();
        GameManager.Instance.AddPassenger();
        uiMan.SetConductorImage(0);
        await MoveWaitingPass();
        NewPassenger();
        
    }

    //Denies the current waiting passenger. Used for button interaction
    public async void DenyPass()
    {

        GameObject pass = GetCurrentWaitingPass();
        if(pass == null)
        {
            audioMan.SelectAudio();
            uiMan.DisplayError("No passengers to deny");
            return;
        }
        audioMan.DenyAudio();
        uiMan.SetConductorImage(2);
        uiMan.CanInteract(false);
        uiMan.DisplayText(pass.GetComponent<Passenger>().GetDeny());
        waitingPass.Remove(pass);
        await pass.GetComponent<Passenger>().MoveTo(OffPlatformLoc.transform.position, false);
        uiMan.SetConductorImage(0);
        Destroy(pass);
        await MoveWaitingPass();
        NewPassenger();
        
        
    }

    //adds the passenger at the given index from the current passenger list. Used for button interaction
    public void AddPass(GameObject pass)
    {
        DontDestroyOnLoad(pass);
        Debug.Log("trying to add passenger");
        for(int i = 0; i < currentPass.Length; i++)
        {
            if(currentPass[i] == null)
            {
                currentPass[i] = pass;
                currentPassNum++;
                return;
            } 
        }
        Debug.LogWarning("could not find open space to add passenger. Should not have been allowed to add");
        
        //RemovePass(currentPass[pos]);

    }

    //Removes the given passenger from the current passenger list. 
    public void RemovePass(GameObject pass)
    {
        int i = Array.IndexOf(currentPass, pass);
        if (i < 0)
        {
            Debug.LogWarning("tried to remove nonexistant passenger");
            return;
        }
        RemovePass(i);
    }

    //Forcibly removes the passenger from the given index. Used for button interaction
    public async void ForceRemovePass(int pos)
    {
        Passenger passScript = currentPass[pos].GetComponent<Passenger>();
        audioMan.RemoveAudio();
        uiMan.CanInteract(false);
        passScript.Display(OnTrainLoc.transform.position);
        await passScript.MoveTo(RemovedLoc.transform.position, true);
        float happinessChange = (1 - passScript.GetHappiness()) / repHapMod;
        if(passScript.GetTrait() == "Famous")
        {
            happinessChange *= 2f;
        }
        else if(passScript.GetTrait() == "Unobtrusive")
        {
            happinessChange *= .5f;
        }
        GetTown().RemoveRep(happinessChange);
        await Task.Delay(100);
        await passScript.MoveTo(OffPlatformLoc.transform.position, false);
        uiMan.CanInteract(true);
        RemovePass(pos);
    }

    //Removes the passenger at the given index from the current passenger list. 
    public void RemovePass(int pos)
    {
        Debug.Log("trying to remove passenger " + pos);
        Destroy(currentPass[pos]);
        currentPass[pos] = null;
        currentPassNum--;
        GameManager.Instance.RemovePassenger();
        DisplayPass();
        //RemovePass(currentPass[pos]);
        
    }

    //what to do when you successfully drop off a passenger at the correct location
    public async Task DropOffSuccess(GameObject pass)
    {
        Passenger passScript = pass.GetComponent<Passenger>();
        Debug.Log("Passenger " + passScript.GetName() + " successfully dropped off");
        uiMan.CanInteract(false);
        float gold = (float)passScript.GetGold();
        float tip = CalculateTip(passScript);
        if(passScript.GetTrait() == "Generous")
        {
            tip *= 2f;
        }
        else if(passScript.GetTrait() == "Stiff")
        {
            tip *= .5f;
        }
        gold *= (1f + tip);
        GetTown().AddWealth(gold / wealthMod);
        float happinessChange = passScript.GetHappiness() / repHapMod;
        if(passScript.GetTrait() == "Famous")
        {
            happinessChange *= 2f;
        }
        else if (passScript.GetTrait() == "Unobtrusive")
        {
            happinessChange *= .5f;
        }
        GetTown().AddRep(happinessChange);
        passScript.Display(OnTrainLoc.transform.position);

        await passScript.MoveTo(waitingLoc.transform.position, false);
        int currentGold = GameManager.Instance.GetGold();
        int newGold = passScript.GetGold();
        await Task.Delay(100);
        passScript.DropOff(GetTown());
        uiMan.DisplayText(pass.GetComponent<Passenger>().GetDropOff());
        var t = new Task[3];
        t[0] = passScript.MoveTo(OffPlatformLoc.transform.position, false);
        t[1] = uiMan.AdjustGold(currentGold, currentGold + newGold);
        if (tip > 0f)
        {
            t[2] = uiMan.DisplayTip(tip);
        }
        else
        {
            t[2] = Task.Delay(100);
        }
        await Task.WhenAll(t);
        uiMan.HideIncrement();
        GameManager.Instance.AddGold(newGold);
        uiMan.CanInteract(true);
        RemovePass(pass);
    }

    public float CalculateTip(Passenger pass)
    {
        float happiness = pass.GetHappiness();
        float tt = tipThreshold;
        if(pass.GetTrait() == "Charitable")
        {
            tt *= .5f;
        }
        else if(pass.GetTrait() == "Stiff")
        {
            tt *= 1.5f;
        }
        return Mathf.Clamp((happiness - tt) / (1 - tt) * maxTip, 0f, maxTip);
    }

    //Get the number of passengers currently on the train
    void FindCurrentPassNum()
    {
        int numPass = 0;
        foreach(GameObject p in currentPass)
        {
            if(p != null)
            {
                numPass++;
            }
        }
        currentPassNum = numPass;
    }
}
