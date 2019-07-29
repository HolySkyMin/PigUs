using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool SaveFileExists { get { return File.Exists($"{Application.persistentDataPath}/save"); } }
    public static bool Phase2Exists { get { return File.Exists($"{Application.persistentDataPath}/save_phase2"); } }

    public SaveData Save;
    public GameConfig Config;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        LoadConfig();
    }

    public void CreateConfig()
    {
        Config = new GameConfig();
        SaveConfig();
    }

    public void LoadConfig()
    {
        if (!File.Exists($"{Application.persistentDataPath}/config"))
            CreateConfig();
        else
        {
            var stream = new FileStream($"{Application.persistentDataPath}/config", FileMode.Open);
            var formatter = new BinaryFormatter();
            Config = formatter.Deserialize(stream) as GameConfig;
            stream.Close();
        }
    }

    public void SaveConfig()
    {
        var stream = new FileStream($"{Application.persistentDataPath}/config", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, Config);
        stream.Close();

        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (Config.AllowFullscreen)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            else
                Screen.SetResolution(1600, 900, false);
        }
    }

    public void CreateData(bool skipTutorial)
    {
        DeleteFile("save_phase2");
        Save = new SaveData() { SkipTutorial = skipTutorial };
        Save.Phase1StoryDay = Random.Range(3, 5);
        SaveToFile();
    }

    public void LoadToData()
    {
        var stream = new FileStream($"{Application.persistentDataPath}/save", FileMode.Open);
        var formatter = new BinaryFormatter();
        Save = formatter.Deserialize(stream) as SaveData;
        stream.Close();
    }

    public void SaveToFile()
    {
        var stream = new FileStream($"{Application.persistentDataPath}/save", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, Save);
        stream.Close();
    }

    public void LoadPhase2()
    {
        var stream = new FileStream($"{Application.persistentDataPath}/save_phase2", FileMode.Open);
        var formatter = new BinaryFormatter();
        Save = formatter.Deserialize(stream) as SaveData;
        stream.Close();
    }

    public void SavePhase2()
    {
        var stream = new FileStream($"{Application.persistentDataPath}/save_phase2", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, Save);
        stream.Close();
    }

    public void DeleteFile(string filename)
    {
        File.Delete($"{Application.persistentDataPath}/{filename}");
    }
}