using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TownSave
{
    [Header("General Town Stats")]
    [Tooltip("Town name")]
    public string townName;
    [Tooltip("Town wealth")]
    public float wealth;
    [Tooltip("Town reputation")]
    [Range(0f,100f)]
    public float reputation = 30;
    [Tooltip("If the town is destroyed or not")]
    public bool destroyed = false;

    public void Setup(Town t)
    {
        townName = t.townName;
        wealth = t.wealth;
        reputation = t.reputation;
        destroyed = t.destroyed;
    }
}
