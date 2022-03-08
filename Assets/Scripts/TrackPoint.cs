using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPoint : MonoBehaviour
{
    public TrackPoint[] nextPoints;
    [HideInInspector]
    public TrackPoint chosenNext;
    public enum PointType
    {
        Continue,
        Station,
        Choice,
        Toll,
    }
    public PointType trackPointType;
    private void Start()
    {
        if (trackPointType != PointType.Choice)
        {
            SetCurrent(0);
        }
    }
    public void SetCurrent(int i)
    {
        chosenNext = nextPoints[i];
    }
    public void TollStop(GameObject train)
    {
        //reduce train money by cost of toll
        //fail if not enough money
    }
    public void StationStop(GameObject train)
    {
        GameManager.Instance.setTrainPosition(train.transform.position);
        //start next scene
    }
    private void OnDrawGizmos()
    {
        if (nextPoints == null)
        {
            return;
        }
        foreach (TrackPoint t in nextPoints)
        {
            Gizmos.DrawLine(transform.position, t.transform.position);
        }
    }
}
