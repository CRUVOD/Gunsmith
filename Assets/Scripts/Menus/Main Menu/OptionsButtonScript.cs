using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButtonScript : MonoBehaviour, MainMenuButtonAction
{
    public GameObject mainMenuObject;
    public GameObject optionsMenuObject;

    public void ButtonAction()
    {
        optionsMenuObject.SetActive(true);
        mainMenuObject.SetActive(false);
    }
}
