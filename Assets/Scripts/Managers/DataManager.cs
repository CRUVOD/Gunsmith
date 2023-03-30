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
    [SerializeField]
    private string attachmentDataFolder = default;

    //Dictionaries to store all data loaded
    private Dictionary<string, WeaponReference> weaponDictionary;
    private Dictionary<string, WeaponAttachmentReference> attachmentDictionary;

    private void LoadFromResource()
    {
        //Weapoons
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

        //Attachments
        attachmentDictionary = new Dictionary<string, WeaponAttachmentReference>();
        WeaponAttachmentReference[] attachmentReferencesFromResource = Resources.LoadAll<WeaponAttachmentReference>(attachmentDataFolder);
        foreach (WeaponAttachmentReference attachmentReference in attachmentReferencesFromResource)
        {
            if (attachmentReference.ID != null)
            {
                attachmentDictionary.Add(attachmentReference.ID, attachmentReference);
            }
            else
            {
                Debug.LogError("Attachment does not have ID");
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
    /// Returns the weapon attachment dictionary
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, WeaponAttachmentReference> GetAttachmentDictionary()
    {
        return attachmentDictionary;
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

    /// <summary>
    /// Tries to return an attachment reference based on ID, will return null
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public WeaponAttachmentReference TryGetAttachmentReference(string ID)
    {
        WeaponAttachmentReference reference;
        if (attachmentDictionary.TryGetValue(ID, out reference))
        {
            return reference;
        }
        else
        {
            return null;
        }
    }
}
