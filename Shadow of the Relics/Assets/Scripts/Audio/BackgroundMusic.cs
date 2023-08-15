using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    protected static BackgroundMusic instance;
    
    public SettingManager settingManager;

    public AudioSource bgm;
    protected float bgmVolume;

    protected virtual void Start()
    {
        instance = this;
        bgmVolume = bgm.volume;

        settingManager.OnLoad();
        bgm.Play();
    }

    public static void PauseMusic(bool pause) => instance.Pause(pause);
    public static void SetMusicVolume(float volume) => instance.SetVolume(volume);

    protected virtual void Pause(bool pause)
    {
        if(pause)
            bgm.Pause();
        else
            bgm.UnPause();
    }

    protected virtual void SetVolume(float volume)
    {
        bgm.Pause();
        bgm.volume = bgmVolume * volume;
        bgm.UnPause();
    }
}
