using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainUIInfo : MonoBehaviour
{
    private TrainUIInfo _trainUIInfo;
    public TrainUIInfo trainUIInfo
    {
        get { return _trainUIInfo; }
    }
    [Tooltip("Gold textbox"), SerializeField]
    private TextMeshProUGUI goldText;
    [Tooltip("Toll price textbox"), SerializeField]
    private TextMeshProUGUI tollPriceText;
    [Tooltip("Jabberwocky price textbox"), SerializeField]
    private TextMeshProUGUI jabberPriceText;
    public void Start()
    {
        _trainUIInfo = this;
        GameManager.Instance.TollChangeEvent += UpdateToll;
        UpdateToll(GameManager.Instance.tollPrice, GameManager.Instance.gold, GameManager.Instance.GetJabberwockyPrice());
    }
    public void UpdateToll(int toll, int gold, int jabberwocky)
    {
        goldText.text = "Gold: " + gold;
        tollPriceText.text = "Toll Price: " + toll;
        jabberPriceText.text = "Jabberwocky Price: " + jabberwocky;
    }
}
