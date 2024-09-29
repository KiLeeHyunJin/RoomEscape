using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;


    public AudioSource BgmSource { get { return bgmSource; } set { bgmSource = value; } }
    public float BGMVolme { get { return bgmSource.volume; } set { bgmSource.volume = value; } }
    public float SFXVolme { get { return sfxSource.volume; } set { sfxSource.volume = value; } }
    public float SetPitchAndSpeed { set { bgmSource.pitch = value; } }


   

    private IEnumerator FadeInRoutine(AudioSource audio, float duringTime)
    {
        float startVolume = 0f;
        float targetVolume = 0.5f;
        float elapsedTime = 0;

        Manager.Sound.BgmSource.mute = false;

        while (elapsedTime < duringTime)
        {
            audio.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duringTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audio.volume = targetVolume;
    }

    IEnumerator FadeOutRoutine(AudioSource audio, float duringTime)
    {
        float startVolume = audio.volume;
        float targetVolume = 0;
        float elapsedTime = 0;

        while (elapsedTime < duringTime)
        {
            audio.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duringTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audio.volume = targetVolume;
        audio.Stop();
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == bgmSource.clip)
            return;
        StartCoroutine(ChangeBGM(clip));
    }

    IEnumerator ChangeBGM(AudioClip newClip)
    {
        if (bgmSource.isPlaying)
        {
            yield return FadeOutRoutine(bgmSource, 1f);
        }

        bgmSource.clip = newClip;
        bgmSource.Play();
        yield return FadeInRoutine(bgmSource, 1f);
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    void StopBGM()
    {
        if (bgmSource.isPlaying == false)
            return;
        bgmSource.Stop();
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    public void PlaySFX(AudioClip clip, bool onlyPlay = false)
    {
        if(onlyPlay == false)
        {
            sfxSource.PlayOneShot(clip);
            return;
        }

        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    /// <summary>
    /// SFX 정지
    /// </summary>
    public void StopSFX()
    {
        if (sfxSource.isPlaying == false)
            return;

        sfxSource.Stop();
    }



    /// <summary>
    /// 피치 변경
    /// </summary>
    public void ChangePitch(float pitch)
    {
        bgmSource.pitch = pitch;
    }

    /// <summary>
    /// 피치 리셋
    /// </summary>
    public void ResetPitch()
    {
        if (bgmSource.pitch != 1)
            bgmSource.pitch = 1;
    }
}

