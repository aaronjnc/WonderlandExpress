using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrain : MonoBehaviour
{
    private FollowPoint nextPoint;
    [SerializeField] private Transform train;
    [SerializeField] private float distanceBehind;
    private Quaternion lookRotation;
    [SerializeField]
    private float rotationSpeed;
    private Vector3 nextPos;
    [SerializeField]
    private int carNum;
    [SerializeField]
    private bool caboose;
    private void Start()
    {
        if (!GameManager.Instance.load)
        {
            nextPoint = GameManager.Instance.GetHeadPoint();
            nextPos = nextPoint.GetPos();
            lookRotation = transform.rotation;
        }
    }
    private void FixedUpdate()
    {    
        if (Vector3.Distance(transform.position, nextPos) < .06)
        {
            transform.position = nextPos;
            if (caboose)
            {
                nextPoint = GameManager.Instance.RemoveHead();
            }
            else
            {
                nextPoint = nextPoint.GetNext();
            }
            nextPos = nextPoint.GetPos();
            Vector3 diff = -(nextPos - transform.position);
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
            transform.position = Vector3.MoveTowards(transform.position, nextPos, vel * Time.deltaTime);
    }

    public void SetNextPoint(FollowPoint nextPoint)
    {
        if (carNum < GameManager.Instance.carCount)
            GetComponent<SpriteRenderer>().enabled = true;
        this.nextPoint = nextPoint;
        nextPos = nextPoint.GetPos();
        if (train.up == transform.up && (train.position.x == transform.position.x || train.position.y == transform.position.y))
        {
            transform.position = train.position + (transform.up * distanceBehind * (carNum + 1));
        }
        Vector3 diff = -(nextPos - transform.position);
        diff.Normalize();
        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        lookRotation = Quaternion.Euler(0, 0, rot - 90);
    }

    public FollowPoint GetNextPoint()
    {
        return nextPoint;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.position = train.position + (transform.up * distanceBehind * (carNum + 1));
        Camera.main.gameObject.GetComponent<CameraTransition>().Transition(transform.position.x > 0);
    }
}
