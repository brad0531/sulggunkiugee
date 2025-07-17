using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public Image fadeImage;
    public float fadeduration = 1f;

    public static Action<FadeInOut> Fade;
    public static Action<FadeInOut> FadeStart;

    public IEnumerator FadeIn()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeduration)
        {
            t+=Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeduration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeduration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeduration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, 0f);
    }

    private IEnumerator FadeInAndOut()
    {
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator ToStart()
    {
        Color color = fadeImage.color;
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        yield return StartCoroutine(FadeOut());
    }

    private void Awake()
    {
        Fade = (fadereal) =>
        {
            fadereal.StartCoroutine(fadereal.FadeInAndOut());
        };
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
