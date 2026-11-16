using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    const string FileName = "save.json";

    static SaveData data;

    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static SaveData Data
    {
        get
        {
            if (data == null) data = Load();
            return data;
        }
    }

    static SaveData Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new SaveData();

            string json = File.ReadAllText(FilePath);
            SaveData loaded = JsonUtility.FromJson<SaveData>(json);

            if (loaded == null || loaded.version != SaveData.CurrentVersion) return new SaveData();

            loaded.Sanitize();
            return loaded;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Save file is broken, using defaults: " + exception.Message);
            return new SaveData();
        }
    }

    public static void Save()
    {
        try
        {
            string tempPath = FilePath + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(Data, true));

            if (File.Exists(FilePath)) File.Replace(tempPath, FilePath, null);
            else File.Move(tempPath, FilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Could not save: " + exception.Message);
        }
    }

    public static void Reset()
    {
        data = new SaveData();

        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Could not delete the save file: " + exception.Message);
        }
    }
}
