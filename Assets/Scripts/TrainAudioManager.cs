using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class TrainAudioManager : MonoBehaviour
{
    [Tooltip("The audio source used for train audio"), SerializeField]
    private AudioSource trainAudioSource;
    [Tooltip("Audio source for train whistle"), SerializeField]
    private AudioSource trainWhistle;
    [Tooltip("UI audio source"), SerializeField]
    private AudioSource uiAudio;
    [Tooltip("Speed up audio clip"), SerializeField]
    private AudioClip trainAccelerate;
    [Tooltip("Slow down audio clip"), SerializeField]
    private AudioClip trainDecelerate;
    [Tooltip("Train max speed audio clip"), SerializeField]
    private AudioClip trainRegular;
    PlayerControls controls;
    [HideInInspector]
    public int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerControls();
        controls.ClickEvents.TrainWhistle.performed += Whistle;
        controls.ClickEvents.TrainWhistle.Enable();
    }

    void Whistle(CallbackContext ctx)
    {
        trainWhistle.Play();
    }

    public void SpeedUp()
    {
        if (trainAudioSource.clip != trainAccelerate)
        {
            trainAudioSource.loop = false;
            trainAudioSource.clip = trainAccelerate;
            trainAudioSource.Play();
            state = 1;
            StartCoroutine("playEngineSound");
        }
    }

    public void SlowDown()
    {
        if (trainAudioSource.clip != trainDecelerate)
        {
            trainAudioSource.loop = false;
            trainAudioSource.clip = trainDecelerate;
            trainAudioSource.Play();
            state = 2;
        }
    }
    public void ConstantSpeed()
    {
        if (trainAudioSource.clip != trainRegular)
        {
            trainAudioSource.loop = true;
            trainAudioSource.clip = trainRegular;
            trainAudioSource.Play();
            state = 0;
        }
    }
    IEnumerator playEngineSound()
    {
        yield return new WaitForSeconds(trainAudioSource.clip.length);
        ConstantSpeed();
    }
    public void UIClick()
    {
        uiAudio.Play();
    }

    public void StopSound()
    {
        trainAudioSource.Pause();
        if (trainWhistle.isPlaying)
        {
            trainWhistle.Pause();
        }
    }
    public void ResumeSound()
    {
        trainAudioSource.Play();
    }
    private void OnDestroy()
    {
        if (controls != null)
            controls.Dispose();
    }
}
