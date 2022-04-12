using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassUIFollow : UIMouseFollow
{
    [Header("Passenger settings")]
    [Tooltip("Name text box")]
    public Text nameText;
    [Tooltip("Gold text box")]
    public Text goldText;
    [Tooltip("Happiness text box")]
    public Text happinessText;
    [Tooltip("Destination text box")]
    public Text destinationText;
    // Start is called before the first frame update

    public void SetupUI(Passenger pass)
    {
        nameText.text = pass.GetName();
        goldText.text = pass.GetGold().ToString() + "g";
        happinessText.text = ((int)(pass.GetHappiness() * 100)).ToString() + "%";
        destinationText.text = pass.GetDestination();
        MoveToMouse();
        Enable();
    }
}
