using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Trait
    {
        [Tooltip("the name of the trait")]
        public string name;
        [Tooltip("The trait's description")]
        public string description;
        [Tooltip("the passenger message for the trait")]
        public string pm;
        [Tooltip("the accept message for the trait")]
        public string am;
        [Tooltip("the deny message for the trait")]
        public string dm;
        [Tooltip("the drop-off message for the trait")]
        public string dom;
    }
    [Header("Real World Passenger Names")]

    [Tooltip("Possible real world passenger first names")]
    public List<string> rwFirstNames = new List<string>();

    [Tooltip("Possible real world passenger last names")]
    public List<string> rwLastNames = new List<string>();

    [Header("Wonderland Passenger Names")]

    [Tooltip("Possible real world passenger first names")]
    public List<string> wlFirstNames = new List<string>();

    [Tooltip("Possible real world passenger last names")]
    public List<string> wlLastNames = new List<string>();

    [Header("Poor Passenger Lines")]

    [Tooltip("Possible passenger passage messages")]
    public List<string> ppms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> pams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> pdms = new List<string>();

    [Tooltip("Possible passenger dropoff messages")]
    public List<string> pdoms = new List<string>();

    [Header("Moderate Passenger Lines")]

    [Tooltip("Possible passenger passage messages")]
    public List<string> mpms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> mams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> mdms = new List<string>();

    [Tooltip("Possible passenger dropoff messages")]
    public List<string> mdoms = new List<string>();

    [Header("Rich Passenger Lines")]

    [Tooltip("Possible passenger passage messages")]
    public List<string> rpms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> rams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> rdms = new List<string>();

    [Tooltip("Possible passenger dropoff messages")]
    public List<string> rdoms = new List<string>();

    [Header("Traits")]

    //[Tooltip("Traits")]
    //public Trait[] traitList;

    [Tooltip("List of all possible traits. Should start with normal")]
    public List<Trait> traitList;

    //DEPRECATED METHOD OF TRAIT CREATION
    //[Tooltip("Trait names. First should be 'Normal', followed by all other traits")]
    //public List<string> traits = new List<string>();
    //[Tooltip("Trait descriptions. Must follow same order as traits")]
    //public List<string> descriptions = new List<string>();
    //[Tooltip("Passenger messages to go with the traits. Must follow same order as traits. \n leave first result blank, it will be ignored")]
    //public List<string> tpms = new List<string>();
    //[Tooltip("Accept messages to go with the traits. Must follow same order as traits. \n leave first result blank, it will be ignored")]
    //public List<string> tams = new List<string>();
    //[Tooltip("Deny messages to go with the traits. Must follow same order as traits. \n leave first result blank, it will be ignored")]
    //public List<string> tdms = new List<string>();
    //[Tooltip("Drop off messages to go with the traits. Must follow same order as traits. \n leave first result blank, it will be ignored")]
    //public List<string> tdoms = new List<string>();

    [Tooltip("Chance of a passenger getting a trait other than 'Normal'")]
    public float traitChance = 0.35f;

    [Header("Other Stats")]

    [Tooltip("Barrier for poor passenger.\n Any passenger with wealth below this number is considered poor.")]
    float poor = .3f;

    [Tooltip("Barrier for rich passenger.\n Any passenger with wealth above this number is considered rich.")]
    float rich = .8f;

    //whether or not the player is in wonderland
    bool wonderland;

    [Header("Unique passenger values")]

    [Tooltip("whether the regular has spawned already or not")]
    public bool regularSpawned = false;

    [Tooltip("whether the regular is currently on the train or not")]
    public bool regularOnTrain = false;

    [Tooltip("Starting happiness for the regular passenger")]
    public float regularStartHappiness = .5f;

    [Tooltip("Starting wealth for the regular passenger")]
    public float regularStartWealth = .5f;

    //takes in an array of the current passengers and removes their names and traits from the list of possible options.
    public void Initialize(GameObject[] passengers)
    {
        IsWonderland();
        //check the names of each existing passenger to ensure no repeat names/traits
        foreach(GameObject obj in passengers)
        {
            if (obj != null)
            {
                Passenger pass = obj.GetComponent<Passenger>();
                if (wonderland)
                {
                    wlFirstNames.Remove(pass.GetFirst());
                    wlLastNames.Remove(pass.GetLast());
                }
                else
                {
                    rwFirstNames.Remove(pass.GetFirst());
                    rwLastNames.Remove(pass.GetLast());
                }
                if(pass.GetTrait() != traitList[0].name)
                {
                    int traitNum = GetIndex(pass.GetTrait());
                    if(traitNum < 0)
                    {
                        Debug.LogError("DID NOT FIND TRAIT IN LIST TO REMOVE");
                    }
                    traitList.RemoveAt(traitNum);
                    if(pass.GetTrait() == "Regular")
                    {
                        regularSpawned = true;
                        regularOnTrain = true;
                    }
                    //descriptions.RemoveAt(traitNum);
                    //tpms.RemoveAt(traitNum);
                   // tams.RemoveAt(traitNum);
                    //tdms.RemoveAt(traitNum);
                    //tdoms.RemoveAt(traitNum);

                }
            }
        }
        //check to see if the regular has spawned, to prevent repeat names/traits
        if (!regularOnTrain && GameManager.Instance.IsRegular())
        {
            regularSpawned = true;
            UniquePassengerInfo.UniquePass regular = GameManager.Instance.GetRegular();
            rwFirstNames.Remove(regular.firstName);
            rwLastNames.Remove(regular.lastName);
            //traitList.RemoveAt(GetIndex("Regular"));
        }
    }

    //gets the index of the trait with the given name
    public int GetIndex(string traitName)
    {
        for(int i = 0; i < traitList.Count; i++)
        {
            if(traitName == traitList[i].name)
            {
                return i;
            }
        }
        return -1;
    }

    //takes a given passenger and assigns them a name and lines 
    public void SetupPassenger(Passenger pass)
    {
        GenerateTrait(pass);
        GenerateName(pass);
        
        if(pass.GetTrait() == "Regular" && !regularSpawned)
        {
            regularSpawned = true;
            pass.SetWealth(regularStartWealth);
            pass.SetHappiness(regularStartHappiness);
            GameManager.Instance.InitializeUPI(pass);
        }
    }

    //generates a random name for the passenger and removes the name from the lists
    public void GenerateName(Passenger pass)
    {
        string first;
        string last;
        if(pass.GetTrait() == "Regular" && regularSpawned)
        {
            UniquePassengerInfo.UniquePass regular = GameManager.Instance.GetRegular();
            first = regular.firstName;
            last = regular.lastName;
            pass.SetWealth(regular.wealth);
            pass.SetHappiness(regular.happiness);
        }
        else if (wonderland)
        {
            first = wlFirstNames.ToArray()[UnityEngine.Random.Range(0, wlFirstNames.ToArray().Length)];
            wlFirstNames.Remove(first);
            last = wlLastNames.ToArray()[UnityEngine.Random.Range(0, wlLastNames.ToArray().Length)];
            wlLastNames.Remove(last);
        }
        else
        {
            first = rwFirstNames.ToArray()[UnityEngine.Random.Range(0, rwFirstNames.ToArray().Length)];
            rwFirstNames.Remove(first);
            last = rwLastNames.ToArray()[UnityEngine.Random.Range(0, rwLastNames.ToArray().Length)];
            rwLastNames.Remove(last);
        }
        pass.SetName(first, last);
    }

    //generates random lines for the passenger
    public void GenerateLines(Passenger pass, int traitNum)
    {
        float wealth = pass.GetWealth();
        string pm;
        string am;
        string dm;
        string dom;
        if (traitNum != 0)
        {
            Trait t = traitList[traitNum];
            traitList.RemoveAt(traitNum);
            pm = t.pm;
            am = t.am;
            dm = t.dm;
            dom = t.dom;
            //pm = tpms[traitNum];
            //tpms.RemoveAt(traitNum);
            //am = tams[traitNum];
            //tams.RemoveAt(traitNum);
            //dm = tdms[traitNum];
           // tdms.RemoveAt(traitNum);
            //dom = tdoms[traitNum];
            //tdoms.RemoveAt(traitNum);

        }
        else
        {
            if (wealth < poor)
            {
                pm = ppms.ToArray()[UnityEngine.Random.Range(0, ppms.ToArray().Length)];
                am = pams.ToArray()[UnityEngine.Random.Range(0, pams.ToArray().Length)];
                dm = pdms.ToArray()[UnityEngine.Random.Range(0, pdms.ToArray().Length)];
                dom = pdoms.ToArray()[UnityEngine.Random.Range(0, pdoms.ToArray().Length)];
            }
            else if (wealth > rich)
            {
                pm = rpms.ToArray()[UnityEngine.Random.Range(0, rpms.ToArray().Length)];
                am = rams.ToArray()[UnityEngine.Random.Range(0, rams.ToArray().Length)];
                dm = rdms.ToArray()[UnityEngine.Random.Range(0, rdms.ToArray().Length)];
                dom = rdoms.ToArray()[UnityEngine.Random.Range(0, rdoms.ToArray().Length)];
            }
            else
            {
                pm = mpms.ToArray()[UnityEngine.Random.Range(0, mpms.ToArray().Length)];
                am = mams.ToArray()[UnityEngine.Random.Range(0, mams.ToArray().Length)];
                dm = mdms.ToArray()[UnityEngine.Random.Range(0, mdms.ToArray().Length)];
                dom = mdoms.ToArray()[UnityEngine.Random.Range(0, mdoms.ToArray().Length)];
            }
        }
        pass.SetLines(pm, am, dm, dom);

        //if(pass.GetTrait() == "Regular")
        //{
            //if (regularSpawned)
            //{
                
            //}
        //}
    }

    //generates trait for the passenger
    public void GenerateTrait(Passenger pass)
    {
        int traitNum = 0;
        if(regularSpawned && !regularOnTrain && GameManager.Instance.CheckRegularTown())
        {
            traitNum = GetIndex("Regular");
            //GameManager.Instance.InitializeUPI(pass);
            //regularOnTrain = true;
        }
        else if(UnityEngine.Random.Range(0f, 1f) <= traitChance)
        {
            traitNum = UnityEngine.Random.Range(1, traitList.Count);
            
            //traits.RemoveAt(traitNum);
            //descriptions.RemoveAt(traitNum);
        }
        //else
        //{
            //pass.SetTrait(traitList[0].name, traitList[0].description);
        //}
        pass.SetTrait(traitList[traitNum].name, traitList[traitNum].description);

        if(pass.GetTrait() == "Traveler")
        {
            pass.SetHappiness(1f - pass.GetHappiness());
        }
        //if (!regularOnTrain && pass.GetTrait() == "Regular")
        //{
        //    GameManager.Instance.UpdateUPI(pass);
        //}

        GenerateLines(pass, traitNum);
    }


    //checks if player is currently in wonderland
    public void IsWonderland()
    {
        wonderland = false;
    }
}
