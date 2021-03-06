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
    private Vector3 rightZoomedOut;
    [Tooltip("Zoomed out camera position for fantasy"), SerializeField]
    private Vector3 leftZoomedOut;
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
    [Tooltip("Base size of camera")]
    private float minZoom;
    [Tooltip("Zoomed out size of camera"), SerializeField]
    private float maxRegularZoom = 30;
    [Tooltip("Zoomed out size of fantasy camera"), SerializeField]
    private float maxFantasyZoom = 60;
    [Tooltip("Destination of camera")]
    private Vector3 dest = Vector3.zero;
    [Tooltip("Maximum zoom given current destination")]
    private float maxZoom = 0;
    [Tooltip("If the Camera can currently read input")]
    private bool interacting = true;
    [Tooltip("Camera is transitioning between frames"), HideInInspector]
    public bool transitioning = false;
    private int directionMod = 1;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        minZoom = cam.orthographicSize;
    }
    public void ZoomOut()
    {
        if (!interacting)
        {
            return;
        }
        if (transform.position.x < 0)
        {
            dest = leftZoomedOut;
            maxZoom = maxFantasyZoom;
        }
        else
        {
            dest = rightZoomedOut;
            maxZoom = maxRegularZoom;
        }
        zoomingIn = false;
        zoomingOut = true;
        stillZooming = false;
        float dist = Vector3.Distance(transform.position, dest);
        float time = Mathf.Abs(dist) / cameraSpeed;
        cameraScaleSpeed = (maxZoom - minZoom) / time;
    }
    public void ZoomIn()
    {
        if (!interacting)
        {
            return;
        }
        zoomingOut = false;
        zoomingIn = true;
        Vector3 pos = train.position;
        pos.z = transform.position.z;
        float dist = Vector3.Distance(pos, transform.position);
        float time = Mathf.Abs(dist) / cameraSpeed;
        cameraScaleSpeed = (maxZoom - minZoom) / time;
    }
    private void FixedUpdate()
    {
        if (zoomingOut)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, cameraSpeed*Time.deltaTime);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + cameraScaleSpeed * Time.deltaTime, cam.orthographicSize, maxZoom);
            if (Vector3.Distance(transform.position, dest) < .001)
            {
                transform.position = dest;
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
        else if (transitioning)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, cameraSpeed * Time.deltaTime);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + directionMod * cameraScaleSpeed * Time.deltaTime, minZoom, maxZoom);
            if (Vector3.Distance(transform.position, dest) < .001)
            {
                transform.position = dest;
                if (directionMod == 1)
                {
                    cam.orthographicSize = maxZoom;
                }
                else
                {
                    cam.orthographicSize = minZoom;
                }
                transitioning = false;
            }
        }
    }

    public void SetInteracting(bool canInteract)
    {
        interacting = canInteract;
    }
    public void Transition(bool rightToLeft)
    {
        if (cam.orthographicSize != maxFantasyZoom && cam.orthographicSize != maxRegularZoom)
            return;
        if (rightToLeft)
        {
            dest = leftZoomedOut;
            maxZoom = maxFantasyZoom;
            minZoom = cam.orthographicSize;
            float dist = Vector3.Distance(dest, transform.position);
            float time = Mathf.Abs(dist) / cameraSpeed;
            cameraScaleSpeed = (maxZoom - minZoom) / time;
            directionMod = 1;
        }
        else
        {
            dest = rightZoomedOut;
            maxZoom = cam.orthographicSize;
            minZoom = maxRegularZoom;
            float dist = Vector3.Distance(dest, transform.position);
            float time = Mathf.Abs(dist) / cameraSpeed;
            cameraScaleSpeed = (maxZoom - minZoom) / time;
            directionMod = -1;
        }
        transitioning = true;
    }
}
