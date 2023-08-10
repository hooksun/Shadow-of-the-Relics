using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMusic : MonoBehaviour
{
    static GameplayMusic instance;

    public AudioSource stealth, detect;
    public float detectTranstitionTime, stealthTransitionTime;

    float transition, volume = 1f, stealthVolume, detectVolume;
    bool detected;

    float startVolume{get=>(detected?stealthVolume:detectVolume) * volume;}

    void Start()
    {
        instance = this;
        this.enabled = false;
        stealthVolume = stealth.volume;
        detectVolume = detect.volume;
        stealth.Play();
    }

    public static void SwitchMusic(bool detected) => instance.Switch(detected);
    public static void PauseMusic(bool pause) => instance.Pause(pause);
    public static void SetMusicVolume(float volume) => instance.SetVolume(volume);

    void Switch(bool detected)
    {
        this.detected = detected;
        transition = 0f;
        this.enabled = true;
    }

    void Pause(bool pause)
    {
        setPause(stealth, pause);
        setPause(detect, pause);
    }

    void setPause(AudioSource source, bool pause)
    {
        if(source.isPlaying)
        {
            if(pause)
                source.Pause();
            else
                source.UnPause();
        }
    }

    void SetVolume(float volume)
    {
        this.volume = volume;
        stealth.volume = stealthVolume * volume;
        detect.volume = detectVolume * volume;
    }

    void Update()
    {
        if(Time.timeScale == 0f)
            return;

        if(detected)
        {
            Transition(stealth, detect, detectTranstitionTime);
            return;
        }
        Transition(detect, stealth, stealthTransitionTime);
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
            to.Play();
            this.enabled = false;
        }
    }
}
