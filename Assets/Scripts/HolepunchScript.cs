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
    private bool displayed = false;
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
    private bool overButton = false;
    public AudioSource audio;
    private void Start()
    {
        openTop = holepunchTopImage.sprite;
        openBottom = holepunchBottomImage.sprite;
        DisplayPunch(false);
        controls = new PlayerControls();
        controls.ClickEvents.Click.performed += ClickEvent;
        controls.ClickEvents.Click.Enable();
    }

    private void ClickEvent(CallbackContext ctx)
    {
        if (displayed && !punched)
        {
            holepunchTopImage.sprite = closedTop;
            holepunchBottomImage.sprite = closedBottom;
            punched = true;
            audio.Play();
            StartCoroutine("ClickWait");
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (!punched)
        {

            Vector3 mousePos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -1f);//Camera.main.ScreenToViewportPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), -1f));
            //Debug.Log(mousePos);
            //mousePos = new Vector3(mousePos.x, mousePos.y, -1);
            //Vector3 cameraPos = Camera.main.transform.position;
            Vector3 cameraPos = new Vector3(mousePos.x, mousePos.y, 10f);
            //Debug.Log("mouse: " + mousePos);
            RaycastHit2D hit = Physics2D.Raycast(cameraPos, (mousePos - cameraPos) * 100, Mathf.Infinity);
            //Debug.DrawRay(cameraPos, (mousePos - cameraPos) * 100, Color.red, 0.1f);
            if (hit.collider != null)
            {
                //Debug.Log("Hit " + hit.collider.gameObject);
                GameObject obj = hit.collider.gameObject;
                Button button = obj.GetComponent<Button>();
                if (button != null)
                {
                    if (!overButton)
                    {
                        overButton = true;
                        DisplayPunch(true);
                    }
                }
                else
                {
                    if (overButton)
                    {
                        overButton = false;
                        DisplayPunch(false);
                    }
                }
            }
            else
            {
                if (overButton)
                {
                    overButton = false;
                    DisplayPunch(false);
                }
            }
        }

        if (!punched && displayed)
        {
            holepunchTop.position = new Vector3(holepunchTop.position.x, Mouse.current.position.y.ReadValue() - 80, holepunchTop.position.z);
            holepunchBottom.position = new Vector3(holepunchBottom.position.x, Mouse.current.position.y.ReadValue() - 80, holepunchBottom.position.z);
        }
    }
    IEnumerator ClickWait()
    {
        yield return new WaitForSeconds(punchTime);
        punched = false;
        holepunchTopImage.sprite = openTop;
        holepunchBottomImage.sprite = openBottom;
    }

    //shows the holepunch
    public void DisplayPunch(bool display)
    {
        displayed = display;
        holepunchBottomImage.enabled = display;
        holepunchTopImage.enabled = display;
    }
}
