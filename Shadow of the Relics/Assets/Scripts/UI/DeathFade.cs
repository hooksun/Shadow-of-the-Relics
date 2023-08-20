using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathFade : MonoBehaviour
{
    public float fadeDelay, fadeTime;
    public Image image;

    float fade, startAlpha;

    void OnEnable()
    {
        fade = fadeDelay;
        startAlpha = image.color.a;
    }

    void OnDisable()
    {
        Color c = image.color;
        c.a = startAlpha;
        image.color = c;
    }

    void Update()
    {
        Color c = image.color;
        c.a = Mathf.Lerp(fadeTime, startAlpha, fade);
        image.color = c;
        if(fade > 0f)
            fade -= Time.deltaTime;
    }
}
