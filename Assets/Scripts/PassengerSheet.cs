using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerSheet : MonoBehaviour
{
    [Tooltip("PassengerOnSheet objects"), SerializeField]
    private PassengerOnSheet[] passengerTexts;
    [Tooltip("Visible position"), SerializeField]
    private float outPosition;
    [Tooltip("Hidden position"), SerializeField]
    private float hiddenPosition;
    private bool moveOut;
    [Tooltip("Speed it slides in/out"), SerializeField]
    private float moveSpeed;
    private Vector2 endPosition;
    private RectTransform t;
    private Passenger[] passengers;
    private void Awake()
    {
        t = GetComponent<RectTransform>();
        passengers = new Passenger[GameManager.Instance.GetPassengerCount()];
        int i = 0;
        foreach(GameObject p in GameManager.Instance.GetPassengers())
        {
            if (p == null)
                continue;
            passengers[i] = p.GetComponent<Passenger>();
            i++;
        }
        Activate();
    }
    public void Activate()
    {
        for (int i = 0; i < passengerTexts.Length; i++)
        {
            if (i < passengers.Length)
                passengerTexts[i].SetPassenger(passengers[i]);
            else
                passengerTexts[i].ClearPassenger();
        }
    }

    public void SlideOut()
    {
        if (t.anchoredPosition.x == outPosition)
        {
            endPosition = new Vector3(hiddenPosition, t.anchoredPosition.y);
        }
        else
        {
            gameObject.SetActive(true);
            endPosition = new Vector3(outPosition, t.anchoredPosition.y);
        }
        moveOut = true;
    }
    private void FixedUpdate()
    {
        if (moveOut)
        {
            t.anchoredPosition = Vector2.MoveTowards(t.anchoredPosition, endPosition, moveSpeed);
            if (Vector2.Distance(endPosition, t.anchoredPosition) < .05)
            {
                moveOut = false;
                t.anchoredPosition = endPosition;
            }
        }
    }
}
