using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//What direction/side should the connecting piece attach to
public enum ConnectionDirection { Left, Right, Above, Below}

public class LevelConnection : MonoBehaviour
{
    public ConnectionDirection direction;
    public bool isAttached = false;
}
