using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The special case of the level manager. Where the level is the player hub
/// </summary>
public class PlayerHubManager : LevelManager
{
    protected override void InitialisePlayer()
    {
        //we don't give the player their lodaout on entering this level
        return;
    }
}