using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanelTrigger : MonoBehaviour
{
    public Sprite[] tutorialImages;

    public float timeGivenToEachPanel;

    public void TriggerTutorialPanels()
    {
        TutorialPanelSystem.instance.StartTutorialPanelSequence(tutorialImages, timeGivenToEachPanel);
    }
}
