using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that just constantly rotates the attached object
/// </summary>
public class ConstantRotation : MonoBehaviour
{
    public float rotateSpeed;
    public bool rotateOnAwake;

    private bool rotation;

    private void Awake()
    {
        rotation = rotateOnAwake;
    }

    /// <summary>
    /// Turns the constant rotation on or off
    /// </summary>
    /// <param name="state"></param>
    public void SetConstantRotation(bool state)
    {
        rotation = state;
    }

    void Update()
    {
        if (rotation)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime); 
        }
    }
}
