using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool SaveFileExists { get { return File.Exists($"{Application.dataPath}/save"); } }

    public SaveData Save;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void CreateData()
    {
        Save = new SaveData();
        SaveToFile();
    }

    public void LoadToData()
    {
        var stream = new FileStream($"{Application.dataPath}/save", FileMode.Open);
        var formatter = new BinaryFormatter();
        Save = formatter.Deserialize(stream) as SaveData;
        stream.Close();
    }

    public void SaveToFile()
    {
        var stream = new FileStream($"{Application.dataPath}/save", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, Save);
        stream.Close();
    }
}