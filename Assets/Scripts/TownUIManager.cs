using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TownUIManager : MonoBehaviour
{
    [Header("UI pieces")]

    [Tooltip("The Text space used for showing the passenger messages")]
    public Text passMessageText;

    [Tooltip("The Text space used for showing 'error' messages (train is full, must remove passenger, etc.)")]
    public Text errorText;

    [Tooltip("All of the remove buttons used for the passengers. Must be listed in order from right to left")]
    public List<GameObject> removeButtons;

    [Tooltip("The default remove button prefab to be used. Not yet used.")]
    public GameObject removeButton;

    [Tooltip("The UI to show when mousing over a passenger")]
    public GameObject passengerStats;

    [Tooltip("current object being displayed")]
    public Passenger currentPassDisplay;

    [Tooltip("z offset for detecting objects")]
    public float zOffset = 0f;

    [Tooltip("camera z offset for detecting object")]
    public float camOffset = -10f;

    //instance of TownUIManager
    private static TownUIManager Instance;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    void FixedUpdate()
    {
        Debug.Log("trying to raycast");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
        mousePos = new Vector3(mousePos.x, mousePos.y, zOffset);
        //Vector3 cameraPos = Camera.main.transform.position;
        Vector3 cameraPos = new Vector3(mousePos.x, mousePos.y, camOffset);
        //Debug.Log("mouse: " + mousePos);
        RaycastHit2D hit = Physics2D.Raycast(cameraPos, (mousePos - cameraPos) * 100, Mathf.Infinity);
        Debug.DrawRay(cameraPos, (mousePos - cameraPos) * 100, Color.red, 0.1f);
        if ( hit.collider != null )
        {
            //Debug.Log("Hit " + hit.collider.gameObject);
            GameObject obj = hit.collider.gameObject;
            Passenger pass = obj.GetComponent<Passenger>();
            if(pass != null)
            {
                if(currentPassDisplay != pass)
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

    public void SetupButtons(GameObject[] arr)
    {
        int currentNum = 0;
        foreach(GameObject button in removeButtons)
        {
            button.SetActive(arr[currentNum] != null);
            currentNum++;
        }

    }

    public static TownUIManager GetManager()
    {
        return Instance;
    }

    public void DisplayPassText(Passenger pass)
    {
        DisplayText("Name: " + pass.GetName() + "\nDestination: " + pass.GetDestinationName() + "\nGold: " + pass.GetGold() + "\n" + pass.GetMessage());
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
        passengerStats.GetComponent<UIMouseFollow>().SetupUI(pass);
    }

    public void HidePassengerStats()
    {
        passengerStats.SetActive(false);
    }
}
