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
    }
    public PointType trackPointType;
    private void Start()
    {
        if (trackPointType == PointType.Continue)
        {
            SetCurrent(0);
        }
    }
    public void SetCurrent(int i)
    {
        if (i != 0)
        {
            Debug.Log("clicked");
        }
        chosenNext = nextPoints[i];
    }
    public void StationStop()
    {

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
