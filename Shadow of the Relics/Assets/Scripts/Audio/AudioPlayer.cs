using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource source;
    public float minPitch, maxPitch, loopLength;
    public bool loop;


    bool playing, looping;

    public void Play()
    {
        if(looping || source.isPlaying)
            return;

        playing = true;
        StartCoroutine(PlayLoop());
    }

    public void Stop()
    {
        playing = false;
    }

    IEnumerator PlayLoop()
    {
        if(!loop)
        {
            PlayAudio();
            yield break;
        }

        while(playing)
        {
            PlayAudio();

            looping = true;
            yield return new WaitForSeconds(loopLength);
            looping = false;
        }
    }

    void PlayAudio()
    {
        source.Stop();
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }

    public void ChangeVolume()
    {

    }
}
