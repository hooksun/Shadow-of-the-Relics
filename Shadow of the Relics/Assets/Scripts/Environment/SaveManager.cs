using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static Saver saver;
    public delegate void SaveDelegate();
    public static SaveDelegate OnSave, OnLoad;
    public static bool loadOnStart;

    static string path{get=>Application.persistentDataPath + "/Save.json";}

    public static void Save()
    {
        OnSave();
        string save = JsonUtility.ToJson(saver);
        File.WriteAllText(path, save);
    }

    public static void Load()
    {
        if(!File.Exists(path))
            return;
        
        string save = File.ReadAllText(path);
        saver = JsonUtility.FromJson<Saver>(save);

        OnLoad();
    }

    void Start()
    {
        saver = new Saver(0f);
        if(loadOnStart)
            Load();
    }
}

public struct Saver
{
    public PlayerSave playerSave;
    
    public Dictionary<string, Vector2> Vectors;
    public Dictionary<string, float> floats;
    public Dictionary<string, bool> bools;

    public Saver(float wtf)
    {
        Vectors = new Dictionary<string, Vector2>();
        floats = new Dictionary<string, float>();
        bools = new Dictionary<string, bool>();
        playerSave = new PlayerSave();
    }
}