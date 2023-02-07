using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    //a struct to hold a single piece of a dialog message
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
    [Tooltip("Whether or not dialogue is actively playing")]
    public bool displayingDialogue = false;
    [Tooltip("Whether the game has ended")]
    public bool gameEnded = false;

    [Header("dialog Lines. probably moved later")]
    [Tooltip("Lines of dialog for toll success")]
    public Dialog[] tollSuccess;
    [Tooltip("Lines of dialog for secondary toll success")]
    public Dialog[] tollSuccess2;
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
    [Header("audio"), SerializeField]
    private TrainAudioManager audioMan;

    [Tooltip("if you want to run the test dialogue")]
    public bool runTest;
    // Start is called before the first frame update
    public async void Awake()
    {
        //if the dialog test is enabled, run the startup dialog test
        if (runTest)
        {
            await DisplayDialog("test");
        }
        HideDisplay();
    }

    public void HideDisplay()
    {
        //continueText.enabled = false;
        //conductorNamePanel.SetActive(false);
        //speakerNamePanel.SetActive(false);
        if (!gameEnded)
        {
            Time.timeScale = 1;
        }
        displayingDialogue = false;
        dialogObject.SetActive(false);
    }

    /*
     * shows dialog corresponding to the given string name
     */
    public async Task DisplayDialog(string path)
    {
        //initialize the dialogue ui
        displayingDialogue = true;
        conductorTalking = true;
        tmo.SetInteract(false);
        ct.SetInteracting(false);
        Debug.Log("display dialog of " + path);
        
        //set up the list of dialog based on the string namepath provided
        Dialog[] currentDialog = (Dialog[])this.GetType().GetField(path).GetValue(this);
        //check for null dialog
        if(currentDialog == null)
        {
            Debug.LogError("DIALOG " + path + " NOT FOUND");
            return;
        }
        //display the first speaker and pass dialog off the handler
        DisplaySpeaker(currentDialog[0].speaker);
        dialogObject.SetActive(true);
        await HandleDialog(currentDialog);

        //hide the dialog ui
        HideDisplay();
        tmo.SetInteract(true);
        ct.SetInteracting(true);
    }

    /*
     * handles the array of dialog items to be displayed
     * lists the text in the text box and handles all special unique interactions
     */
    public async Task HandleDialog(Dialog[] dialogs)
    {
        //enable input 
        canGetInput = true;
        continueText.enabled = false;
        Debug.Log("dialogs to run: " + dialogs.Length);

        //handle each individual piece of dialog
        foreach (Dialog d in dialogs)
        {
            //display the current speaker
            await Task.Delay(100);
            Debug.Log("running dialog");
            DisplaySpeaker(d.speaker);

            //display the current message
            await Task.Delay(100);
            await DisplayText(d.message);

            //handle the different unique interactions
            if(d.uniqueInteraction == "PayToll")
            {
                await GameManager.Instance.TollPass();
            }
            else if (d.uniqueInteraction == "PayTollWL")
            {
                await GameManager.Instance.TollPassWL();
            }
            else if(d.uniqueInteraction == "GameOver")
            {
                Time.timeScale = 0;
                endGame.SetActive(true);
                endGame.GetComponent<GameOverCanvas>().Setup();
                gameEnded = true;
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

    /*
     * displays the portraits of the speakers in dialog
     * highlights the current speaker and greys out whoever they are talking to
     */
    public void DisplaySpeaker(string speaker)
    {
        Debug.Log("displaying: " + speaker);
        //if the conductor is the speaker, highlight him and grey out the target
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
        //else, highlight the speaker and grey out the conductor
        else 
        {
            if (conductorTalking)
            {
                speakerNamePanel.SetActive(true);
                speakerNameText.enabled = true;
                conductorNamePanel.SetActive(false);
                conductorNameText.enabled = false;

                //if the current speaker is not already being displayed, display the curren't speaker's image
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

    /*
     * displays the dialog text through the typewriter script
     */
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

    /*
     * runs whenever receiving input during the dialog scene. 
     * Handles skipping through text and moving on the next set of dialog if input is enabled
     */
    public async void inputReceived()
    {
        if (!canGetInput || !displayingDialogue)
        {
            //Debug.Log("Input Ignored");
            return;
        }
        //Debug.Log("Input received");
        audioMan.UIClick();
        if (tw.IsTyping())
        {
            tw.StopText();
            
        }
        else if(waitingForInput){
            waitingForInput = false;
            continueText.enabled = true;
        }
        canGetInput = false;
        await Task.Delay(100);
        ResetInput();
        //Invoke("ResetInput", .1f);
    }

    /*
     * resets the buffer on input interaction
     */
    public void ResetInput()
    {
        canGetInput = true;
    }
}
