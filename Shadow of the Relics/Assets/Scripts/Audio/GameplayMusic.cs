using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMusic : BackgroundMusic
{
    public AudioSource detect;
    public float detectTranstitionTime, stealthTransitionTime;

    float transition, volume = 1f, detectVolume;
    bool detected;

    float startVolume{get=>(detected?bgmVolume:detectVolume) * volume;}

    float detectTime;

    protected override void Start()
    {
        this.enabled = false;
        detectVolume = detect.volume;
        base.Start();
    }

    public static void SwitchMusic(bool detected) => ((GameplayMusic)instance).Switch(detected);

    void Switch(bool detected)
    {
        if(detect.isPlaying == detected && bgm.isPlaying != detected)
            return;
        this.detected = detected;
        transition = 0f;
        this.enabled = true;
    }

    protected override void Pause(bool pause)
    {
        setPause(bgm, pause, ref bgmTime);
        setPause(detect, pause, ref detectTime);
    }

    void setPause(AudioSource source, bool pause, ref float time)
    {
        if(pause)
        {
            if(source.isPlaying)
            {
                time = source.time;
                source.Pause();
            }
        }
        else
        {
            source.time = time;
            source.UnPause();
        }
    }

    protected override void SetVolume(float volume)
    {
        this.volume = volume;
        bgm.volume = bgmVolume * volume;
        detect.volume = detectVolume * volume;
    }

    void Update()
    {
        if(Time.timeScale == 0f)
            return;

        if(detected)
        {
            Transition(bgm, detect, detectTranstitionTime);
            return;
        }
        Transition(detect, bgm, stealthTransitionTime);
    }

    void Transition(AudioSource from, AudioSource to, float time)
    {
        #if UNITY_WEBGL

        transition = time;

        #else

        float musictime = from.time;
        from.Pause();
        from.volume = startVolume * Mathf.InverseLerp(time, 0f, transition);
        from.time = musictime;
        from.UnPause();
        from.time = musictime;
        transition += Time.deltaTime;

        #endif

        if(transition >= time)
        {
            from.Stop();
            from.volume = startVolume;
            detected = !detected;
            to.volume = startVolume;
            to.time = 0f;
            to.Play();
            this.enabled = false;
        }
    }
}
