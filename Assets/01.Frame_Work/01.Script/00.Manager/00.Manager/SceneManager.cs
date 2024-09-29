using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] Image fade;
    [SerializeField] Slider loadingBar;
    [SerializeField] float fadeTime;
    [SerializeField] RectTransform rectSlider;

    private BaseScene curScene;

    public BaseScene GetCurScene()
    {
        if (curScene == null)
        {
            curScene = FindObjectOfType<BaseScene>();
        }
        return curScene;
    }

    public T GetCurScene<T>() where T : BaseScene
    {
        if (curScene == null)
        {
            curScene = FindObjectOfType<BaseScene>();
        }
        return curScene as T;
    }

    public void LoadScene(string sceneName)
    {
        //대기목록 초기화
        Manager.Resource.ClearWaitList();
        StartCoroutine(LoadingRoutine(sceneName));
    }

    IEnumerator LoadingRoutine(string sceneName)
    {
        fade.gameObject.SetActive(true);
        yield return FadeOut();

        Manager.Pool.ClearPool();
        Manager.Sound.StopSFX();
        Manager.UI.ClearPopUpUI();
        Manager.UI.ClearWindowUI();
        Manager.UI.CloseInGameUI();

        Time.timeScale = 0f;
        System.GC.Collect();


        // 실제 로딩 작업 시작
        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);

        while (!oper.isDone)
        {
            // 로딩 진행도를 0 ~ 0.9까지로 설정
            float loadingProgress = Mathf.Clamp01((oper.progress / 0.9f) * 0.9f);
            loadingBar.value = loadingProgress;

            yield return null;
        }
        loadingBar.value = 0.9f;

        // 로딩 완료 후 추가 1초 동안 진행도를 0.9에서 1까지 업데이트
        float current = 0;
        float percent = 0;
        float additionalLoadingTime = 1f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / additionalLoadingTime;

            loadingBar.value = Mathf.Lerp(0.9f, 1f, percent);

            yield return null;
        }


        while (Manager.Resource.LoadCount != 0)
        {
            yield return null;
        }

        yield return FadeIn();

        Manager.UI.EnsureEventSystem();
        Time.timeScale = 1f;
        fade.gameObject.SetActive(false);
    }



    IEnumerator FadeOut()
    {
        float rate = 0;
        Color fadeOutColor = new Color(fade.color.r, fade.color.g, fade.color.b, 1f);
        Color fadeInColor = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            fade.color = Color.Lerp(fadeInColor, fadeOutColor, rate);
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float rate = 0;
        Color fadeOutColor = new Color(fade.color.r, fade.color.g, fade.color.b, 1f);
        Color fadeInColor = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            fade.color = Color.Lerp(fadeOutColor, fadeInColor, rate);
            yield return null;
        }
    }
}
