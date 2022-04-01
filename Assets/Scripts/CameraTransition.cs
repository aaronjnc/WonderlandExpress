using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [Tooltip("Camera speed"), SerializeField]
    private float cameraSpeed;
    [Tooltip("Camera scale change speed")]
    private float cameraScaleSpeed;
    [Tooltip("Zoomed Out Camera Position"), SerializeField]
    private Vector3 zoomedOutPos;
    [Tooltip("Camera is zooming out")]
    private bool zoomingOut = false;
    [Tooltip("Camera is zooming in")]
    private bool zoomingIn = false;
    [Tooltip("Train transform"), SerializeField]
    private Transform train;
    [Tooltip("Camera object")]
    private Camera cam;
    [Tooltip("Camera not zoomed in yet")]
    private bool stillZooming = false;
    [Tooltip("Base size of camera"), SerializeField]
    private int minZoom = 5;
    [Tooltip("Zoomed out size of camera"), SerializeField]
    private int maxZoom = 30;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    public void ZoomOut()
    {
        zoomingOut = true;
        stillZooming = false;
        float dist = Vector3.Distance(transform.position, zoomedOutPos);
        float time = Mathf.Abs(dist) / cameraSpeed;
        cameraScaleSpeed = (maxZoom - minZoom) / time;
    }
    public void ZoomIn()
    {
        zoomingIn = true;
        Vector3 pos = train.position;
        pos.z = transform.position.z;
        float dist = Vector3.Distance(pos, zoomedOutPos);
        float time = Mathf.Abs(dist) / cameraSpeed;
        cameraScaleSpeed = (maxZoom - minZoom) / time;
    }
    private void FixedUpdate()
    {
        if (zoomingOut)
        {
            transform.position = Vector3.MoveTowards(transform.position, zoomedOutPos, cameraSpeed*Time.deltaTime);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + cameraScaleSpeed * Time.deltaTime, cam.orthographicSize, maxZoom);
            if (Vector3.Distance(transform.position, zoomedOutPos) < .001)
            {
                transform.position = zoomedOutPos;
                cam.orthographicSize = maxZoom;
                zoomingOut = false;
            }
        }
        else if (zoomingIn)
        {
            Vector3 pos = train.position;
            pos.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, pos, cameraSpeed * Time.deltaTime);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - cameraScaleSpeed * Time.deltaTime, minZoom, cam.orthographicSize);
            if (Vector3.Distance(transform.position, pos) < .01)
            {
                if (cam.orthographicSize != minZoom)
                    stillZooming = true;
                zoomingIn = false;
                TrainMovement.Instance.zoomedOut = false;
            }
        }
        else if (stillZooming)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - cameraScaleSpeed * Time.deltaTime, minZoom, cam.orthographicSize);
            if (cam.orthographicSize == minZoom)
                stillZooming = false;
        }
    }
}
