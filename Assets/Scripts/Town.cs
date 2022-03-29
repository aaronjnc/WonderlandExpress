using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    [Header("General Town Stats")]
    [Tooltip("Town name")]
    public string townName;
    [Tooltip("Town wealth")]
    public float wealth;
    [Tooltip("Town reputation")]
    public float reputation = 30;
    [Tooltip("If the town is destroyed or not")]
    public bool destroyed = false;
    [Tooltip("Towns that cannot be set as destinations from this town. \nUsed to prevent really long journeys for now")]
    public List<Town> townBanList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //adjust town wealth by adding the wealth of a passenger
    public void AddWealth(float passengerWealth)
    {
        wealth += (passengerWealth);
    }

    //adjust town reputation by adding the reputation of a passenger
    public void AddRep(float passengerRep)
    {
        reputation += (passengerRep);
    }

    //adjust town reputation by removing the reputation of a passenger
    public void RemoveRep(float passengerRep)
    {
        reputation -= (passengerRep);
    }

    //get town reputation
    public float GetRep()
    {
        return reputation;
    }

    //get town wealth
    public float GetWealth()
    {
        return wealth;
    }

    //get town name
    public string GetName()
    {
        return townName;
    }

    //Set the town's stats to 0 when a town is destroyed
    public void DestroyTown()
    {
        wealth = 0;
        reputation = 0;
        destroyed = true;
    }

    //checks if the town is destroyed
    public bool IsDestroyed()
    {
        return destroyed;
    }

    //gets the town ban lsit
    public List<Town> getBanList()
    {
        return townBanList;
    }
}
