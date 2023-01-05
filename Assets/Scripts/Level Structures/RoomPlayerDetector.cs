using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayerDetector : MonoBehaviour
{
    //Player Enter delegate
    public delegate void OnPlayerEnterDelegate();
    public OnPlayerEnterDelegate OnPlayerEnter;

    //Player Exit delegate
    public delegate void OnPlayerExitDelegate();
    public OnPlayerExitDelegate OnPlayerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            OnPlayerEnter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            OnPlayerExit();
        }
    }
}
