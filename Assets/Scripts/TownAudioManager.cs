using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("Audio Source for UI")]
    public AudioSource uiSource;
    [Tooltip("Audio Source for background noise")]
    public AudioSource backgroundSource;

    [Header("Audio strings with format of Audio/[name]_click")]
    [Tooltip("string for accept")]
    public string acceptAudioString = "yes";

    [Tooltip("string for deny")]
    public string denyAudioString = "no";

    [Tooltip("string for remove")]
    public string removeAudioString = "remove";

    [Tooltip("string for click")]
    public string selectAudioString = "select";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcceptAudio()
    {
        PlayUIAudio(acceptAudioString);
    }

    public void DenyAudio()
    {
        PlayUIAudio(denyAudioString);
    }

    public void RemoveAudio()
    {
        PlayUIAudio(removeAudioString);
    }

    public void SelectAudio()
    {
        PlayUIAudio(selectAudioString);
    }

    public void PlayUIAudio(string name)
    {
        var uiAudio = Resources.Load<AudioClip>("Audio/" + name + "_click");
        if(uiAudio != null)
        {
            uiSource.clip = uiAudio;
            uiSource.Play();
        }
        else
        {
            Debug.LogWarning("Audio can't be found");
        }
    }

    public void PlayBGAudio(string name)
    {
        var uiAudio = Resources.Load<AudioClip>("Audio/town_background_" + name);
        if (uiAudio != null)
        {
            backgroundSource.clip = uiAudio;
            backgroundSource.Play();
            backgroundSource.loop = true;
        }
        else
        {
            Debug.LogWarning("Audio can't be found");
        }
    }

    public void PlayBGAudio(int i)
    {
        PlayBGAudio(i.ToString());
    }
}
