using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TrackChooser : MonoBehaviour
{
    [Tooltip("Chosen track")]
    [SerializeField] private int trackOption = 0;
    [Tooltip("Track point related to this choice")]
    public TrackPoint trackPoint;
    public void Clicked()
    {
        trackPoint.SetChosenTrack(trackOption);
    }
}
