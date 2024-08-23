using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] Image fade;
    [SerializeField] RectTransform labbit;
    [SerializeField] GameObject canvas;
    [SerializeField] Slider loadingBar;
    [SerializeField] float fadeTime;
    [SerializeField] RectTransform rectSlider;
    [SerializeField] TMP_Text textProgressData;

    public Vector2 start;
    public Vector2 end;
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
        Manager.Sound.PlayBGMSceneName(sceneName);
        //대기목록 초기화
        Manager.Resource.ClearWaitList();
        StartCoroutine(LoadingRoutine(sceneName));
    }

    private IEnumerator LoadingRoutine(string sceneName)
    {
        canvas.SetActive(true);

        start = new Vector2(rectSlider.anchoredPosition.x - (rectSlider.rect.width / 2), rectSlider.anchoredPosition.y + 200);
        end = new Vector2(rectSlider.anchoredPosition.x + (rectSlider.rect.width / 2), rectSlider.anchoredPosition.y + 200);
        Debug.Log($"{rectSlider.anchoredPosition.x}/{rectSlider.rect.width / 2}/{end}");
        if (string.IsNullOrEmpty(sceneName))
            sceneName = "Chapter1Scene";
        // 실제 로딩 작업 시작
        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);

        while (!oper.isDone)
        {
            // 로딩 진행도를 0 ~ 0.9까지로 설정
            float loadingProgress = Mathf.Clamp01((oper.progress / 0.9f) * 0.9f);
            loadingBar.value = loadingProgress;
            labbit.anchoredPosition = Vector3.Lerp(start, end, loadingProgress);
            textProgressData.text = $"Loading... {loadingBar.value * 100:F0}%";

            yield return null;
        }
        loadingBar.value = 0.9f;
        labbit.anchoredPosition = Vector3.Lerp(start, end, 0.9f);
        textProgressData.text = $"Loading... {loadingBar.value * 100:F0}%";

        // 로딩 완료 후 추가 1초 동안 진행도를 0.9에서 1까지 업데이트
        float current = 0;
        float percent = 0;
        float additionalLoadingTime = 1f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / additionalLoadingTime;

            loadingBar.value = Mathf.Lerp(0.9f, 1f, percent);
            labbit.anchoredPosition = Vector3.Lerp(start, end, Mathf.Lerp(0.9f, 1f, percent));
            textProgressData.text = $"Loading... {loadingBar.value * 100:F0}%";

            yield return null;
        }
        System.GC.Collect();
        while (Manager.Resource.LoadCount != 0)
        {
            yield return null;
        }

        canvas.SetActive(false);
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
