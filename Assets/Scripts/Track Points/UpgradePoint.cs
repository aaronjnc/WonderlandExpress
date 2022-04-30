using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePoint : TrackPoint
{
    [SerializeField]
    private GameObject upgradeScreen;
    public override bool StopAction()
    {
        Time.timeScale = 0;
        upgradeScreen.SetActive(true);
        return true;
    }
}
