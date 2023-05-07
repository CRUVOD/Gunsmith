using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that triggers a tutorial panel sequence on dialogue end
/// </summary>
public class DialogueTriggerTutorialPanels : DialogueAction
{
    public TutorialPanelTrigger tutorialPanelTrigger;

    public override void Release()
    {
        tutorialPanelTrigger.TriggerTutorialPanels();
    }
}
