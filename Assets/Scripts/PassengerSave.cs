using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassengerSave
{
    [Tooltip("Passenger first name")]
    public string firstName;
    [Tooltip("Passenger last name")]
    public string lastName;
    [Tooltip("Passenger name")]
    public string passName;
    [Tooltip("The amount of gold a passenger will pay if you successfully transport them.")]
    public int gold;
    [Tooltip("The passenger's wealth, relative to their town. Used for determining wealth tier (not used yet)")]
    public float wealth;
    [Tooltip("Passenger happiness")]
    [Range(0f, 1f)]
    public float happiness;
    [Tooltip("Passenger destination")]
    public string destination;
    [Header("Passenger lines")]
    [Tooltip("Passenger asking for passage")]
    public string passageMessage;
    [Tooltip("Passenger when passage is accepted")]
    public string acceptMessage;
    [Tooltip("Passenger when passage is denied")]
    public string denyMessage;
    [Tooltip("Passenger when dropped off")]
    public string dropOffMessage;
    [Header("Trait Settings")]
    [Tooltip("The trait that this passenger has. Dictates certain things they can do")]
    public string trait = "None";
    [Tooltip("Description of the passenger's trait")]
    public string traitDescription = "";
    [Header("Resource settings")]
    public float[] passengerPosition = new float[3];

    public void Setup(Passenger p)
    {
        firstName = p.firstName;
        lastName = p.lastName;
        passName = firstName + " " + lastName;
        gold = p.gold;
        wealth = p.wealth;
        happiness = p.happiness;
        destination = p.destination;
        passageMessage = p.passageMessage;
        acceptMessage = p.acceptMessage;
        denyMessage = p.denyMessage;
        dropOffMessage = p.dropOffMessage;
        trait = p.trait;
        traitDescription = p.traitDescription;
        Vector3 pos = p.transform.position;
        passengerPosition = new float[] {pos.x, pos.y, pos.z };
    }
}
