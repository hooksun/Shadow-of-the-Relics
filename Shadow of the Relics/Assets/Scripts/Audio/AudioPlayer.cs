using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    static List<AudioPlayer> ActiveAudios = new List<AudioPlayer>();

    public AudioSource source;
    public float minPitch, maxPitch, volume, length;

    bool playing, onCooldown;

    static float globalVolume;

    void OnEnable()
    {
        ActiveAudios.Add(this);
    }

    void OnDisable()
    {
        ActiveAudios.Remove(this);
    }

    public void Play()
    {
        if(onCooldown)
            return;

        PlayAudio();
        if(length > 0f)
            StartCoroutine(Cooldown(length));
    }

    public void Stop()
    {
        if(source.isPlaying)
            source.Stop();
        if(onCooldown)
        {
            StopCoroutine(Cooldown(length));
            onCooldown = false;
        }
    }

    IEnumerator Cooldown(float time)
    {
        onCooldown = true;
        yield return new WaitForSeconds(time);
        onCooldown = false;
    }

    void PlayAudio()
    {
        source.Stop();
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }

    public static void SetGlobalVolume(float volume)
    {
        globalVolume = volume;
        foreach(AudioPlayer player in ActiveAudios)
            player.SetVolume(volume);
    }

    void SetVolume(float volume)
    {
        source.volume = this.volume * volume;
    }

    public static void PauseAll(bool pause)
    {
        foreach(AudioPlayer audio in ActiveAudios)
        {
            audio.Pause(pause);
        }
    }

    void Pause(bool pause)
    {
        if(pause)
        {
            if(source.isPlaying)
                source.Pause();
            return;
        }
        source.UnPause();
    }
}
