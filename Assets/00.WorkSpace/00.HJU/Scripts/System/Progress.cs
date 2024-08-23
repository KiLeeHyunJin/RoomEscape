using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Progress : MonoBehaviour
{
    [SerializeField] Slider sliderProgress;
    [SerializeField] Transform labbit;
    [SerializeField] TMP_Text textProgressData;
    [SerializeField] float progressTime;
    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 endPosition;

    private void OnEnable()
    {
        Play();
    }

    public void Play(UnityAction action = null)
    {
        StartCoroutine(OnProgress(action));
    }

    private IEnumerator OnProgress(UnityAction action)
    {
       // AsyncOperation operation = UnitySceneManager.LoadSceneAsync("LoginScene");

        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / progressTime;

            sliderProgress.value = percent;
            labbit.position = Vector3.Lerp(startPosition, endPosition, percent);
            textProgressData.text = $"Loading... {sliderProgress.value * 100:F0}%";

            yield return null;
        }

        action?.Invoke();
    }

    //실제 로딩에 필요한 시간 적용 필요

}
