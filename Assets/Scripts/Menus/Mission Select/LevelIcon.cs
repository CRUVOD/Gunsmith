using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelIcon : MonoBehaviour, IPointerClickHandler
{
    public LevelInfo levelInfo;

    [HideInInspector]
    public LevelInfoDisplayer levelInfoDisplayer;

    public void OnPointerClick(PointerEventData eventData)
    {
        //On click, we update the level info display to this one's level
        if (levelInfo != null)
        {
            levelInfoDisplayer.UpdateLevelDisplay(levelInfo);
        }
    }
}
