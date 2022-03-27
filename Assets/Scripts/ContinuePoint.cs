using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuePoint : TrackPoint
{
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
