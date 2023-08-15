using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    public Image[] Backgrounds;
    public float showTime, fadeTime;

    void Start()
    {
        StartCoroutine(CutScene());
    }

    IEnumerator CutScene()
    {
        for(int i = 1; i < Backgrounds.Length; i++)
        {
            float fade = fadeTime;
            while(fade > 0f)
            {
                float a = fade/fadeTime;
                SetAlpha(Backgrounds[i-1], a);
                SetAlpha(Backgrounds[i], 1-a);
                yield return null;
                fade -= Time.deltaTime;
            }
            SetAlpha(Backgrounds[i-1], 0f);
            SetAlpha(Backgrounds[i], 1f);

            yield return new WaitForSeconds(showTime);
        }
        SceneManager.LoadScene(0);
    }

    void SetAlpha(Image image, float a)
    {
        Color color = image.color;
        color.a = a;
        image.color = color;
    }
}