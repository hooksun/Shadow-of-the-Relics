using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    protected static BackgroundMusic instance;
    
    public SettingManager settingManager;

    public AudioSource bgm;
    protected float bgmVolume, bgmTime;

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
        PauseBGM(pause);
    }

    protected virtual void SetVolume(float volume)
    {
        float time = bgm.time;
        bgm.Pause();
        bgm.volume = bgmVolume * volume;
        bgm.time = time;
        bgm.UnPause();
    }

    protected void PauseBGM(bool pause)
    {
        if(pause)
        {
            bgmTime = bgm.time;
            bgm.Pause();
        }
        else
        {
            bgm.time = bgmTime;
            bgm.Play();
        }
    }
}
