using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    [Header("Manager stuff")]

    [Tooltip("UI manager for town passenger setting")]
    public TownUIManager uiMan;

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

    [Tooltip("Passengers currently on the train")]
    public List<GameObject> currentPass;

    [Tooltip("maximum number of passengers on the train")]
    public int trainCap = 5;

    [Tooltip("Passengers waiting to get on the train")]
    public List<GameObject> waitingPass;

    [Tooltip("The maximum number of passengers that can try to board at each town")]
    public int maxPassPerTown = 5;

    [Header("General Passenger Generation Stats")]

    [Tooltip("The maximum percentage deviation from the generated wealth value a passenger can have between 0 and .5. \neg. a passenger can be x% lower or x% higher than the general value for a town")]
    [Range(0f, 1f)]
    public float wealthDev = .25f;

    [Tooltip("The default happiness of a passenger from 0 to 1")]
    [Range(0f, 1f)]
    public float startHappiness = .5f;

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
        Debug.Log("array size: " + waitingPass.Count + ", " + waitingPass.ToString());
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
            pass.GetComponent<Passenger>().Hide();
        }
        foreach(GameObject pass in waitingPass)
        {
            pass.GetComponent<Passenger>().Hide();
        }
    }

    //Sets up a new town's display
    void SetupTown()
    {
        CheckPassengerDestination();
        GeneratePassengers();
        //Debug.Log("list length: " + waitingPass.Count);
        DisplayAllPass();
        //Debug.Log("list length: " + waitingPass.Count);
        NewPassenger();
        //Debug.Log("list length: " + waitingPass.Count);
    }

    //checks all current passengers and removes all passengers who have reached their destination
    void CheckPassengerDestination()
    {
        List<GameObject> removed = new List<GameObject>();
        Town town = GetTown();
        foreach (GameObject pass in currentPass)
        {
            Town destination = pass.GetComponent<Passenger>().GetDestination();
            if(destination == town)
            {
                removed.Add(pass);
            }
        }
        foreach(GameObject pass in removed)
        {
            DropOffSuccess(pass);
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
            pass.GetComponent<Passenger>().Display(displayPos);
            displayPos += passOffset;
        }
        uiMan.SetupButtons(currentPass.Count);
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
        float wealth = Random.Range(startWealth - wealthDev, startWealth + wealthDev);
        float happiness = Random.Range(startHappiness - happinessDev, startHappiness + happinessDev);
        string name = firstNames.ToArray()[Random.Range(0, firstNames.ToArray().Length)] + " " + lastNames.ToArray()[Random.Range(0, firstNames.ToArray().Length)];
        string pm = pms.ToArray()[Random.Range(0, pms.ToArray().Length)];
        string am = ams.ToArray()[Random.Range(0, ams.ToArray().Length)]; 
        string dm = dms.ToArray()[Random.Range(0, dms.ToArray().Length)];
        Town destination = GetDestination();
        GameObject newPass = Instantiate(passPrefab);
        newPass.GetComponent<Passenger>().Setup(name, wealth, happiness, destination, pm, am, dm);
        return newPass;

    }

    //gets the default starting wealth for the given town
    float GetTownWealth(Town town)
    {
        float wealth = town.GetWealth();
        wealth *= wealthMod;
        return Mathf.Max(0f, Mathf.Min(1f, wealth));
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
        return town;
    }

    //gets a random town for a destination
    Town GetDestination()
    {
        return dest;
    }

    //Sets up the new waiting passenger to ask for passage
    void NewPassenger()
    {
        
        if(GetCurrentWaitingPass() == null)
        {
            Debug.Log("NewPassenger Null");
            uiMan.DisplayError("No more passengers waiting");
            Debug.Log("GameManager list size: " + GameManager.Instance.GetPassengers().Count);
            return;
        }
        Passenger pass = GetCurrentWaitingPass().GetComponent<Passenger>();
        uiMan.DisplayText("Name: " + pass.GetName() + "\nDestination: " + pass.GetDestinationName());
    }

    //Accepts the current waiting passenger. Used for button interaction
    public void AcceptPass()
    {
        Debug.Log("accept: list length: " + waitingPass.Count);
        //Debug.Log("accept: list length: " + waitingPass.Count);
        GameObject pass = GetCurrentWaitingPass();
        if (pass == null)
        {
            uiMan.DisplayError("No passengers to accept");
            return;
        }
        if (currentPass.Count >= trainCap)
        {
            uiMan.DisplayError("Tried to add past train max capacity. Should not add");
            return;
        }
        //Debug.Log("accept: list length: " + waitingPass.Count);
        waitingPass.Remove(pass);
        currentPass.Add(pass);
        DisplayAllPass();
        NewPassenger();
    }

    //Denies the current waiting passenger. Used for button interaction
    public void DenyPass()
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

    //Removes the given passenger from the current passenger list. Used for button interaction
    public void RemovePass(GameObject pass)
    {
        if (!currentPass.Contains(pass))
        {
            Debug.LogWarning("tried to remove nonexistant passenger");
            return;
        }
        currentPass.Remove(pass);
        DisplayPass();
        Destroy(pass);
    }

    //Removes the passenger at the given index from the current passenger list. Used for button interaction
    public void RemovePass(int pos)
    {
        Debug.Log("trying to remove passenger " + pos);
        RemovePass(currentPass[pos]);
        
    }

    //what to do when you successfully drop off a passenger at the correct location
    public void DropOffSuccess(GameObject pass)
    {
        Debug.Log("Passenger " + pass.GetComponent<Passenger>().GetName() + " successfully dropped off");
        currentPass.Remove(pass);
    }

}
