using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathFade : MonoBehaviour
{
    public float fadeDelay, fadeTime;
    public Image image;

    float fade;

    void OnEnable()
    {
        fade = fadeDelay;
    }

    void Update()
    {
        Color c = image.color;
        c.a = Mathf.Lerp(fadeTime, 0f, fade);
        image.color = c;
        if(fade > 0f)
            fade -= Time.deltaTime;
    }
}
