using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrain : MonoBehaviour
{
    public TrackPoint nextPoint;
    [SerializeField] private Transform train;
    [SerializeField] private float distanceBehind;
    private Quaternion lookRotation;
    [SerializeField]
    private float rotationSpeed;
    private void Start()
    {
        lookRotation = transform.rotation;
    }
    private void FixedUpdate()
    {    
        if (Vector3.Distance(transform.position, nextPoint.transform.position) < .06)
        {
            transform.position = nextPoint.transform.position;
            nextPoint = nextPoint.chosenNext;
            Vector3 diff = -(nextPoint.transform.position - transform.position);
            diff.Normalize();
            float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            lookRotation = Quaternion.Euler(0, 0, rot - 90);
        }
        if (transform.rotation != lookRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        float vel = TrainMovement.Instance.velocity;
        if (vel != 0)
            transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, vel * Time.deltaTime);
    }

    public void SetNextPoint(string name)
    {
        Debug.Log(name);
        nextPoint = GameObject.Find(name).GetComponent<TrackPoint>();
        if (train.up == transform.up && (train.position.x == transform.position.x || train.position.y == transform.position.y))
        {
            transform.position = train.position + (transform.up * distanceBehind);
        }
        Vector3 diff = -(nextPoint.transform.position - transform.position);
        diff.Normalize();
        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        lookRotation = Quaternion.Euler(0, 0, rot - 90);
    }

    public string GetNextPoint()
    {
        if (nextPoint.trackChoice)
        {
            transform.position = nextPoint.transform.position;
            nextPoint = nextPoint.chosenNext;
        }
        return nextPoint.name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.position = train.position + (transform.up * distanceBehind);
        Camera.main.gameObject.GetComponent<CameraTransition>().Transition(transform.position.x > 0);
    }
}
