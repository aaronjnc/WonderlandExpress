using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private void Start()
    {
        _instance = this;
        if (GameManager.Instance.load)
        {
            nextPoint = GameObject.Find(GameManager.Instance.GetCurrentStop()).GetComponent<TrackPoint>().chosenNext;
            transform.position = GameManager.Instance.getTrainPosition();
        }
        controls = new PlayerControls();
        controls.Menu.Pause.performed += Pause;
        controls.Menu.Pause.Enable();
    }
    private void Pause(CallbackContext ctx)
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
    void FixedUpdate()
    {
        if (!stopped && Vector3.Distance(transform.position, nextPoint.transform.position) > 0)
        {
            if (Vector3.Distance(transform.position, nextPoint.transform.position) < stoppingDistance
                && !nextPoint.continuous)
            {
                if (currentVel == Vector3.zero)
                {
                    currentVel = transform.right * velocity;
                }
                transform.position = Vector3.SmoothDamp(transform.position, nextPoint.transform.position, ref currentVel, 1, maxVelocity);
                if (Vector3.Distance(transform.position, nextPoint.transform.position) < .1)
                {
                    transform.position = nextPoint.transform.position;
                }
            }
            else
            {
                velocity = Mathf.Clamp(velocity + Time.deltaTime * acceleration, 0, maxVelocity);
                transform.position = Vector3.MoveTowards(transform.position, nextPoint.transform.position, velocity*Time.deltaTime);
            }
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
        else if (nextPoint.chosenNext != null)
        {
            if (!nextPoint.continuous)
            {
                velocity = 0;
                currentVel = Vector3.zero;
            }
            TrackPoint previousChosen = nextPoint;
            nextPoint = nextPoint.chosenNext;
            if (!previousChosen.continuous)
            {
                previousChosen.StopAction();
            }
            transform.right = nextPoint.transform.position - transform.position;
            stopped = false;
        }
        else
        {
            stopped = true;
        }
    }
}
