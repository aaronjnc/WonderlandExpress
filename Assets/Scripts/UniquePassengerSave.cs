using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UniquePassengerSave
{
    public string firstName;
    public string lastName;
    public float happiness;
    public string currentTown;
    public float wealth;

    public void Setup(UniquePassengerInfo.UniquePass upi)
    {
        firstName = upi.firstName;
        lastName = upi.lastName;
        happiness = upi.happiness;
        currentTown = upi.currentTown;
        wealth = upi.wealth;
    }
}
