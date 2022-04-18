using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollPoint : TrackPoint
{
    //[Tooltip("Cost to pass through this toll")]
    //public int tollCost = 50;
    [Tooltip("End game canvas obect"), SerializeField]
    private GameObject endGame;
    [Tooltip("Dialogue Manager")]
    public DialogueManager dm;
    [Tooltip("name of Dialogue String for success")]
    public string successString;
    [Tooltip("name of Dialogue String for failure")]
    public string failureString;
    public async override void StopAction()
    {
        if (GameManager.Instance.trainSceneTesting)
            return;
        Debug.Log("TollPoint activation");
        if (GameManager.Instance.CheckToll())
        {
            await dm.DisplayDialog(successString);
            
        }
        else
        {
            await dm.DisplayDialog(failureString);
        }
    }
}
