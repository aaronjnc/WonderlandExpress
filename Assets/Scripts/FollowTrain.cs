using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrain : MonoBehaviour
{
    public TrackPoint nextPoint;
    [SerializeField] private Transform train;
    [SerializeField] private float distanceBehind;
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, nextPoint.transform.position) < .06)
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
        if (-train.up == transform.right)
        {
            transform.position = train.position - (transform.right * distanceBehind);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.position = train.position - (transform.right * distanceBehind);
    }
}
