using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using TMPro;
using System.Threading.Tasks;

public class TownUIManager : MonoBehaviour
{
    [Header("UI pieces")]

    [Tooltip("The tip text display")]
    public GameObject tipDisplay;

    [Tooltip("The Text space used for showing the passenger messages")]
    public TextMeshProUGUI passMessageText;

    [Tooltip("The Text space used for showing 'error' messages (train is full, must remove passenger, etc.)")]
    public TextMeshProUGUI errorText;

    [Tooltip("The Text space used for showing town name")]
    public TextMeshProUGUI townNameText;

    [Tooltip("The Text space used for showing current gold")]
    public TextMeshProUGUI goldText;

    [Tooltip("The Text space used for showing increases in gold")]
    public TextMeshProUGUI goldIncrementText;

    [Tooltip("The Text space used for showing the next toll price")]
    public TextMeshProUGUI tollText;

    [Tooltip("All of the remove buttons used for the passengers. Must be listed in order from right to left")]
    public List<GameObject> removeButtons;

    [Tooltip("The accept button")]
    public Button acceptButton;

    [Tooltip("The deny button")]
    public Button denyButton;

    [Tooltip("The leave button")]
    public Button leaveButton;

    [Tooltip("The left car change button")]
    public Button leftButton;

    [Tooltip("The right car change button")]
    public Button rightButton;

    [Tooltip("The default remove button prefab to be used. Not yet used.")]
    public GameObject removeButton;

    [Tooltip("The UI to show when mousing over a passenger")]
    public GameObject passengerStats;

    [Tooltip("The Conductor")]
    public SpriteRenderer conductor;

    [Tooltip("The second passenger car")]
    public GameObject secondTrain;

    [Header("UI Sprites")]

    [Tooltip("Conductor sprites. 0=default, 1=yes, 2=no")]
    public Sprite[] conductorSprites;

    [Header("UI Stats")]

    [Tooltip("current object being displayed")]
    public Passenger currentPassDisplay;

    [Tooltip("z offset for detecting objects")]
    public float zOffset = 0f;

    [Tooltip("camera z offset for detecting object")]
    public float camOffset = -10f;

    [Tooltip("whether the ui can be interacted with or not")]
    public bool interact = true;

    [Tooltip("the rate that number tick up at per frame")]
    public int tickRate = 1;

    [Tooltip("length of time in milliseconds the tip should be displayed for")]
    public float tipTime = 1000f;

    //instance of TownUIManager
    private static TownUIManager Instance;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        passMessageText.text = "";
        errorText.text = "";
        townNameText.text = "";
        goldText.text = "";
        goldIncrementText.text = "";
        tollText.text = "";
        passengerStats.SetActive(false);
    }

    void FixedUpdate()
    {
        if (interact)
        {


            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
            mousePos = new Vector3(mousePos.x, mousePos.y, zOffset);
            //Vector3 cameraPos = Camera.main.transform.position;
            Vector3 cameraPos = new Vector3(mousePos.x, mousePos.y, camOffset);
            //Debug.Log("mouse: " + mousePos);
            RaycastHit2D hit = Physics2D.Raycast(cameraPos, (mousePos - cameraPos) * 100, Mathf.Infinity);
            Debug.DrawRay(cameraPos, (mousePos - cameraPos) * 100, Color.red, 0.1f);
            if (hit.collider != null)
            {
                //Debug.Log("Hit " + hit.collider.gameObject);
                GameObject obj = hit.collider.gameObject;
                Passenger pass = obj.GetComponent<Passenger>();
                if (pass != null)
                {
                    if (currentPassDisplay != pass)
                    {
                        currentPassDisplay = pass;
                        ShowPassengerStats(pass);
                    }
                }
                else
                {
                    HidePassengerStats();
                    currentPassDisplay = null;
                }
            }
            else
            {
                HidePassengerStats();
                currentPassDisplay = null;
            }
        }
    }

    public void SetupButtons(GameObject[] arr, int currentCar)
    {
        int currentNum = 0;
        foreach(GameObject button in removeButtons)
        {
            //GameObject button = arr[i];
            button.SetActive(arr[currentCar * 5 + currentNum] != null);
            currentNum++;
        }

    }

    public void DisplayTrains(bool display)
    {
        secondTrain.SetActive(display);
    }

    public void DisplayLeft(bool display)
    {
        leftButton.gameObject.SetActive(display);
    }

    public void DisplayRight(bool display)
    {
        rightButton.gameObject.SetActive(display);
    }

    public async Task DisplayTip(float tip)
    {
        await tipDisplay.GetComponent<TipMove>().floatMove(tipTime, tip);
    }

    public void DisplayTown(Town t)
    {
        townNameText.text = t.GetName();
    }

    public void DisplayTown(string tName)
    {
        townNameText.text = tName;
    }

    public void DisplayGold(int g)
    {
        goldText.text = "Gold: " + g + "g";
    }

    public void DisplayIncrement(int g)
    {
        goldIncrementText.text = "+" + g + "g";
    }

    public void HideIncrement()
    {
        goldIncrementText.text = "";
    }

    public void DisplayToll(int t)
    {
        tollText.text = "Toll Price: " + t + "g";
    }

    public async Task AdjustGold(int current, int final)
    {

        DisplayGold(current);
        DisplayIncrement(final - current);
        while(current < final)
        {
            if(final - current < tickRate)
            {
                current = final;
            }
            else
            {
                current += tickRate;
            }
            DisplayGold(current);
            await Task.Yield();
        }
        
        
    }

    //not needed yet. but will format gold to show a reasonable number if values get too high.
    //public string formatGold(int gold)
    //{
        //if(gold > 1000000000)
        //{
        //    return ((float)gold) / 100
        //}
        //else if(gold > 1000000)
        //{

        //}
        //else if(gold > 1000)
        //{

    //  }
    //}

    public static TownUIManager GetManager()
    {
        return Instance;
    }

    public void DisplayPassText(Passenger pass)
    {
        DisplayText("Name: " + pass.GetName() + "\nDestination: " + pass.GetDestination() + "\nGold: " + pass.GetGold() + "\n" + pass.GetMessage() + "\n\nTrait: " + pass.GetTrait() + "\n" + pass.GetTraitDescription());
    }

    public void DisplayText(string text)
    {
        passMessageText.text = text;
        errorText.text = "";
    }

    public void DisplayError(string text)
    {
        errorText.text = text;
    }


    //REMOVE FROM THIS OBJECT. PUT SOMEWHERE ELSE
    public void SwitchScene()
    {
        GameManager.Instance.load = true;
        SceneManager.LoadScene("ILikeTrains");
    }

    public void ShowPassengerStats(Passenger pass)
    {
        passengerStats.SetActive(true);
        passengerStats.GetComponent<PassUIFollow>().SetupUI(pass);
    }

    public void HidePassengerStats()
    {
        passengerStats.SetActive(false);
    }

    //sets whether or not the buttons are interactable
    public void CanInteract(bool ci)
    {
        acceptButton.interactable = ci;
        denyButton.interactable = ci;
        leaveButton.interactable = ci;
        foreach(GameObject b in removeButtons)
        {
            b.GetComponent<Button>().interactable = ci;
        }
        interact = ci;
    }

    //sets the conductor to the given state. 0=default, 1=yes, 2=no
    public void SetConductorImage(int state)
    {
        conductor.sprite = conductorSprites[state];
    }
}
