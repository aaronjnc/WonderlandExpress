using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownUIFollow : UIMouseFollow
{
    [Header("Town settings")]
    [Tooltip("Name text box")]
    public TextMeshProUGUI nameText;
    [Tooltip("Wealth text box")]
    public Image[] wealthImages;
    [Tooltip("Happiness text box")]
    public Image happinessImage;
    [Tooltip("The sprites that are used to display happiness")]
    public Sprite[] happinessSprites;
    [Tooltip("Destroyed image")]
    public Image destroyedImage;
    [Tooltip("How much wealth is required from a town to increase in wealth ranks")]
    public int wealthTiers = 30;
    // Start is called before the first frame update

    public void SetupUI(Town town)
    {
        nameText.text = town.GetName();
        int repCounter = (int)town.GetRep() / 20;
        if(repCounter > 4)
        {
            repCounter = 4;
        }
        happinessImage.sprite = happinessSprites[repCounter];

        int wealthCounter = (int)town.GetWealth() / wealthTiers + 1;
        //Debug.Log("wealth count at: " + wealthCounter + " with wealth at: " + town.GetWealth());
        if (wealthCounter > 5)
        {
            wealthCounter = 5;
        }
        for (int i = 0; i < wealthImages.Length; i++)
        {
            //Debug.Log(i);
            if (i < wealthCounter)
            {
                wealthImages[i].enabled = true;
                //Debug.Log("enable");
            }
            else
            {
                wealthImages[i].enabled = false;
            }
        }
        MoveToMouse();
        Enable();
    }
}
