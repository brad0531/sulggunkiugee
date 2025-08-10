using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum SFXSound
    {
        
    };

    public enum BGMSound
    {

    };
    public static AudioManager Instance;


    [Header("BGM Source")]
    public AudioSource bgmSource;
    public List<AudioClip> bgmSources; 
    [Header("SFX Pool Settings")]
    public int sfxPoolSize = 10;
    private List<AudioSource> sfxPool;
    private int sfxIndex = 0;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f; // 🔹 마스터 볼륨 추가
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Fade Settings")]
    public float fadeTime = 1.0f; 

    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitSFXPool()
    {
        sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            sfxPool.Add(src);
        }
    }

    public void PlayBGM(BGMSound sound, bool loop = true)
    {
        AudioClip clip = bgmSources[(int)sound];
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume * masterVolume; // 🔹 마스터 볼륨 적용
        bgmSource.Play();
    }

    public void ChangeBGM(BGMSound newClip, bool loop = true)
    {
        AudioClip NewClip = bgmSources[(int)newClip];
        if (NewClip == null) return;

        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
        bgmFadeCoroutine = StartCoroutine(FadeBGM(NewClip, loop));
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource src = sfxPool[sfxIndex];
        src.clip = clip;
        src.volume = sfxVolume * masterVolume; // 🔹 마스터 볼륨 적용
        src.Play();

        sfxIndex = (sfxIndex + 1) % sfxPoolSize;
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        AudioSource src = sfxPool[sfxIndex];
        src.transform.position = position;
        src.spatialBlend = 1f;
        src.clip = clip;
        src.volume = sfxVolume * masterVolume; // 🔹 마스터 볼륨 적용
        src.Play();

        src.spatialBlend = 0f;
        sfxIndex = (sfxIndex + 1) % sfxPoolSize;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume * masterVolume; // 🔹 마스터 볼륨 적용
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    private IEnumerator FadeBGM(AudioClip newClip, bool loop)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = 0f;
        bgmSource.Stop();

        bgmSource.clip = newClip;
        bgmSource.loop = loop;
        bgmSource.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume * masterVolume, t / fadeTime); // 🔹 마스터 볼륨 적용
            yield return null;
        }
        bgmSource.volume = bgmVolume * masterVolume;
    }
}
