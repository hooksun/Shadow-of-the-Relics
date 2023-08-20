using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    public CanvasGroup[] Backgrounds;
    public float showTime, fadeTime;
    public int nextScene;

    int current;
    bool fading;

    void Start()
    {
        StartCoroutine(CutScene());
    }

    public void Skip(InputAction.CallbackContext ctx)
    {
        if(!ctx.started || fading)
            return;

        StopAllCoroutines();
        StartCoroutine(CutScene(current+1));
    }

    IEnumerator CutScene(int start = 1)
    {
        for(int i = start; i < Backgrounds.Length; i++)
        {
            current = i;
            fading = true;
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

            fading = false;
            yield return new WaitForSeconds(showTime);
        }
        SceneManager.LoadScene(nextScene);
    }

    void SetAlpha(CanvasGroup image, float a)
    {
        image.alpha = a;
    }
}