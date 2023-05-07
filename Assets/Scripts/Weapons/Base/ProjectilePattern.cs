using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePattern : MonoBehaviour
{
    public ProjectileInstance[] projectileInstances;

    private void Awake()
    {
        PrepareProjectilePattern();
    }

    private void PrepareProjectilePattern()
    {
        if (projectileInstances.Length > 0)
        {
            //Sorts the array based on timeOffsets
            Array.Sort(projectileInstances,
                delegate (ProjectileInstance x, ProjectileInstance y) { return x.timeOffset.CompareTo(y.timeOffset); });
        }
    }

    /// <summary>
    /// Returns true if the pattern is valid
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return projectileInstances.Length != 0;
    }
}

[System.Serializable]
public class ProjectileInstance
{
    //Time (in seconds) at which this projectile should be fired from the initial projectile/call of weapon use
    public float timeOffset;
    //Angle (in degrees) offset in Z this projectile will fire at from weapon direction
    public float angleOffset;
}
