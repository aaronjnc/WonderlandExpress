using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TrackChooser : MonoBehaviour
{
    PlayerControls controls;
    [HideInInspector]
    [Tooltip("Chosen track")]
    public int trackOption = 0;
    [Tooltip("Track point related to this choice")]
    public TrackPoint trackPoint;
    private void Start()
    {
        controls = new PlayerControls();
        controls.ClickEvents.Click.performed += Click;
        controls.ClickEvents.Click.Enable();
    }
    private void Click(CallbackContext ctx)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()));

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);
        if (!hit)
            return;
        if (hit.collider.gameObject == this.gameObject)
        {
            trackPoint.SetChosenTrack(trackOption);
        }
    }
    private void OnDestroy()
    {
        if (controls != null)
            controls.Disable();
    }
}
