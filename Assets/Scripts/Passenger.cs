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
    [Tooltip("Passenger when dropped off")]
    public string dropOffMessage;
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
    [Header("Trait Settings")]
    [Tooltip("The trait that this passenger has. Dictates certain things they can do")]
    public string trait = "None";
    [Tooltip("Description of the passenger's trait")]
    public string traitDescription = "";
    [Tooltip("The amount that money is multiplied by if the passenger is shady")]
    public int shadyGoldMod = 3;
    //reference to the town UI manager
    //private TownUIManager uiMan;
    // Start is called before the first frame update
    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.enabled = false;
        //uiMan = TownUIManager.GetManager();
    }

    public void Setup(string fName, string lName, int pGold, float pWealth, float pHappiness, string pDestination, string pm, string am, string dm, string dom)
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
        dropOffMessage = dom;
    }

    public void Setup(string pName, int pGold, float pWealth, float pHappiness, string pDestination, string pm, string am, string dm, string dom)
    {
        passName = pName;
        gold = pGold;
        wealth = pWealth;
        happiness = pHappiness;
        destination = pDestination;
        passageMessage = pm;
        acceptMessage = am;
        denyMessage = dm;
        dropOffMessage = dom;
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

    public void SetLines(string pm, string am, string dm, string dom)
    {
        passageMessage = pm;
        acceptMessage = am;
        denyMessage = dm;
        dropOffMessage = dom;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTrait(string pTrait, string description)
    {
        trait = pTrait;
        traitDescription = description;
    }

    public string GetTrait()
    {
        return trait;
    }

    public string GetTraitDescription()
    {
        return traitDescription;
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
        if(happiness <= 0f)
        {
            happiness = 0f;
        }
        if (happiness >= 1f)
        {
            happiness = 0f;
        }
    }

    //increases happiness by the given amount
    public void IncreaseHappiness(float diff)
    {
        happiness += diff;
        if (happiness <= 0f)
        {
            happiness = 0f;
        }
        if (happiness >= 1f)
        {
            happiness = 0f;
        }
    }

    public int GetGold()
    {
        if (trait == "Shady")
        {
            return gold * shadyGoldMod;
        }
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

    public string GetDropOff()
    {
        return dropOffMessage;
    }

    public void DropOff(Town t)
    {
        if (trait == "Shady")
        {
            t.DestroyTown();
        }
    }

    //reduces happiness as train moves. decrease = amount happiness typically drops by, numPassengers = number of passengers
    public void OnTrainMove(float decrease, int numPassengers)
    {
        switch (trait)
        {
            case "Carefree":
                decrease = 0f;
                break;

            case "Antisocial":
                decrease *= (float)(numPassengers - 2);
                break;

            case "Social":
                decrease *= (float)(3 - numPassengers);
                break;

            case "Irritable":
                decrease *= 2;
                break;

            default:
                break;
        }
        
        DecreaseHappiness(decrease);
    

    }

    //move to the designated location. trown: false-> bounce multiple times, true -> bounce once and much higher
    public async Task MoveTo(Vector3 pos, bool thrown)
    {
        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        //determine stats for the bouncing step animation.
        float bounceHeight = 4.5f;
        float moveSpeed = (float)speed;
        if (thrown)
        {
            //pos -= new Vector3(0, (bc.bounds.center.y - transform.position.y), 0);
            moveSpeed *= 3.0f;
        }
        if (!thrown)
        {
            bounceHeight = UnityEngine.Random.Range(0.2f, 0.5f);
        }
        float distance = 0;
        float totalDist = (pos - transform.position).magnitude;
        int bounceCount = 1;
        if (!thrown)
        {
            bounceCount = (int)(totalDist / bounceDist);
        }
        

        float period = totalDist / (float)bounceCount;
        //Debug.Log("total distance: " + totalDist + "\nbounce count: " + bounceCount + "\nperiod: " + period);
        //Debug.Log("bounce height: " + bounceHeight);
        float startTime = Time.time;
        int startFrame = Time.frameCount;
        Debug.Log("start time: " + startTime + " at deltatime " + Time.deltaTime + " on frame " + startFrame);
        int counter = 0;

        

        Vector3 travelPos = transform.position;
        //Debug.Log("start movement: " + speed);
        while(travelPos != pos)
        {
            //Debug.Log((pos - travelPos).magnitude);
            if((pos - travelPos).magnitude <= moveSpeed)
            {
                Debug.Log("almost done");
                travelPos = pos;
                transform.position = pos;
                transform.eulerAngles = Vector3.zero;

                float endTime = Time.time;
                int endFrame = Time.frameCount;
                float timeDiff = endTime - startTime;
                int frameDiff = endFrame - startFrame;
                Debug.Log("Done at time " + endTime + " with time elapsed: " + timeDiff + ", at frame " + endFrame + " with frames elapsed: " + frameDiff + " and at count " + counter);
                Vector3 centerOfRotation = transform.position + transform.up.normalized * (sprite.bounds.extents.y / 4);// sprite.bounds.center;
                Debug.Log("transform: " + transform.position + " sprite center: " + centerOfRotation);
                Debug.DrawRay(centerOfRotation, transform.position - centerOfRotation, Color.red, 1f);

            }
            else
            {
                //Debug.Log("moving " + ((pos - transform.position).normalized).magnitude + ", " + speed);
                counter++;

                travelPos += (pos - travelPos).normalized * moveSpeed;

                distance += moveSpeed;
                float bounceOffset = bounceHeight / 2 * -Mathf.Cos(((2 * Mathf.PI) / period) * distance) + bounceHeight / 2;
                Vector3 bounce = new Vector3(0, bounceOffset, 0);
                float angleOffset;
                if (thrown)
                {
                    angleOffset = (360 / totalDist) * moveSpeed;
                    Vector3 centerOfRotation = transform.position + transform.up.normalized * (sprite.bounds.extents.y/4);// sprite.bounds.center;
                    transform.RotateAround(centerOfRotation, Vector3.forward, angleOffset);
                    Vector3 diff = (centerOfRotation - transform.position) * -Mathf.Cos(((2 * Mathf.PI) / period) * distance) + (centerOfRotation - transform.position);
                    bounce -= diff;
                    //Debug.DrawRay(centerOfRotation, transform.position - centerOfRotation, Color.red, .5f);
                }
                else
                {
                    angleOffset = -tiltAngle * Mathf.Sin(((2 * Mathf.PI) / (period * 2)) * distance);
                    transform.eulerAngles = new Vector3(0f, 0f, angleOffset);
                }


                transform.position = travelPos + bounce;

                
                
                //Debug.DrawRay(travelPos, pos, Color.red, .1f);
                //Debug.DrawRay(transform.position, travelPos, Color.green, .1f);
            }
            await Task.Yield();
        }

    }
}
