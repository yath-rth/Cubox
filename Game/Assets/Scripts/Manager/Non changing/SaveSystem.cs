using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(PlayerData Data)
    {
        string path = Path.Combine(Application.persistentDataPath, "data");

        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = File.Create(path);

            PlayerData data = Data;

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log("Save Completed");
        }
        catch
        {
            Debug.LogError("Save was not completed");
        }
    }

    public static PlayerData LoadPlayer()
    {
        string path = Path.Combine(Application.persistentDataPath, "data");
        Debug.Log(path);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            Debug.Log("Save Opened");

            return data;
        }
        else
        {
            Debug.LogError("Save File not found");

            PlayerData playerData = new PlayerData();
            SavePlayer(playerData);

            return playerData;
        }
    }
}
