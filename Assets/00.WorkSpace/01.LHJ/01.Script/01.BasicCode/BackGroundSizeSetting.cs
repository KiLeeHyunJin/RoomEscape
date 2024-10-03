using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챕터 필드의 크기 비율을 재설정한다.
/// </summary>
public class BackGroundSizeSetting : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;
    private void Start()
    {
        InitSize();
    }
    /// <summary>
    /// 하위 객체의 사이즈를 재설정한다.
    /// </summary>
    public void InitSize()
    {
        StartCoroutine(SynchRatio());
    }
    /// <summary>
    /// 1프레임 이후 비율 설정한다.
    /// </summary>
    IEnumerator SynchRatio()
    {
        yield return new WaitForEndOfFrame();
        FlowSynch();
    }

    /// <summary>
    /// 하위 객체 크기 비율 설정한다.(챕터 필드)
    /// </summary>
    void FlowSynch()
    {
        //자식이 없을 경우 종료
        if (transform.childCount == 0)
            return;

        //캔버스가 없을 경우 가져오기
        if (scaler == null)
            scaler = GetComponentInParent<CanvasScaler>();
        //디바이스 비율 계산
        float deviceHeightRatio = 
            Screen.height / (scaler.transform as RectTransform).localScale.x;

        //
        foreach (Transform parent in transform)
        {
            RectTransform parentRect = parent as RectTransform;
            float height = parentRect.rect.height;
            
            if (height <= 0)
                continue;

            //비율을 계산한다.
            float adjustmentValue = deviceHeightRatio / height;

            parentRect.sizeDelta *= adjustmentValue;//현재 크기에 비율을 곱한다.
            parentRect.localScale = Vector3.one;//크기 초기화

            foreach (Transform child in parent)
            { //자식들은 위치 및 스캐일만 비율로 곱한다.
                SetChildRatio(child, adjustmentValue);
            }
        }
    }

    void SetChildRatio(Transform child, float adjustmentValue)
    {
        RectTransform childRect = child as RectTransform;
        childRect.anchoredPosition *= adjustmentValue;
        childRect.localScale *= adjustmentValue;
    }
}
