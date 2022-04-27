using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainUpgrader : MonoBehaviour
{
    [Tooltip("Train car gameobjects"), SerializeField]
    private GameObject[] trainCars;
    [Tooltip("Cars text"), SerializeField]
    private TextMeshProUGUI carCountText;
    [Tooltip("Upgrade level text"), SerializeField]
    private TextMeshProUGUI upgradeLevelText;
    [Tooltip("Buy car button"), SerializeField]
    private Button buyCar;
    [Tooltip("Upgrade car button"), SerializeField]
    private Button upgradeCar;
    [Tooltip("Car cost text"), SerializeField]
    private TextMeshProUGUI carCostText;
    [Tooltip("Upgrade cost text"), SerializeField]
    private TextMeshProUGUI upgradeCostText;
    [SerializeField]
    public int maxCars = 3;
    [SerializeField]
    public int maxUpgradeLevel = 5;
    private int cars = 1;
    private int upgradeLevel = 0;
    [SerializeField]
    private int baseUpgradeCost;
    private int upgradeCost;
    [SerializeField]
    private int baseCarCost;
    private int carCost;
    private void OnEnable()
    {
        cars = GameManager.Instance.carCount;
        upgradeLevel = GameManager.Instance.carLevel;
        for (int i = 0; i < cars; i++)
        {
            trainCars[i].SetActive(true);
        }
        CheckBuyButton();
        CheckUpgradeButton();
    }
    public void UpgradeCar()
    {
        GameManager.Instance.UpgradeCars(upgradeCost);
        upgradeLevel++;
        CheckUpgradeButton();
        CheckBuyButton();
    }
    public void BuyCar()
    {
        GameManager.Instance.BuyCar(carCost);
        trainCars[cars].SetActive(true);
        cars++;
        CheckBuyButton();
        CheckUpgradeButton();
    }
    private void CheckUpgradeButton()
    {
        upgradeLevelText.text = "Car Level: " + upgradeLevel + "/" + maxUpgradeLevel;
        upgradeCost = baseUpgradeCost * (upgradeLevel + 1);
        upgradeCostText.text = "Cost: " + upgradeCost + " gold";
        if (GameManager.Instance.gold < upgradeCost || upgradeLevel == maxUpgradeLevel)
        {
            upgradeCar.interactable = false;
        }
        else
        {
            upgradeCar.interactable = true;
        }
    }
    private void CheckBuyButton()
    {
        carCountText.text = "Cars: " + cars + "/" + maxCars;
        carCost = baseCarCost * cars;
        carCostText.text = "Cost: " + carCost + " gold";
        if (GameManager.Instance.gold < carCost || cars == maxCars)
        {
            buyCar.interactable = false;
        }
        else
        {
            buyCar.interactable = true;
        }
    }
    public void Exit()
    {
        this.gameObject.SetActive(false);
    }
}
