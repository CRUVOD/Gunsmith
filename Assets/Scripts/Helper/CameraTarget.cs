using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Player player;
    public float maxDistanceFromPlayer;

    private void LateUpdate()
    {
        CalculatePosition();
    }

    void CalculatePosition()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > maxDistanceFromPlayer)
        {
            Vector3 fromOriginToObject = transform.position - player.transform.position;
            fromOriginToObject *= maxDistanceFromPlayer / distance;
            transform.position = player.transform.position + fromOriginToObject;
        }
    }
}
