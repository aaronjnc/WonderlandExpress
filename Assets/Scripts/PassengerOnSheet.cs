using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassengerOnSheet : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI passengerName;
    [SerializeField]
    private TextMeshProUGUI destination;
    [SerializeField]
    private TextMeshProUGUI gold;
    [SerializeField]
    private TextMeshProUGUI happiness;
    [SerializeField]
    private TextMeshProUGUI trait;
    private Passenger passenger;
    public void SetPassenger(Passenger p)
    {
        passenger = p;
        SetTexts();
        passengerName.enabled = true;
        destination.enabled = true;
        gold.enabled = true;
        happiness.enabled = true;
        trait.enabled = true;   
    }
    public void ClearPassenger()
    {
        passengerName.enabled = false;
        destination.enabled = false;
        gold.enabled = false;
        happiness.enabled = false;
        trait.enabled = false;
    }

    private void SetTexts()
    {
        passengerName.text = "Name: " + passenger.firstName + " " + passenger.lastName;
        destination.text = "Destination: " + passenger.destination;
        gold.text = "Payment: " + passenger.gold;
        happiness.text = "Happiness: " + Mathf.Round(passenger.happiness*100) + "%";
        trait.text = "Trait: " + passenger.trait;
    }
}
