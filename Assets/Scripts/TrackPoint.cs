using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class TrackPoint : MonoBehaviour
{
    [Tooltip("Next points in track"), SerializeField]
    protected TrackPoint[] nextPoints;
    [Tooltip("Next point in track"), HideInInspector]
    public TrackPoint chosenNext;
    [Tooltip("This is not a stop point")]
    public bool continuous = true;
    [Tooltip("Track point has a choice"), HideInInspector] 
    public bool trackChoice = false;
    protected void Awake()
    {
        if (nextPoints.Length != 1)
        {
            trackChoice = true;
        }
        if (!trackChoice)
        {
            SetChosenTrack(0);
        }
    }
    private void TollReached()
    {

    }
    public virtual void SetChosenTrack(int i)
    {
        chosenNext = nextPoints[i];
    }
    public abstract void StopAction();

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
