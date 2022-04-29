using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassMapManager : MonoBehaviour
{
    [Tooltip("the sprite for the open map button")]
    public Sprite mapButtonSprite;
    [Tooltip("the sprite for the close map button")]
    public Sprite mapCloseSprite;
    [Tooltip("The image component for this object")]
    public Image buttonImage;
    [Tooltip("the map left arrow")]
    public GameObject mapLeftArrow;
    [Tooltip("the map right arrow")]
    public GameObject mapRightArrow;
    [Tooltip("the map panel")]
    public GameObject map;
    [Tooltip("the real world map sprite")]
    public Sprite mapRW;
    [Tooltip("the wonderland map sprite")]
    public Sprite mapWL;
    [Tooltip("if the player is currently in wonderland")]
    public bool trainInWonderland = false;
    [Tooltip("if the map is currently displaying wonderland")]
    public bool mapInWonderland = false;
    [Tooltip("if the map is being displayed or not")]
    public bool isMapDisplayed = false;
    [Tooltip("the container for wonderland town names")]
    public GameObject WLNames;
    [Tooltip("the container for real world  town names")]
    public GameObject RWNames;
    [Tooltip("The audio manager")]
    public TownAudioManager audioMan;
    // Start is called before the first frame update
    void Start()
    {
        mapLeftArrow.SetActive(false);
        mapRightArrow.SetActive(false);
        map.SetActive(false);
        trainInWonderland = GameManager.Instance.GetTrainPosition().x < 0;
        mapInWonderland = trainInWonderland;
        buttonImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //toggles the map on or off
    public void ToggleMap()
    {
        if (isMapDisplayed)
        {
            DisableMap();
        }
        else
        {
            EnableMap();
        }
    }

    //disables the map
    public void DisableMap()
    {
        mapLeftArrow.SetActive(false);
        mapRightArrow.SetActive(false);
        RWNames.SetActive(false);
        WLNames.SetActive(false);
        map.SetActive(false);
        mapInWonderland = trainInWonderland;
        isMapDisplayed = false;
        buttonImage.sprite = mapButtonSprite;
        audioMan.SelectAudio();
    }

    //enables the map
    public void EnableMap()
    {
        if (mapInWonderland)
        {
            MapWL();
        }
        else
        {
            MapRW();
        }
        buttonImage.sprite = mapCloseSprite;
        map.SetActive(true);
        isMapDisplayed = true;
    }

    //swaps the map to wonderland
    public void MapWL()
    {
        mapLeftArrow.SetActive(false);
        mapRightArrow.SetActive(true);
        RWNames.SetActive(false);
        WLNames.SetActive(true);
        map.GetComponent<Image>().sprite = mapWL;
        mapInWonderland = true;
        audioMan.SelectAudio();
    }

    //swaps the map to real world
    public void MapRW()
    {
        mapLeftArrow.SetActive(true);
        mapRightArrow.SetActive(false);
        RWNames.SetActive(true);
        WLNames.SetActive(false);
        map.GetComponent<Image>().sprite = mapRW;
        mapInWonderland = false;
        audioMan.SelectAudio();
    }
}
