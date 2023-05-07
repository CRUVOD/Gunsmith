using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script manages the tutorial panel that appears on the right side 
/// of the screen
/// </summary>
public class TutorialPanelSystem : MonoBehaviour
{
    public static TutorialPanelSystem instance;

    public TutorialPanel tutorialPanel;

    [Tooltip("Time to wait for the animation of opening and closing the tutorial panel to play in between subsqeunt tutorial images")]
    public float openCloseTimeCompensation;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void StartTutorialPanelSequence(Sprite[] images, float timeGivenToEachPanel)
    {
        if (images.Length == 0)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(PlayTutorialPanelSequence(images, timeGivenToEachPanel));
    }

    private IEnumerator PlayTutorialPanelSequence(Sprite[] images, float timeGivenToEachPanel)
    {
        foreach (Sprite image in images)
        {
            tutorialPanel.Show();
            tutorialPanel.SetImage(image);
            yield return new WaitForSeconds(timeGivenToEachPanel);
            tutorialPanel.Hide();
            yield return new WaitForSeconds(openCloseTimeCompensation);
        }
    }
}
