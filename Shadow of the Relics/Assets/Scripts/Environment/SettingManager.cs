using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingManager : MonoBehaviour
{
    public static Settings settings;

    static string path{get=>Application.persistentDataPath + "/Settings.json";}

    public void SetVolume(float volume)
    {
        settings.volume = volume;
        AudioPlayer.SetGlobalVolume(volume);
    }

    public void SetMusic(float music)
    {
        settings.musicVolume = music;
        GameplayMusic.SetMusicVolume(music);
    }

    public void OnSave()
    {
        string json = JsonUtility.ToJson(settings);

        File.WriteAllText(path, json);
    }

    public void OnLoad()
    {
        if(!File.Exists(path))
            return;
        
        string json = File.ReadAllText(path);
        settings = JsonUtility.FromJson<Settings>(json);
        AudioPlayer.SetGlobalVolume(settings.volume);
        GameplayMusic.SetMusicVolume(settings.musicVolume);

    }
}


public struct Settings
{
    public float volume, musicVolume;
}