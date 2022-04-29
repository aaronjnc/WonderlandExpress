using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class HolepunchScript : MonoBehaviour
{
    [SerializeField]
    private Transform holepunchTop;
    [SerializeField]
    private Transform holepunchBottom;
    private bool punched = false;
    [SerializeField]
    private float punchTime;
    [SerializeField]
    private Image holepunchTopImage;
    [SerializeField]
    private Image holepunchBottomImage;
    private Sprite openTop;
    private Sprite openBottom;
    [SerializeField]
    private Sprite closedTop;
    [SerializeField]
    private Sprite closedBottom;
    PlayerControls controls;
    private void Start()
    {
        openTop = holepunchTopImage.sprite;
        openBottom = holepunchBottomImage.sprite;
        controls = new PlayerControls();
        controls.ClickEvents.Click.performed += ClickEvent;
        controls.ClickEvents.Click.Enable();
    }

    private void ClickEvent(CallbackContext ctx)
    {
        holepunchTopImage.sprite = closedTop;
        holepunchBottomImage.sprite = closedBottom;
        punched = true;
        StartCoroutine("ClickWait");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!punched)
        {
            holepunchTop.position = new Vector3(holepunchTop.position.x, Mouse.current.position.y.ReadValue()-80, holepunchTop.position.z);
            holepunchBottom.position = new Vector3(holepunchBottom.position.x, Mouse.current.position.y.ReadValue()-80, holepunchBottom.position.z);
        }
    }
    IEnumerator ClickWait()
    {
        yield return new WaitForSeconds(punchTime);
        punched = false;
        holepunchTopImage.sprite = openTop;
        holepunchBottomImage.sprite = openBottom;
    }
}
