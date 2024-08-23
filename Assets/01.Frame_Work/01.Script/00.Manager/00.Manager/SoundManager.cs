using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource itemSource;
    [SerializeField] AudioSource buttonSource;

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] Sfxs;

    public AudioSource BgmSource { get { return bgmSource; } set { bgmSource = value; } }
    public float BGMVolme { get { return bgmSource.volume; } set { bgmSource.volume = value; } }
    public float SFXVolme { get { return sfxSource.volume; } set { sfxSource.volume = value; } }
    public float SetPitchAndSpeed { set { bgmSource.pitch = value; } }

    private void Start()
    {
        PlayBGMByIndex(0);
    }

    public Dictionary<string, int> itemGetSoundMapping = new Dictionary<string, int>
    {
        { "Shovel", 1 },
        { "FirePlaceKey", 4 },
        { "Fishbowl", 29 },
        { "Cup", 29 },
        { "Dough", 19 },
        { "YellowPostMessage",19},
        { "GreenPostMessage", 19 },
        { "ChocoDoorKey", 4 },
        { "SilverRing", 18 },
        { "Handkerchief", 18 },
        { "RightHint", 39 },
        { "LefttHint", 39 },
        { "Bone", 5 }
    };

    public Dictionary<string, int> itemUseSoundMapping = new Dictionary<string, int>
    {
        { "Handkerchief", 6},
        { "Bone", 6}
    };

    public Dictionary<string, int> itemCombineSoundMapping = new Dictionary<string, int>
    {
        { "CompleteHint",6}
    };

    /// <summary>
    /// 씬 변경할 때 BGM 재생
    /// </summary>
    public void PlayBGMSceneName(string SceneName)
    {
        switch (SceneName)
        {
            case "LoginScene":
                PlayBGMByIndex(0);
                break;
            case "LobbyScene":
                PlayBGMByIndex(1);
                break;
            default: //챕터에서 BGM 2개 랜덤재생
                int randomIndex = UnityEngine.Random.Range(4, 6);
                PlayBGMByIndex(randomIndex);
                break;
        }
    }

    /// <summary>
    /// 인덱스에 따른 BGM 재생
    /// </summary>
    public void PlayBGMByIndex(int index)
    {
        if (index >= 0 && index < bgms.Length)
        {
            StartCoroutine(ChangeBGM(bgmSource, bgms[index]));
        }
        else
        {
            Debug.Log("해당 인덱스는 존재하지 않습니다. : " + index);
        }
    }

    /// <summary>
    /// 인덱스에 따른 SFX 재생
    /// </summary>
    public void PlayButtonSound(int index)
    {
        if (index >= 0 && index < Sfxs.Length)
        {
            buttonSource.Stop();    

            if(index > 1 && index < 41 )
            {
                Debug.Log("Call Clip");
                Manager.Resource.GetAsset("basic", index.ToString(), ResourceType.AudioClip, (obj) => 
                {
                    AudioClip clip = (AudioClip)obj;
                    if(clip != null)
                    {
                        Debug.Log("Play Clip");
                        PlayButtonSound(clip);
                    }
                });
            }
            else
            {
                PlayButtonSound(Sfxs[index]);
            }
        }
        else
        {
            Debug.Log("해당 인덱스는 존재하지 않습니다. : " + index);
        }
    }

    void PlayButtonSound(AudioClip clip)
    {
        buttonSource.clip = clip;
        buttonSource.Play();
    }

    public void PlayItemSound(int index)
    {
        if (index >= 0 && index < Sfxs.Length)
        {
            itemSource.Stop();
            itemSource.clip = Sfxs[index];
            itemSource.Play();
        }
        else
        {
            Debug.Log("해당 인덱스는 존재하지 않습니다. : " + index);
        }
    }

    IEnumerator ChangeBGM(AudioSource newSource, AudioClip newClip)
    {
        if (bgmSource.isPlaying)
        {
            yield return FadeOutRoutine(bgmSource, 1f);
            bgmSource.Stop();
        }

        newSource.clip = newClip;
        newSource.Play();
        yield return FadeInRoutine(newSource, 1f);
        bgmSource = newSource;
    }

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
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying == false)
            return;

        bgmSource.Stop();
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        //sfxSource.PlayOneShot(clip);
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
    /// 특정 시간부터 BGM 재생
    /// </summary>
    public void PlaySoundFromTime(int index, float startTime)
    {
        bgmSource.time = startTime;
        PlayBGMByIndex(index);
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

