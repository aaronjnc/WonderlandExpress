using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class Passenger : MonoBehaviour
{
    [Header("General Passenger Stats")]
    [Tooltip("Passenger first name")]
    public string firstName;
    [Tooltip("Passenger last name")]
    public string lastName;
    [Tooltip("Passenger name")]
    public string passName;
    [Tooltip("The amount of gold a passenger will pay if you successfully transport them.")]
    public int gold;
    [Tooltip("The passenger's wealth, relative to their town. Used for determining wealth tier (not used yet)")]
    public float wealth;
    [Tooltip("Passenger happiness")]
    [Range(0f, 1f)]
    public float happiness;
    [Tooltip("Passenger destination")]
    public string destination;
    [Header("Passenger lines")]
    [Tooltip("Passenger asking for passage")]
    public string passageMessage;
    [Tooltip("Passenger when passage is accepted")]
    public string acceptMessage;
    [Tooltip("Passenger when passage is denied")]
    public string denyMessage;
    [Tooltip("sprite renderer")]
    public SpriteRenderer sr;
    [Header("Movement Stats")]
    [Tooltip("speed at which the passenger moves")]
    public double speed = .05;
    [Tooltip("max distance for any given bounce")]
    public float bounceDist = .4f;
    //[Tooltip("Height of each bounce")]
    //public float bounceHeight = .1f;
    [Tooltip("how far to the side they can tilt on each bounce")]
    public float tiltAngle = 30f;

    //reference to the town UI manager
    //private TownUIManager uiMan;
    // Start is called before the first frame update
    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
        //uiMan = TownUIManager.GetManager();
    }

    public void Setup(string fName, string lName, int pGold, float pWealth, float pHappiness, string pDestination, string pm, string am, string dm)
    {
        firstName = fName;
        lastName = lName;
        passName = firstName + " " + lastName;
        gold = pGold;
        wealth = pWealth;
        happiness = pHappiness;
        destination = pDestination;
        passageMessage = pm;
        acceptMessage = am;
        denyMessage = dm;
    }

    public void Setup(string pName, int pGold, float pWealth, float pHappiness, string pDestination, string pm, string am, string dm)
    {
        passName = pName;
        gold = pGold;
        wealth = pWealth;
        happiness = pHappiness;
        destination = pDestination;
        passageMessage = pm;
        acceptMessage = am;
        denyMessage = dm;
    }

    public void Setup(int pGold, float pWealth, float pHappiness, string pDestination)
    {
        gold = pGold;
        wealth = pWealth;
        happiness = pHappiness;
        destination = pDestination;
    }

    public void SetName(string first, string last)
    {
        firstName = first;
        lastName = last;
        passName = first + " " + last;
    }

    public void SetLines(string pm, string am, string dm)
    {
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

    public int GetGold()
    {
        return gold;
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

    public string GetFirst()
    {
        return firstName;
    }

    public string GetLast()
    {
        return lastName;
    }

    public string GetDestination()
    {
        return destination;
    }

    //public string GetDestinationName()
    //{
    //    return destination.GetName();
    //}

    public string GetMessage()
    {
        return passageMessage;
    }

    public string GetAccept()
    {
        return acceptMessage;
    }

    public string GetDeny()
    {
        return denyMessage;
    }

    //move to the designated location
    public async Task MoveTo(Vector3 pos)
    {
        //determine stats for the bouncing step animation.
        float bounceHeight = UnityEngine.Random.Range(0.2f, 0.5f);
        float distance = 0;
        float totalDist = (pos - transform.position).magnitude;
        int bounceCount = (int)(totalDist / bounceDist);
        float period = totalDist / (float)bounceCount;
        Debug.Log("total distance: " + totalDist + "\nbounce count: " + bounceCount + "\nperiod: " + period);
        Debug.Log("bounce height: " + bounceHeight);


        Vector3 travelPos = transform.position;
        Debug.Log("start movement: " + speed);
        while(travelPos != pos)
        {
            //Debug.Log((pos - travelPos).magnitude);
            if((pos - travelPos).magnitude <= speed)
            {
                Debug.Log("almost done");
                travelPos = pos;
                transform.position = pos;
                transform.eulerAngles = Vector3.zero;
            }
            else
            {
                //Debug.Log("moving " + ((pos - transform.position).normalized).magnitude + ", " + speed);
                

                travelPos += (pos - travelPos).normalized * (float)speed;

                distance += (float)speed;
                float bounceOffset = bounceHeight / 2 * -Mathf.Cos(((2 * Mathf.PI) / period) * distance) + bounceHeight / 2;
                float angleOffset = tiltAngle * Mathf.Sin(((2 * Mathf.PI) / (period * 2)) * distance); 

                transform.position = travelPos + new Vector3(0, bounceOffset, 0);

                transform.eulerAngles = new Vector3(0f, 0f, angleOffset);
                //Debug.DrawRay(travelPos, pos, Color.red, .1f);
                //Debug.DrawRay(transform.position, travelPos, Color.green, .1f);
            }
            await Task.Yield();
        }
        

    }
}
