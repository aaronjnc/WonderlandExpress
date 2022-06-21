using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsMove : MonoBehaviour
{
    [Tooltip("the front clouds movement velocity")]
    public Vector3 frontMove;
    [Tooltip("the back clouds movement velocity")]
    public Vector3 backMove;
    [Tooltip("the lower clouds movement velocity")]
    public Vector3 lowerMove;
    [Tooltip("Where the front clouds should move to when no longer visible")]
    public Vector3 frontStartPoint;
    [Tooltip("Once the front clouds have passed this point they are no longer visible")]
    public Vector3 frontEndPoint;
    [Tooltip("Where the back clouds should move to when no longer visible")]
    public Vector3 backStartPoint;
    [Tooltip("Once the back clouds have passed this point they are no longer visible")]
    public Vector3 backEndPoint;
    [Tooltip("Where the lower clouds should move to when no longer visible")]
    public Vector3 lowerStartPoint;
    [Tooltip("Once the lower clouds have passed this point they are no longer visible")]
    public Vector3 lowerEndPoint;
    [Tooltip("The front clouds")]
    public GameObject[] frontClouds;
    [Tooltip("The back clouds")]
    public GameObject[] backClouds;
    [Tooltip("The lower clouds")]
    public GameObject[] lowerClouds;
    // Start is called before the first frame update
    // Initialize starting values for start and end points of different cloud sections based on how many clouds there are
    void Start()
    {
        //adjust start and end points if there are multiple clouds in the front
        if(frontClouds.Length > 1)
        {
            frontStartPoint = frontClouds[frontClouds.Length - 1].transform.position;
            Bounds bounds = frontClouds[0].GetComponent<SpriteRenderer>().bounds;
            frontEndPoint = bounds.center + new Vector3(bounds.extents.x * 2, 0, 0);
        }
        //set starting velocity for front clouds
        foreach(GameObject cloud in frontClouds)
        {
            cloud.GetComponent<Rigidbody2D>().velocity = frontMove;
        }
        //adjust start and end points if there are multiple clouds in the back
        if (backClouds.Length > 1)
        {
            backStartPoint = backClouds[backClouds.Length - 1].transform.position;
            Bounds bounds = backClouds[0].GetComponent<SpriteRenderer>().bounds;
            backEndPoint = bounds.center + new Vector3(bounds.extents.x * 2, 0, 0);
        }
        //set starting velocity for back clouds
        foreach (GameObject cloud in backClouds)
        {
            cloud.GetComponent<Rigidbody2D>().velocity = backMove;
        }
        //adjust start and end points if there are multiple clouds in the train windows
        if (lowerClouds.Length > 1)
        {
            lowerStartPoint = lowerClouds[lowerClouds.Length - 1].transform.position;
            Bounds bounds = lowerClouds[0].GetComponent<SpriteRenderer>().bounds;
            lowerEndPoint = bounds.center + new Vector3(bounds.extents.x * 2, 0, 0);
        }
        //set starting velocity for train window  clouds
        foreach (GameObject cloud in lowerClouds)
        {
            cloud.GetComponent<Rigidbody2D>().velocity = lowerMove;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if any cloud moves past their end position, shift them back to the start
        foreach (GameObject cloud in frontClouds)
        {
            if (cloud.GetComponent<SpriteRenderer>().bounds.center.x > frontEndPoint.x)
            {
                cloud.transform.position = frontStartPoint;
            }
        }
        foreach (GameObject cloud in backClouds)
        {
            if (cloud.GetComponent<SpriteRenderer>().bounds.center.x > backEndPoint.x)
            {
                cloud.transform.position = backStartPoint;
            }
        }
        foreach (GameObject cloud in lowerClouds)
        {
            if (cloud.GetComponent<SpriteRenderer>().bounds.center.x > lowerEndPoint.x)
            {
                cloud.transform.position = lowerStartPoint;
            }
        }
    }
}
