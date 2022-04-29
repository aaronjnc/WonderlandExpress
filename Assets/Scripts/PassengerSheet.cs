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
    private Passenger[][] passengers;
    private int currentPage = 0;
    [Tooltip("Previous page"), SerializeField]
    private GameObject previous;
    [Tooltip("Next page"), SerializeField]
    private GameObject next;
    private void Awake()
    {
        t = GetComponent<RectTransform>();
        UpdatePassengers();
        ChangePage(0);
    }
    public void UpdatePassengers()
    {
        if (GameManager.Instance.GetPassengerCount() == 0)
        {
            passengers = new Passenger[0][];
            Activate();
            return;
        }
        passengers = new Passenger[((GameManager.Instance.GetPassengerCount() - 1) / 5) + 1][];
        int i = 0;
        foreach (GameObject p in GameManager.Instance.GetPassengers())
        {
            if (p == null)
                continue;
            if (passengers[i / 5] == null)
            {
                passengers[i / 5] = new Passenger[5];
            }
            passengers[i / 5][i % 5] = p.GetComponent<Passenger>();
            i++;
        }
        Activate();
    }
    public void Activate()
    {
        for (int i = 0; i < passengerTexts.Length; i++)
        {
            if (currentPage < passengers.Length && i < passengers[currentPage].Length)
                passengerTexts[i].SetPassenger(passengers[currentPage][i]);
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
    public void ChangePage(int i)
    {
        if (currentPage + i < 0 || currentPage + i >= passengers.Length)
            return;
        currentPage += i;
        if (currentPage > 0)
        {
            previous.SetActive(true);
        }
        else
        {
            previous.SetActive(false);
        }
        next.SetActive(currentPage < passengers.Length - 1);
        Activate();
    }
}
