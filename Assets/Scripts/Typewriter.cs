using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class Typewriter : MonoBehaviour
{
    
    [Tooltip("Background color, used for hiding text")]
    public Color bgColor = Color.grey;
    //the string for starting the background color
    string colorString;
    //the string for ending the background color
    string colorEnd = "</color>";
    //the stringbuilder to use
    StringBuilder sb;
    //used to tell task to stop
    bool stop = false;
    //whether the typewriter is currently typing or not
    bool typing = false;
    //program framerate
    int fps = 30;
    // Start is called before the first frame update
    void Awake()
    {
        colorString = "<color=#" + ColorUtility.ToHtmlStringRGBA(bgColor) + ">";
        sb = new StringBuilder();
        fps = Application.targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsTyping()
    {
        return typing;
    }

    public void StopText()
    {
        stop = true;
    }

    //write text as typewriter
    public async Task Typewrite(TextMeshProUGUI textBox, string text, float charPerSec)
    {
        textBox.text = string.Empty;
        stop = false;
        typing = true;
        int count = 0;
        sb.Clear();
        while(count < text.Length)
        {
            if (stop)
            {
                stop = false;
                break;
            }
            sb.Clear();
            sb.Append(text.Substring(0, count + 1));
            sb.Append(colorString);
            sb.Append(text.Substring(count));
            sb.Append(colorEnd);
            textBox.text = sb.ToString();
            await Task.Delay((int)(1 / charPerSec));
            await Task.Yield();
            count++;
        }
        textBox.text = text;
        typing = false;
    }

    //clear text
    public void Clear(Text textBox)
    {
        textBox.text = string.Empty;
    }
}
