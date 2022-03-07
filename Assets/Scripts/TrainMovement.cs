using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public float maxVelocity;
    public float velocity;
    public TrackPoint nextPoint;
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, nextPoint.transform.position) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, maxVelocity * Time.deltaTime);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
        else if (nextPoint.chosenNext != null)
        {
            TrackPoint previousChosen = nextPoint;
            nextPoint = nextPoint.chosenNext;
            if (previousChosen.trackPointType == TrackPoint.PointType.Choice)
            {
                previousChosen.chosenNext = null;
            }
            transform.right = nextPoint.transform.position - transform.position;
            if (nextPoint.trackPointType == TrackPoint.PointType.Station)
            {
                nextPoint.StationStop(this.gameObject);
            }
        }
    }
}
