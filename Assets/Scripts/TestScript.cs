using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put anything here, temporary test script
/// </summary>
public class TestScript : MonoBehaviour
{
    public Image image;

    float Timer;
    
    private void Update()
    {
        image.fillAmount = Timer / 10f;

        Timer += Time.deltaTime;
    }
}
