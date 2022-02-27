using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPoint : MonoBehaviour
{
    public TrackPoint[] nextPoints;
    [HideInInspector]
    public TrackPoint chosenNext;
    private void Start()
    {
        if (nextPoints != null && nextPoints.Length == 1)
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
