using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    [Tooltip("Maximum train velocity")]
    public float maxVelocity;
    [Tooltip("How quickly the train accelerates")]
    public float acceleration;
    [Tooltip("Current train velocity")]
    public float velocity;
    [Tooltip("Reference to next track point")]
    public TrackPoint nextPoint;
    Vector3 currentVel = Vector3.zero;
    void FixedUpdate()
    {
        if (nextPoint != null && nextPoint.trackPointType == TrackPoint.PointType.Continue)
        {
            velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
        }
        if (Vector3.Distance(transform.position, nextPoint.transform.position) > 0)
        {
            if (nextPoint.trackPointType == TrackPoint.PointType.Continue)
            {
                transform.position = Vector3.Slerp(transform.position, nextPoint.transform.position, maxVelocity);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, nextPoint.transform.position, ref currentVel, maxVelocity);
            }
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
        else if (nextPoint.chosenNext != null)
        {
            TrackPoint previousChosen = nextPoint;
            nextPoint = nextPoint.chosenNext;
            if (previousChosen.trackPointType == TrackPoint.PointType.Choice)
            {
                velocity = 0;
                previousChosen.chosenNext = null;
            }
            transform.right = nextPoint.transform.position - transform.position;
            if (nextPoint.trackPointType == TrackPoint.PointType.Station)
            {
                velocity = 0;
                nextPoint.StationStop(this.gameObject);
            }
        }
        else
        {
            nextPoint = null;
        }
    }
}
