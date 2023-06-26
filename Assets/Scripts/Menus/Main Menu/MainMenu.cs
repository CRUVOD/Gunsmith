using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public MainMenuFacade menuFacade;

    private void Awake()
    {
        
    }

    public void Play()
    {
        menuFacade.Play();
    }

    public void Options()
    {
        menuFacade.Options();
    }

    public void Quit()
    {
        menuFacade.Quit();
    }
}
