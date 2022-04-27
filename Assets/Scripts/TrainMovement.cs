using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


public class TrainMovement : MonoBehaviour
{
    private static TrainMovement _instance;
    public static TrainMovement Instance
    {
        get
        {
            return _instance;
        }
    }
    [Tooltip("Maximum train velocity")]
    public float maxVelocity;
    [Tooltip("How quickly the train accelerates")]
    public float acceleration;
    [Tooltip("Current train velocity"), HideInInspector]
    public float velocity;
    [Tooltip("Reference to next track point")]
    public TrackPoint nextPoint;
    [Tooltip("Vector representing current velocity")]
    private Vector3 currentVel = Vector3.zero;
    [Tooltip("How far from next point to begin slowing")]
    public float stoppingDistance;
    [Tooltip("Train is stopped")]
    private bool stopped = false;
    [Tooltip("Pause Menu game object")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Player controls")]
    private PlayerControls controls;
    [Tooltip("Layer of track choice"), SerializeField] 
    private LayerMask choiceLayer;
    [Tooltip("List of following cars"), SerializeField] 
    private List<FollowTrain> trainCars = new List<FollowTrain>();
    [Tooltip("Camera is zoomed out"), HideInInspector]
    public bool zoomedOut = false;
    [Tooltip("Camera transition script"), SerializeField]
    private CameraTransition camTransition;
    [Tooltip("Train audio manager"), SerializeField]
    private TrainAudioManager trainAudioManager;
    private Quaternion lookRotation;
    [SerializeField]
    private float rotationSpeed;
    private float previousTime;
    private bool paused = false;
    private TrackPoint previousChosen;
    private void Start()
    {
        _instance = this;
        if (GameManager.Instance.load)
        {
            TrackPoint loadPoint = GameObject.Find(GameManager.Instance.GetCurrentStop()).GetComponent<TrackPoint>();
            nextPoint = GameObject.Find(GameManager.Instance.GetCurrentStop()).GetComponent<TrackPoint>().chosenNext;
            transform.position = GameManager.Instance.GetTrainPosition();
            transform.eulerAngles = GameManager.Instance.GetTrainRotation();
            LoadFollowTrains();
        }
        controls = new PlayerControls();
        controls.Menu.Pause.performed += Pause;
        controls.Menu.Pause.Enable();
        controls.ClickEvents.Click.performed += Click;
        controls.ClickEvents.Click.Enable();
        controls.ClickEvents.ZoomOut.performed += Zoom;
        controls.ClickEvents.ZoomOut.Enable();
        lookRotation = transform.rotation;
        trainAudioManager.SpeedUp();
    }
    private void Zoom(CallbackContext ctx)
    {
        if (Time.timeScale == 0 || camTransition.transitioning)
            return;
        if (zoomedOut)
        {
            camTransition.ZoomIn();
        }
        else
        {
            camTransition.ZoomOut();
            zoomedOut = true;
        }
    }

    private void Pause(CallbackContext ctx)
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = previousTime;
        }
        else
        {
            pauseMenu.SetActive(true);
            previousTime = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    private void Click(CallbackContext ctx)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()));

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero, choiceLayer);
        if (!hit)
            return;
        hit.collider.gameObject.GetComponent<TrackChooser>()?.Clicked();
    }
    void FixedUpdate()
    {
        if (paused)
        {
            if (!previousChosen.continuous)
            {
                trainAudioManager.SpeedUp();
            }
            Vector3 diff = -(nextPoint.transform.position - transform.position);
            diff.Normalize();
            float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            lookRotation = Quaternion.Euler(0, 0, rot - 90);
            velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
            transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, velocity * Time.deltaTime);
            if (!zoomedOut)
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            stopped = false;
            paused = false;
        }
        else if (!stopped && Vector3.Distance(transform.position, nextPoint.transform.position) > 0)
        {
            if (transform.rotation != lookRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            if (Vector3.Distance(transform.position, nextPoint.transform.position) < stoppingDistance
                && !nextPoint.continuous)
            {
                trainAudioManager.SlowDown();
                if (currentVel == Vector3.zero)
                {
                    currentVel = -transform.up * velocity;
                }
                transform.position = Vector3.SmoothDamp(transform.position, nextPoint.transform.position, ref currentVel, 1, maxVelocity);
                velocity = currentVel.magnitude;
                if (Vector3.Distance(transform.position, nextPoint.transform.position) < .1)
                {
                    transform.position = nextPoint.transform.position;
                }
            }
            else
            {
                if (trainAudioManager.state != 0 && trainAudioManager.state != 1)
                    trainAudioManager.ConstantSpeed();
                velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
                transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, velocity*Time.deltaTime);
            }
            if (!zoomedOut)
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
        else if (nextPoint.chosenNext != null)
        {
            if (!nextPoint.continuous)
            {
                velocity = 0;
                currentVel = Vector3.zero;
            }
            previousChosen = nextPoint;
            nextPoint = nextPoint.chosenNext;
            if (!previousChosen.continuous)
            {
                paused = previousChosen.StopAction();
                if (!paused)
                    trainAudioManager.SpeedUp();
            }
            if (!paused)
            {
                Vector3 diff = -(nextPoint.transform.position - transform.position);
                diff.Normalize();
                float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                lookRotation = Quaternion.Euler(0, 0, rot - 90);
                velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
                transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, velocity * Time.deltaTime);
                if (!zoomedOut)
                    Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
                stopped = false;
            }
        }
        else
        {
            velocity = 0;
            currentVel = Vector3.zero;
            stopped = true;
        }
    }

    public void LoadFollowTrains()
    {
        GameManager.Instance.LoadFollowTrain(trainCars);
    }

    public void SaveFollowTrains()
    {
        GameManager.Instance.ClearFollowTrains();
        for (int i = 0; i < trainCars.Count; i++)
        {
            GameManager.Instance.AddFollowTrain(trainCars[i]);
        }
    }

    private void OnDestroy()
    {
        if (controls != null)
            controls.Disable();
    }
}
