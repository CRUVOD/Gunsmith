using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenuObject;
    public GameObject optionsMenuObject;
    public AudioMixer defaultAudioMixer;

    public void SetMasterVolume(float volume)
    {
        defaultAudioMixer.SetFloat("MasterVolume", volume);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }
}
