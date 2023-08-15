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

    protected override void Start()
    {
        this.enabled = false;
        detectVolume = detect.volume;
        base.Start();
    }

    public static void SwitchMusic(bool detected) => ((GameplayMusic)instance).Switch(detected);

    void Switch(bool detected)
    {
        this.detected = detected;
        transition = 0f;
        this.enabled = true;
    }

    protected override void Pause(bool pause)
    {
        setPause(bgm, pause);
        setPause(detect, pause);
    }

    void setPause(AudioSource source, bool pause)
    {
        if(pause)
        {
            if(source.isPlaying)
                source.Pause();
        }
        else
            source.UnPause();
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
        from.Pause();
        from.volume = startVolume * Mathf.InverseLerp(time, 0f, transition);
        from.UnPause();
        transition += Time.deltaTime;
        if(transition >= time)
        {
            from.Stop();
            from.volume = startVolume;
            detected = !detected;
            to.volume = startVolume;
            to.Play();
            this.enabled = false;
        }
    }
}
