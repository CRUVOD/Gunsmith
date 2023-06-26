using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButtonScript : MonoBehaviour, MainMenuButtonAction
{
    public void ButtonAction()
    {
        Application.Quit();
    }
}
