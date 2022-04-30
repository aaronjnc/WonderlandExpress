using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniquePassengerInfo : MonoBehaviour
{
    [System.Serializable]
    public struct UniquePass
    {
        public string firstName;
        public string lastName;
        public float happiness;
        public string currentTown;
        public float wealth;
    }
    [Tooltip("info for the unique regular passenger")]
    public UniquePass regular;
    [Tooltip("the amount values are modified by when dropped off")]
    public float successMod = .1f;
    [Tooltip("the amount values are modified by when kicked off")]
    public float failMod = .1f;
    [Tooltip("the amount values are modified when when ignored")]
    public float ignoreMod = .05f;

    public UniquePass GetRegular()
    {
        return regular;
    }

    public string GetRegularTown()
    {
        return regular.currentTown;
    }

    public bool RegularSpawned()
    {
        return regular.currentTown != "";
    }

    public void InitializeUPI(Passenger pass, string town)
    {
        if (pass.GetTrait() == "Regular")
        {
            regular.firstName = pass.GetFirst();
            regular.lastName = pass.GetLast();
            regular.happiness = pass.GetHappiness();
            regular.wealth = pass.GetWealth();
            regular.currentTown = town;
        }
    }

    public void DropOffUPI(Passenger pass, string town)
    {
        if(pass.GetTrait() == "Regular")
        {
            regular.happiness = Mathf.Clamp(regular.happiness + successMod, 0f, 1f);
            regular.wealth = Mathf.Clamp(regular.wealth + successMod, 0f, 1f);
            regular.currentTown = town;
        }
    }

    public void KickOffUPI(Passenger pass, string town)
    {
        if (pass.GetTrait() == "Regular")
        {
            regular.happiness = Mathf.Clamp(regular.happiness - failMod, 0f, 1f);
            regular.wealth = Mathf.Clamp(regular.wealth - failMod, 0f, 1f);
            regular.currentTown = town;
        }
    }

    public void IgnoreUPI(Passenger pass, string town)
    {
        if (pass.GetTrait() == "Regular")
        {
            regular.happiness = Mathf.Clamp(regular.happiness - ignoreMod, 0f, 1f);
            regular.wealth = Mathf.Clamp(regular.wealth - ignoreMod, 0f, 1f);
        }
    }
}
