using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers a tutorial panel sequence on player enter
/// </summary>
public class TutorialPanelPlayerEnter : MonoBehaviour
{
    private bool hasBeenTriggered;
    public TutorialPanelTrigger tutorialPanelTrigger;
    private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            tutorialPanelTrigger.TriggerTutorialPanels();
        }
    }
}
