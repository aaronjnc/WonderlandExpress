using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("General Passenger Stats")]
    [Tooltip("Passenger name")]
    public string passName;
    [Tooltip("Passenger wealth")]
    [Range(0f,1f)]
    public float wealth;
    [Tooltip("Passenger happiness")]
    [Range(0f, 1f)]
    public float happiness;
    [Tooltip("Passenger destination")]
    public Town destination;
    [Header("Passenger lines")]
    [Tooltip("Passenger asking for passage")]
    public string passageMessage;
    [Tooltip("Passenger when passage is accepted")]
    public string acceptMessage;
    [Tooltip("Passenger when passage is denied")]
    public string denyMessage;
    [Tooltip("sprite renderer")]
    public SpriteRenderer sr;
    // Start is called before the first frame update
    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    public void Setup(string pName, float pWealth, float pHappiness, Town pDestination, string pm, string am, string dm)
    {
        passName = pName;
        wealth = pWealth;
        happiness = pHappiness;
        destination = pDestination;
        passageMessage = pm;
        acceptMessage = am;
        denyMessage = dm;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //moves to the given position and sets the rendered to active
    public void Display(Vector3 pos)
    {
        transform.position = pos;
        sr.enabled = true;
    }

    //hides the spriteRenderer
    public void Hide()
    {
        sr.enabled = false;
    }

    //decreases happiness by the given amount
    public void DecreaseHappiness(float diff)
    {
        happiness -= diff;
    }

    //increases happiness by the given amount
    public void IncreaseHappiness(float diff)
    {
        happiness += diff;
    }

    public float GetWealth()
    {
        return wealth;
    }

    public float GetHappiness()
    {
        return happiness;
    }

    public string GetName()
    {
        return passName;
    }

    public Town GetDestination()
    {
        return destination;
    }

    public string GetDestinationName()
    {
        return destination.GetName();
    }
}
