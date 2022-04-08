using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassengerManager : MonoBehaviour
{
    [Header("Manager stuff")]

    [Tooltip("UI manager for town passenger setting")]
    public TownUIManager uiMan;

    [Tooltip("The town dictionary. used for finding current town")]
    private TownDictionary townDict;

    [Tooltip("The passenger generator. used for generating passenger names and lines")]
    public PassengerGenerator passGen;

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
    public float wealthDev = .5f;

    [Tooltip("The default happiness of a passenger from 0 to 1")]
    [Range(0f, 1f)]
    public float startHappiness = .7f;

    [Tooltip("The maximum percentage deviation from the generated happiness value a passenger can have between 0 and .5. \neg. a passenger can be x% lower or x% higher than the general start value")]
    [Range(0f, .5f)]
    public float happinessDev = .1f;

    [Header("modifiers for calculations")]

    [Tooltip("modifier for converting between town and passenger wealth. \nPassenger->Town: divide by modifer \nTown->Passenger: multiply by modifier ")]
    [Min(0f)]
    public float wealthMod = .2f;

    [Tooltip("modifier for converting between town rep and passenger happiness. \nPassenger->Town: divide by modifer \nTown->Passenger: multiply by modifier ")]
    [Min(0f)]
    public float repHapMod = .2f;

    [Tooltip("modifier for converting between town reputation and number of passengers \n rep * mod = num")]
    public float passRepMod = .05f;

    [Header("Passenger names/lines")]

    [Tooltip("Possible passenger first names")]
    public List<string> firstNames = new List<string>();

    [Tooltip("Possible passenger last names")]
    public List<string> lastNames = new List<string>();

    [Tooltip("Possible passenger passage messages")]
    public List<string> pms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> ams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> dms = new List<string>();

    [Header("Test variables")]
    public Town town;

    public Town dest;

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
        townDict = TownDictionary.Instance;
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
    void SetupTown()
    {
        SetupTownUI();
        passGen.InitializeNames(currentPass);
        PassActive();
        CheckPassengerDestination();
        GeneratePassengers();
        //Debug.Log("list length: " + waitingPass.Count);
        DisplayAllPass();
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

    //What to do when leaving the town
    public void LeaveTown()
    {
        PassInactive();
        uiMan.SwitchScene();
    }

    //checks all current passengers and removes all passengers who have reached their destination
    void CheckPassengerDestination()
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
            DropOffSuccess(pass);
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
            pass.GetComponent<Passenger>().Display(displayPos);
            displayPos += waitingOffset;
        }
    }

    //moves current waiting passengers to their new positions
    async void MoveWaitingPass()
    {
        Vector3 displayPos = waitingLoc.transform.position;
        foreach (GameObject pass in waitingPass)
        {
            pass.GetComponent<Passenger>().Display(displayPos);
            displayPos += waitingOffset;
        }
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
        float wealth = UnityEngine.Random.Range((.5f - wealthDev) * 10f, (.5f + wealthDev) * 10f) / 10f;
        Debug.Log(" min: " + (.5f - wealthDev) + " max: " + (.5f + wealthDev));
        int gold = (int)(wealth * startWealth);
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
        uiMan.CanInteract(false);
        Debug.Log("accept: list length: " + waitingPass.Count);
        //Debug.Log("accept: list length: " + waitingPass.Count);
        GameObject pass = GetCurrentWaitingPass();
        if (pass == null)
        {
            uiMan.DisplayError("No passengers to accept");
            return;
        }
        if (currentPassNum >= trainCap)
        {
            uiMan.DisplayError("Tried to add past train max capacity. Should not add");
            return;
        }
        //Debug.Log("accept: list length: " + waitingPass.Count);
        await pass.GetComponent<Passenger>().MoveTo(OnTrainLoc.transform.position);
        waitingPass.Remove(pass);
        AddPass(pass);
        DisplayAllPass();
        NewPassenger();
        uiMan.CanInteract(true);
    }

    //Denies the current waiting passenger. Used for button interaction
    public async void DenyPass()
    {
        GameObject pass = GetCurrentWaitingPass();
        if(pass == null)
        {
            uiMan.DisplayError("No passengers to deny");
            return;
        }
        waitingPass.Remove(pass);
        DisplayWaitingPass();
        NewPassenger();
        Destroy(pass);
    }

    //adds the passenger at the given index from the current passenger list. Used for button interaction
    public async void AddPass(GameObject pass)
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
    public async void RemovePass(GameObject pass)
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
    public void ForceRemovePass(int pos)
    {
        GetTown().RemoveRep((1 - currentPass[pos].GetComponent<Passenger>().GetHappiness() ) / repHapMod);
        RemovePass(pos);
    }

    //Removes the passenger at the given index from the current passenger list. 
    public void RemovePass(int pos)
    {
        Debug.Log("trying to remove passenger " + pos);
        Destroy(currentPass[pos]);
        currentPass[pos] = null;
        currentPassNum--;
        DisplayPass();
        //RemovePass(currentPass[pos]);
        
    }

    //what to do when you successfully drop off a passenger at the correct location
    public void DropOffSuccess(GameObject pass)
    {
        Debug.Log("Passenger " + pass.GetComponent<Passenger>().GetName() + " successfully dropped off");
        GetTown().AddWealth((float)pass.GetComponent<Passenger>().GetGold() / wealthMod);
        GetTown().AddRep(pass.GetComponent<Passenger>().GetHappiness() / repHapMod);
        GameManager.Instance.AddGold(pass.GetComponent<Passenger>().GetGold());
        uiMan.DisplayGold(GameManager.Instance.GetGold());
        RemovePass(pass);
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
