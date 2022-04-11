using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerGenerator : MonoBehaviour
{
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

    [Header("Moderate Passenger Lines")]

    [Tooltip("Possible passenger passage messages")]
    public List<string> mpms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> mams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> mdms = new List<string>();

    [Header("Rich Passenger Lines")]

    [Tooltip("Possible passenger passage messages")]
    public List<string> rpms = new List<string>();

    [Tooltip("Possible passenger accept messages")]
    public List<string> rams = new List<string>();

    [Tooltip("Possible passenger deny messages")]
    public List<string> rdms = new List<string>();

    [Header("Other Stats")]

    [Tooltip("Barrier for poor passenger.\n Any passenger with wealth below this number is considered poor.")]
    float poor = .3f;

    [Tooltip("Barrier for rich passenger.\n Any passenger with wealth above this number is considered rich.")]
    float rich = .8f;

    //whether or not the player is in wonderland
    bool wonderland;

    //takes in an array of the current passengers and removes their names from the list of possible options.
    public void InitializeNames(GameObject[] passengers)
    {
        IsWonderland();
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
            }
        }
    }

    //takes a given passenger and assigns them a name and lines 
    public void SetupPassenger(Passenger pass)
    {
        GenerateName(pass);
        GenerateLines(pass);
    }

    //generates a random name for the passenger and removes the name from the lists
    public void GenerateName(Passenger pass)
    {
        string first;
        string last;
        if (wonderland)
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
    public void GenerateLines(Passenger pass)
    {
        float wealth = pass.GetWealth();
        string pm;
        string am;
        string dm;
        if(wealth < poor)
        {
            pm = ppms.ToArray()[UnityEngine.Random.Range(0, ppms.ToArray().Length)];
            am = pams.ToArray()[UnityEngine.Random.Range(0, pams.ToArray().Length)];
            dm = pdms.ToArray()[UnityEngine.Random.Range(0, pdms.ToArray().Length)];
        }
        else if(wealth > rich)
        {
            pm = rpms.ToArray()[UnityEngine.Random.Range(0, rpms.ToArray().Length)];
            am = rams.ToArray()[UnityEngine.Random.Range(0, rams.ToArray().Length)];
            dm = rdms.ToArray()[UnityEngine.Random.Range(0, rdms.ToArray().Length)];
        }
        else
        {
            pm = mpms.ToArray()[UnityEngine.Random.Range(0, mpms.ToArray().Length)];
            am = mams.ToArray()[UnityEngine.Random.Range(0, mams.ToArray().Length)];
            dm = mdms.ToArray()[UnityEngine.Random.Range(0, mdms.ToArray().Length)];
        }
        pass.SetLines(pm, am, dm);
    }


    //checks if player is currently in wonderland
    public void IsWonderland()
    {
        wonderland = false;
    }
}