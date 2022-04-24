using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;


public class TipMove : MonoBehaviour
{
    [Tooltip("starting position")]
    private Vector3 startPos;
    [Tooltip("initial moving velocity for the text")]
    public Vector3 initSpeed;
    //[Tooltip("rate at which alpha decreases per frame")]
    //public float alphaDecRate;
    [Tooltip("this object's canvas group")]
    private CanvasGroup cg;
    [Tooltip("this object's TextMeshPro component")]
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        startPos = transform.position;
        cg = GetComponent<CanvasGroup>();
    }

    /**
     * makes the text float upwards for the time specified displaying the specified tip percentage
     * @param time the amount of time in milliseconds
     * @param tip the tip percentage to display
     */
    public async Task floatMove(float time, float tip)
    {
        transform.position = startPos;
        cg.alpha = 1;
        text.text = (int)(tip * 100f) + " % tip";
        //gameObject.SetActive(true);
        float currentTime = 0f;
        float perFrameIncrease = 1000f / Application.targetFrameRate;
        while(cg.alpha > 0f)
        {
            float percentComplete = Mathf.Clamp(currentTime / time, 0f, 1f);
            transform.position = transform.position + initSpeed * Mathf.Lerp(1f, 0f, percentComplete);
            cg.alpha = Mathf.Clamp(1f - percentComplete, 0f, 1f);
            currentTime += perFrameIncrease;
            await Task.Yield();
        }
        cg.alpha = 0f;
        //gameObject.SetActive(false);
        
    }
}
