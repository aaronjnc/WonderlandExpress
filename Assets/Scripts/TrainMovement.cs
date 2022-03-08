using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [Tooltip("Maximum train velocity")]
    public float maxVelocity;
    [Tooltip("How quickly the train accelerates")]
    public float acceleration;
    [HideInInspector]
    [Tooltip("Current train velocity")]
    public float velocity;
    [Tooltip("Reference to next track point")]
    public TrackPoint nextPoint;
    [Tooltip("Vector representing current velocity")]
    private Vector3 currentVel = Vector3.zero;
    [Tooltip("How far from next point to begin slowing")]
    public float stoppingDistance;
    [Tooltip("Train is stopped")]
    private bool stopped = false;
    void FixedUpdate()
    {
        if (!stopped && Vector3.Distance(transform.position, nextPoint.transform.position) > 0)
        {
            if (Vector3.Distance(transform.position, nextPoint.transform.position) < stoppingDistance
                && nextPoint.trackPointType != TrackPoint.PointType.Continue)
            {
                if (currentVel == Vector3.zero)
                {
                    currentVel = transform.right * velocity;
                }
                transform.position = Vector3.SmoothDamp(transform.position, nextPoint.transform.position, ref currentVel, 1, maxVelocity);
                if (Vector3.Distance(transform.position, nextPoint.transform.position) < .1)
                {
                    transform.position = nextPoint.transform.position;
                }
            }
            else
            {
                velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
                transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, velocity*Time.deltaTime);
            }
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
        else if (nextPoint.chosenNext != null)
        {
            if (nextPoint.trackPointType != TrackPoint.PointType.Continue)
            {
                velocity = 0;
                currentVel = Vector3.zero;
            }
            TrackPoint previousChosen = nextPoint;
            nextPoint = nextPoint.chosenNext;
            if (previousChosen.trackPointType == TrackPoint.PointType.Choice)
            {
                previousChosen.chosenNext = null;
            }
            transform.right = nextPoint.transform.position - transform.position;
            if (previousChosen.trackPointType == TrackPoint.PointType.Station)
            {
                nextPoint.StationStop(this.gameObject);
            }
            stopped = false;
        }
        else
        {
            stopped = true;
        }
    }
}
