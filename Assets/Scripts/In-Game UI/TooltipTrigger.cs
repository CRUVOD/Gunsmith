using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;

    public string header;
    public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(content))
        {
            return;
        }

        delay = LeanTween.delayedCall(0.5f, () =>
        {
            TooltipSystem.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (delay!= null)
        {
            LeanTween.cancel(delay.uniqueId);
            TooltipSystem.Hide();
        }
    }
}