using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuePoint : TrackPoint
{
    private void Awake()
    {
        gameObject.name = transform.position.ToString();
    }
    public override void StopAction()
    {
        return;
    }
    public override void SetChosenTrack(int i)
    {
        continuous = true;
        chosenNext = nextPoints[i];
    }
}
