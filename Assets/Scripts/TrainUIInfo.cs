using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

public class TrainUIInfo : MonoBehaviour
{
    private TrainUIInfo _trainUIInfo;
    public TrainUIInfo trainUIInfo
    {
        get { return _trainUIInfo; }
    }
    [Tooltip("Gold textbox"), SerializeField]
    private TextMeshProUGUI goldText;
    [Tooltip("Gold Increment textbox"), SerializeField]
    private TextMeshProUGUI goldIncrementText;
    [Tooltip("Toll price textbox"), SerializeField]
    private TextMeshProUGUI tollPriceText;
    [Tooltip("Toll Increment textbox"), SerializeField]
    private TextMeshProUGUI tollIncrementPriceText;
    [Tooltip("Jabberwocky price textbox"), SerializeField]
    private TextMeshProUGUI jabberPriceText;
    [Tooltip("the rate that the gold number tick up at per frame")]
    public int tickRate = 1;
    public void Start()
    {
        _trainUIInfo = this;
        GameManager.Instance.TollChangeEvent += SlowUpdateToll;
        UpdateToll(GameManager.Instance.tollPrice, GameManager.Instance.gold, GameManager.Instance.GetJabberwockyPrice());
        HideIncrement();
    }
    public void UpdateToll(int toll, int gold, int jabberwocky)
    {
        goldText.text = "Gold: " + gold;
        tollPriceText.text = "Toll Price: " + toll;
        jabberPriceText.text = "Jabberwocky Price: " + jabberwocky;
        HideIncrement();
    }
    public async Task SlowUpdateToll(int currentToll, int newToll, int currentGold, int newGold, int jabberwocky)
    {
        DisplayGold(currentGold);
        if (newGold - currentGold != 0)
        {
            DisplayGoldIncrement(newGold - currentGold);
        }
        DisplayToll(currentToll);
        if (newToll - currentToll != 0)
        {
            DisplayTollIncrement(newToll - currentToll);
        }
        while (currentGold > newGold || currentToll < newToll)
        {
            if (currentGold - newGold < tickRate)
            {
                currentGold = newGold;
            }
            else
            {
                currentGold -= tickRate;
            }
            if (newToll - currentToll < newToll / 10)
            {
                currentToll = newToll;
            }
            else
            {
                currentToll += newToll / 10;
            }
            DisplayGold(currentGold);
            DisplayToll(currentToll);
            await Task.Yield();
        }
        HideIncrement();
        
        
        
    }

    public void DisplayGold(int gold)
    {
        goldText.text = "Gold: " + gold + "g";
    }

    public void DisplayToll(int toll)
    {
        tollPriceText.text = "Toll Price: " + toll + "g";
    }

    public void DisplayJW(int jabberwocky)
    {
        jabberPriceText.text = "Jabberwocky Price: " + jabberwocky + "g";
    }

    public void DisplayGoldIncrement(int goldDiff)
    {
        goldIncrementText.text = goldDiff + "g";
    }

    public void DisplayTollIncrement(int tollDiff)
    {
        tollIncrementPriceText.text = "+" + tollDiff + "g";
    }

    public void HideIncrement()
    {
        goldIncrementText.text = "";
        tollIncrementPriceText.text = "";
    }
}
