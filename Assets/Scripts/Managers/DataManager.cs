using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads scriptable objects data from resource folder
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            return;
        }

        DontDestroyOnLoad(gameObject);
        Initialise();
    }

    [SerializeField]
    private string weaponDataFolder = default;

    //Stores all data loaded
    private Dictionary<string, WeaponReference> weaponDictionary;

    private void LoadFromResource()
    {
        weaponDictionary = new Dictionary<string, WeaponReference>();

        WeaponReference[] weaponReferencesFromResource = Resources.LoadAll<WeaponReference>(weaponDataFolder);
        foreach (WeaponReference weaponReference in weaponReferencesFromResource)
        {
            if (weaponReference.ID != null)
            {
                weaponDictionary.Add(weaponReference.ID, weaponReference);
            }
            else
            {
                Debug.LogError("Weapon does not have ID");
            }
        }
    }

    public void Initialise()
    {
        LoadFromResource();
    }

    /// <summary>
    /// Returns the weapon dictionary
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, WeaponReference> GetWeaponDictionary()
    {
        return weaponDictionary;
    }

    /// <summary>
    /// Tries to return a weapon reference based on ID, will return null
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public WeaponReference TryGetWeaponReference(string ID)
    {
        WeaponReference reference;
        if (weaponDictionary.TryGetValue(ID, out reference))
        {
            return reference;
        }
        else
        {
            return null;
        }
    }
}
