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
    [Tooltip("Train whistle sound array"), SerializeField]
    private AudioClip[] trainWhistleSound;
    [Tooltip("Actual train constant sound"), SerializeField]
    private AudioClip actualTrainConstant;
    private static TrainAudioManager _instance;
    PlayerControls controls;
    [HideInInspector]
    public int state = 0;
    private int trainAudioNum = 0;
    [SerializeField, Range(0,1)]
    private float MaxVolume = .8f;
    public static TrainAudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TrainAudioManager>();
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        controls = new PlayerControls();
        controls.ClickEvents.TrainWhistle.performed += Whistle;
        controls.ClickEvents.TrainWhistle.Enable();
        trainAudioSource.loop = true;
        SwitchSound(GameManager.Instance.GetAudioNum());
        trainAudioSource.Play();
    }

    void Whistle(CallbackContext ctx)
    {
        if (trainWhistle.clip != trainWhistleSound[trainAudioNum])
        {
            trainWhistle.clip = trainWhistleSound[trainAudioNum];
        }
        trainWhistle.Play();
    }

    public void UpdateTrainVolume(float percentage)
    {
        trainAudioSource.volume = percentage * MaxVolume;
    }

    public void SpeedUp()
    {
        if (trainAudioNum == 0)
        {
            ActualTrainSound();
            state = 1;
            return;
        }
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
        if (trainAudioNum == 0)
        {
            ActualTrainSound();
            state = 2;
            return;
        }
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
        if (trainAudioNum == 0)
        {
            ActualTrainSound();
            state = 0;
            return;
        }
        if (trainAudioSource.clip != trainRegular)
        {
            trainAudioSource.loop = true;
            trainAudioSource.clip = trainRegular;
            trainAudioSource.Play();
            state = 0;
        }
    }
    private void ActualTrainSound()
    {
        if (trainAudioSource.clip != actualTrainConstant)
        {
            trainAudioSource.clip = actualTrainConstant;
            trainAudioSource.loop = true;
            trainAudioSource.Play();
        }
        else if (!trainAudioSource.isPlaying)
        {
            trainAudioSource.Play();
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
        /*if (trainWhistle.isPlaying)
        {
            trainWhistle.Pause();
        }*/
    }
    public void ResumeSound()
    {
        trainAudioSource.Play();
    }
    public void SwitchSound(int NewSound)
    {
        trainAudioNum = NewSound;
        if (NewSound == 0)
        {
            ActualTrainSound();
            return;
        }
        else
        {
            trainAudioSource.clip = trainRegular;
        }
        /*switch (state)
        {
            case 0:
                trainAudioSource.clip = trainRegular;
                break;
            case 1:
                trainAudioSource.clip = trainAccelerate;
                break;
            case 2:
                trainAudioSource.clip = trainDecelerate;
                break;
        }*/
    }
    private void OnDestroy()
    {
        if (controls != null)
            controls.Dispose();
    }
}
