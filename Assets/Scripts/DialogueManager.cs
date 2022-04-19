using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct Dialog 
    {
        public string speaker;
        public string message;
        public string uniqueInteraction;
    }

    [Header("dialog Objects")]
    [Tooltip("the overarching game object for dialog windows. used to turn on/off display")]
    public GameObject dialogObject;
    [Tooltip("Text box for dialog")]
    public TextMeshProUGUI dialogText;
    [Tooltip("Text box to say to press button to continue")]
    public TextMeshProUGUI continueText;
    [Space]
    [Tooltip("Text box for speaker name")]
    public TextMeshProUGUI speakerNameText;
    [Tooltip("Panel for speaker name")]
    public GameObject speakerNamePanel;
    [Tooltip("Image for speaker")]
    public Image speakerImage;
    [Space]
    [Tooltip("Text box for conductor name")]
    public TextMeshProUGUI conductorNameText;
    [Tooltip("Panel for conductor name")]
    public GameObject conductorNamePanel;
    [Tooltip("Image for conductor")]
    public Image conductorImage;
    [Space]
    [Tooltip("The typewriter script")]
    public Typewriter tw;
    [Tooltip("The camera script, to disable zooms while running")]
    public CameraTransition ct;
    [Tooltip("The town mouseover script, to disable mouseover ui while running")]
    public TownMouseOverUI tmo;
    [Space]
    [Tooltip("Passenger train. Used for throwing passengers")]
    public GameObject passTrain;
    [Tooltip("Jabberwocky head. Used to determine where to throw passengers when being eaten")]
    public GameObject jwHead;
    [Tooltip("End game canvas obect"), SerializeField]
    private GameObject endGame;

    [Header("dialog stats")]
    [Tooltip("Rate at which letters are typed in characters per second")]
    public float charPerSec = 0.3f;
    [Tooltip("If the conductor is the one currently talking")]
    public bool conductorTalking = true;
    [Tooltip("The current speaker")]
    public string currentSpeaker = string.Empty;
    [Tooltip("Color for current speaker")]
    public Color currentSpeakerColor;
    [Tooltip("Color for waiting speaker")]
    public Color waitingSpeakerColor;
    [Tooltip("Whether it's waiting for current input or not")]
    public bool waitingForInput = false;
    [Tooltip("Whether or not it can take input currently. Used to prevent one button press from hitting multiple times")]
    public bool canGetInput = true;

    [Header("dialog Lines. probably moved later")]
    [Tooltip("Lines of dialog for toll success")]
    public Dialog[] tollSuccess;
    //[Tooltip("associated speakers for toll success")]
    //public List<string> tollSuccessSpeakers = new List<string>();
    //[Space]
    [Tooltip("Lines of Dialog for toll failure")]
    public Dialog[] tollFailure;
    //[Tooltip("associated speakers for toll failure")]
    //public List<string> tollFailureSpeakers = new List<string>();
    //[Space]
    [Tooltip("Lines of Dialog for Jabberwocky success")]
    public Dialog[] jwSuccess;
    //[Tooltip("associated speakers for Jabberwocky success")]
    //public List<string> jwSuccessSpeakers = new List<string>();
    //[Space]
    [Tooltip("Lines of Dialog for Jabberwocky failure")]
    public Dialog[] jwFailure;
    //[Tooltip("associated speakers for Jabberwocky failure")]
    //public List<string> jwFailureSpeakers = new List<string>();
    //[Space]
    [Tooltip("Lines of Dialog for Jabberwocky game over")]
    public Dialog[] jwGameOver;
    [Tooltip("Lines of Dialog for testing")]
    public Dialog[] test;
    //[Tooltip("associated speakers for Jabberwocky game over")]
    //public List<string> jwGameOverSpeakers = new List<string>();

    // Start is called before the first frame update
    public void Awake()
    {
        //DisplayDialog("test");
        HideDisplay();
    }

    public void HideDisplay()
    {
        Time.timeScale = 1;
        dialogObject.SetActive(false);
    }

    //shows dialog corresponding to the given string
    public async Task DisplayDialog(string path)
    {
        Time.timeScale = 0;
        conductorTalking = true;
        tmo.SetInteract(false);
        ct.SetInteracting(false);
        Debug.Log("display dialog of " + path);
        dialogObject.SetActive(true);
        Dialog[] currentDialog = (Dialog[])this.GetType().GetField(path).GetValue(this);
        await HandleDialog(currentDialog);
        HideDisplay();
        tmo.SetInteract(true);
        ct.SetInteracting(true);
    }

    public async Task HandleDialog(Dialog[] dialogs)
    {
        canGetInput = true;
        continueText.enabled = false;
        Debug.Log("dialogs to run: " + dialogs.Length);
        foreach (Dialog d in dialogs)
        {
            await Task.Delay(100);
            Debug.Log("running dialog");
            DisplaySpeaker(d.speaker);
            await Task.Delay(100);
            await DisplayText(d.message);
            if(d.uniqueInteraction == "PayToll")
            {
                await GameManager.Instance.TollPass();
            }
            else if(d.uniqueInteraction == "GameOver")
            {
                Time.timeScale = 0;
                endGame.SetActive(true);
            }
            else if(d.uniqueInteraction == "PayJW")
            {
                await GameManager.Instance.PayJabberwocky();
                Debug.Log("You survived");
            }
            else if(d.uniqueInteraction == "FailJW")
            {
                int random = Random.Range(0, GameManager.Instance.GetPassengerCount());
                await GameManager.Instance.EatPassenger(random, passTrain, jwHead);
            }
        }
        Debug.Log("all dialogs complete");

        
    }

    public void DisplaySpeaker(string speaker)
    {
        Debug.Log("displaying: " + speaker);
        if(speaker == "Conductor")
        {
            if (!conductorTalking)
            {
                speakerImage.color = waitingSpeakerColor;
                conductorImage.color = currentSpeakerColor;
                conductorNamePanel.SetActive(true);
                conductorNameText.enabled = true;
                conductorNameText.text = "Conductor";
                speakerNamePanel.SetActive(false);
                speakerNameText.enabled = false;
                conductorTalking = true;
            }
        }
        else 
        {
            if (conductorTalking)
            {
                speakerNamePanel.SetActive(true);
                speakerNameText.enabled = true;
                conductorNamePanel.SetActive(false);
                conductorNameText.enabled = false;
                if (currentSpeaker != speaker)
                {
                    speakerImage.sprite = Resources.Load<Sprite>("Character Sprites/" + speaker);
                    speakerNameText.text = speaker;
                }
                conductorImage.color = waitingSpeakerColor;
                speakerImage.color = currentSpeakerColor;
                conductorTalking = false;
            }
            
        }
    }

    public async Task DisplayText(string text)
    {
        continueText.enabled = false;
        await tw.Typewrite(dialogText, text, charPerSec);
        continueText.enabled = true;
        waitingForInput = true;
        while (waitingForInput)
        {
            await Task.Yield();
        }
        
    }

    public void inputReceived()
    {
        if (!canGetInput)
        {
            //Debug.Log("Input Ignored");
            return;
        }
        //Debug.Log("Input received");
        if (tw.IsTyping())
        {
            tw.StopText();
            
        }
        else if(waitingForInput){
            waitingForInput = false;
        }
        canGetInput = false;
        Invoke("ResetInput", .1f);
    }

    public void ResetInput()
    {
        canGetInput = true;
    }
}
