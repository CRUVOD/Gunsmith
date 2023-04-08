using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;

    public int characterWrapLimit;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;

        //rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = mousePos;
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerText.gameObject.SetActive(false);
        }
        else
        {
            headerText.gameObject.SetActive(true);
        }

        headerText.text = header;
        contentText.text = content;

        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;    
    }
}
