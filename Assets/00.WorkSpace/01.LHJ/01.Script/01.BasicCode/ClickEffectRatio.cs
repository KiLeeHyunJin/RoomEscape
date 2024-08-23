using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 클릭 이팩트 클래스
/// </summary>
public class ClickEffectRatio : PooledObject
{
    [SerializeField] float multipleScale;
    [SerializeField] float duringTime;

    RectTransform rectTransform;
    Image img;

    private void Awake()
    {
        //현재 객체의 렉트트랜스폼을 가져온다.
        rectTransform = GetComponent<RectTransform>();
        //현재 객체의 이미지를 가져온다.
        img = GetComponent<Image>();
    }

    /// <summary>
    /// 매개변수 위치에 현재 게임오브젝트를 배치한다.
    /// </summary>
    public void SetPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    /// <summary>
    /// 상태 정보를 초기화한다.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        //크기의 비율을 1배수로 설정한다.
        rectTransform.localScale = Vector3.one;
        //현재 이미지의 색상 정보를 가져온다.
        Color color = img.color;
        //Alpha값을 1로 변경한다.(최대값)
        color.a = 1;
        //변경한 색상 정보를 대입해서 적용한다.
        img.color = color;
        //클릭 이펙트 루틴을 진행한다.
        StartCoroutine(Effect());
    }

    /// <summary>
    /// 클릭 이펙트 루틴
    /// </summary>
    IEnumerator Effect()
    {
        while (true)
        {
            //   (1틱 지난시간 / 유지시간)  지난 시간 비율을 계산한다.
            float timeScale = (Time.deltaTime / duringTime);
            // (1초간 변경될 사이즈의 비율 * 지난 시간 비율 )을 현재 스케일에 더한다.
            rectTransform.localScale += new Vector3(multipleScale, multipleScale, 0) * timeScale;
            //Alpha값 정보를 지난 시간 비율만큼 차감한다.
            Color color = img.color;
            color.a -= timeScale;
            img.color = color;
            yield return null;
            //Alpha값이 0.02 이하일 경우 루틴을 종료한다. 
            if (color.a <= 0.02f)
                break;
        }
        //객체를 오브젝트 풀에 반환한다.
        Release();
    }
}
