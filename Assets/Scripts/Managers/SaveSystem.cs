using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.saved";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static void SavePlayer(PlayerData playerData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.saved";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, playerData);
        fileStream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.saved";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            PlayerData playerData = formatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();
            return playerData;
        }
        else
        {
            Debug.LogWarning("Player save file not found at" + path);
            Debug.LogWarning("Making default player");
            PlayerData defaultData = new PlayerData(new string[] { "K98K", "P226" }, new string[] { "Overdrive" });
            defaultData.weaponsInLoadout = new string[] { "K98K", "P226" };
            SavePlayer(defaultData);
            return defaultData;
        }
    }

    public static void DeleteSave()
    {
        if (SaveFileExists())
        {
            string path = Application.persistentDataPath + "/player.saved";
            File.Delete(path);
        }
    }

    /// <summary>
    /// Returns true if there exists a save file
    /// </summary>
    /// <returns></returns>
    public static bool SaveFileExists()
    {
        string path = Application.persistentDataPath + "/player.saved";
        return File.Exists(path);
    }
}
