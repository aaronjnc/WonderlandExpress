using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrain : MonoBehaviour
{
    public TrackPoint nextPoint;
    [SerializeField] private Rigidbody2D train;
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, nextPoint.transform.position) < .05)
        {
            transform.position = nextPoint.transform.position;
            nextPoint = nextPoint.chosenNext;
            transform.right = nextPoint.transform.position - transform.position;
        }
        float vel = TrainMovement.Instance.velocity;
        if (vel != 0)
            transform.position += transform.right * TrainMovement.Instance.velocity * Time.deltaTime;
    }

    public void SetNextPoint(string name)
    {
        nextPoint = GameObject.Find(name).GetComponent<TrackPoint>();
    }
}
