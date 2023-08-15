using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingManager : MonoBehaviour
{
    public Slider volume, music;

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
        BackgroundMusic.SetMusicVolume(music);
    }

    public static void OnSave()
    {
        string json = JsonUtility.ToJson(settings);

        File.WriteAllText(path, json);
    }

    void OnEnable()
    {
        if(volume == null)
            return;
        volume.value = settings.volume;
        music.value = settings.musicVolume;
    }

    public void OnLoad()
    {
        if(!File.Exists(path))
        {
            settings = new Settings(1f, 1f);
            return;
        }
        
        string json = File.ReadAllText(path);
        settings = JsonUtility.FromJson<Settings>(json);
        AudioPlayer.SetGlobalVolume(settings.volume);
        BackgroundMusic.SetMusicVolume(settings.musicVolume);
    }
}


public struct Settings
{
    public float volume, musicVolume;

    public Settings(float vol, float mus)
    {
        volume = vol;
        musicVolume = mus;
    }
}