using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Level Info")]
public class LevelInfo : ScriptableObject
{
    public string LevelName;
    public string SceneName;
    public string description;
    public int powerLevel;
    public int weaponChestCount;
    public int attachmentChestCount;
}

