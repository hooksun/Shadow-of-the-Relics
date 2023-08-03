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

    public List<EnemySave> EnemySaves;
    public List<bool> CollectedArtifacts;

    public Saver(float wtf)
    {
        playerSave = new PlayerSave();
        EnemySaves = new List<EnemySave>();
        CollectedArtifacts = new List<bool>();
    }
}