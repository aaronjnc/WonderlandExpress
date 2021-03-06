using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class UIMouseFollow : MonoBehaviour
{
    
    [Tooltip("the z position of this ui")]
    public float zOffset = 0;
    //this ui object's rect transform
    private RectTransform rt;
    //this object's rect
    private Rect rect;
    //this object's canvas group
    private CanvasGroup cg;
    

    // Start is called before the first frame update
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        rect = rt.rect;
        cg = GetComponent<CanvasGroup>();
        MoveToMouse();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToMouse();
    }

    protected void MoveToMouse()
    {
        Vector3 mousePos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0);
        //Debug.Log("mouse pos: " + mousePos);
        transform.position = new Vector3(mousePos.x + rect.width / 2, mousePos.y + rect.height / 2, zOffset);
        //Gizmos.DrawWireSphere(mousePos, 1f);
    }

    public void Enable()
    {
        cg.alpha = 1f;
    }

    public void Disable()
    {
        cg.alpha = 0f;
        gameObject.SetActive(false);
    }
}
