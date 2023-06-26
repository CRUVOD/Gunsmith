using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFacade : MonoBehaviour
{
    private PlayButtonScript playButtonScript;
    private OptionsButtonScript optionsButtonScript;
    private QuitButtonScript quitButtonScript;

    private void Awake()
    {
        playButtonScript = GetComponent<PlayButtonScript>();
        optionsButtonScript = GetComponent<OptionsButtonScript>();
        quitButtonScript = GetComponent<QuitButtonScript>();
    }

    public void Play()
    {
        playButtonScript.ButtonAction();
    }

    public void Options()
    {
        optionsButtonScript.ButtonAction();
    }

    public void Quit()
    {
        quitButtonScript.ButtonAction();
    }
}