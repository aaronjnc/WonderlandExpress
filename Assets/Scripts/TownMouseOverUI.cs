using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TownMouseOverUI : MonoBehaviour
{
    [Tooltip("whether the ui can be interacted with or not")]
    public bool interact = true;

    [Tooltip("The UI to show when mousing over a town")]
    public GameObject townStats;

    [Tooltip("current object being displayed")]
    public Town currentTownDisplay;

    [Tooltip("z offset for detecting objects")]
    public float zOffset = 0f;

    [Tooltip("camera z offset for detecting object")]
    public float camOffset = -10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
                StationPoint station = obj.GetComponent<StationPoint>();
                if (station != null)
                {
                    Town town = TownDictionary.Instance.FindTown(station.GetTownName());
                    if (town != null)
                    {
                        if (currentTownDisplay != town)
                        {
                            currentTownDisplay = town;
                            ShowTownStats(town);
                        }
                    }
                    else
                    {
                        HideTownStats();
                        currentTownDisplay = null;
                    }
                }
                else
                {
                    HideTownStats();
                    currentTownDisplay = null;
                }
            }
            else
            {
                HideTownStats();
                currentTownDisplay = null;
            }
        }
    }


    public void ShowTownStats(Town town)
    {
        townStats.SetActive(true);
        townStats.GetComponent<TownUIFollow>().SetupUI(town);
    }

    public void HideTownStats()
    {
        townStats.SetActive(false);
    }
}
