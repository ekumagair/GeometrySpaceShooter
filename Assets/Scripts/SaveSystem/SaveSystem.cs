using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

public class SaveSystem : MonoBehaviour
{
    public static string GetSaveFilePath()
    {
        return Application.persistentDataPath + "/SavedData.gss";
    }

    public static void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetSaveFilePath();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED data at " + path);
        }
    }

    public static PlayerData LoadData()
    {
        string path = GetSaveFilePath();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("LOADED data at " + path);
            }

            return data;
        }
        else
        {
            Debug.Log("Data NOT FOUND in " + path);
            return null;
        }
    }

    public static void DeleteSave()
    {
        string path = GetSaveFilePath();

        if (File.Exists(path))
        {
            File.Delete(path);

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("DELETED data at " + path);
            }
        }
    }

    public static bool SaveExists()
    {
        string path = GetSaveFilePath();

        return File.Exists(path);
    }
}
