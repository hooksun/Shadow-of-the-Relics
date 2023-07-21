using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMusic : MonoBehaviour
{
    static GameplayMusic instance;

    public AudioSource stealth, detect;
    public float detectTranstitionTime, stealthTransitionTime;

    float transition, startVolume;
    bool detected;

    void Start()
    {
        this.enabled = false;
        stealth.Play();
    }

    public static void SwitchMusic(bool detected) => instance.Switch(detected);
    public static void PauseMusic(bool pause) => instance.Pause(pause);

    void Switch(bool detected)
    {
        this.detected = detected;
        startVolume = (detected?stealth.volume:detect.volume);
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
