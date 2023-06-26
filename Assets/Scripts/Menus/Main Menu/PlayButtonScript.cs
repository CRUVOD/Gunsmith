using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour, MainMenuButtonAction
{
    public void ButtonAction()
    {
        SceneManager.LoadScene("IntroLevel");
    }

}
