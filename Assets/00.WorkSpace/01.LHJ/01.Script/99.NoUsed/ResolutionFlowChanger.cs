using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ResolutionFlowChanger : MonoBehaviour
{
    struct ResolutionSizeData
    {
        public float width;
        public float height;

        public float originWidth;
        public float originHeight;

        public float originRatio;

        public float changeValue;
    }


    CanvasScaler scaler;
    Canvas canvas;
    RectTransform canvasSize;
    [SerializeField] RectTransform safeArea;

    /*[SerializeField]*/
    ResolutionSizeData resolution;
    // 해상도 대응을 해야하는 자식 객체
    RectTransform[] childs;
    private void Start()
    {
        //세이프 에어리어 설정
        if (safeArea != null)
        {
            Vector2 screenSize = new(Screen.width, Screen.height);

            safeArea.anchorMin = Screen.safeArea.min / screenSize;
            safeArea.anchorMax = Screen.safeArea.max / screenSize;

            safeArea.offsetMax = safeArea.offsetMin = Vector2.zero;
        }

        canvas = GetComponent<Canvas>();
        childs = GetComponentsInChildren<RectTransform>();
        canvasSize = transform as RectTransform;
        if (canvas == null)
            return;

        scaler = canvas.GetComponent<CanvasScaler>();
        //초기값 세팅
        if (scaler != null)
        {
            //가로 길이 저장
            resolution.originWidth = resolution.width = scaler.referenceResolution.x;
            //세로 길이 저장
            resolution.originHeight = resolution.height = scaler.referenceResolution.y;
            //비율 저장
            resolution.originRatio = scaler.referenceResolution.x / scaler.referenceResolution.y;

            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                //스케일 위드 설정이면 현재 기기 해상도로 설정
                scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            }
        }
        else
        {
            //초기값 저장
            resolution.originWidth = resolution.width = canvasSize.rect.width;
            resolution.originHeight = resolution.height = canvasSize.rect.height;
            resolution.originRatio = canvasSize.rect.width / canvasSize.rect.height;
        }
        //기준이 될 값을 저장
        resolution.changeValue = GetRatioValue();
        //자식들을 크기와 위치를 조정
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].sizeDelta *= resolution.changeValue;
            childs[i].anchoredPosition *= resolution.changeValue;
        }
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        Scaler();
#endif
    }

    void Scaler()
    {
        if (canvas == null)
            return;
        //변경했던 해상도와 같다면 종료
        if (Screen.width == resolution.width &&
            Screen.height == resolution.height)
            return;
        //기준이될 비율 저장
        float changeAnchorValue = GetRatioValue();

        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].TryGetComponent(out TextMeshProUGUI textMeshPro))
            {
                //오토 사이즈 설정
                textMeshPro.enableAutoSizing = true;
                //오토 사이즈 폰트 범위 설정
                textMeshPro.fontSizeMin = 1;
                textMeshPro.fontSizeMax = 999;
                //오프셋 범위 0으로 설정
                textMeshPro.rectTransform.offsetMax = textMeshPro.rectTransform.offsetMin = Vector2.zero;
                //한 Tick 이후 오토 사이즈 해제
                StartCoroutine(ChangeFont(textMeshPro));
            }
            else
            {
                //값을 복원 후 다시 재조정
                childs[i].sizeDelta = (childs[i].sizeDelta / resolution.changeValue) * changeAnchorValue;
                //childs[i].localScale = Vector3.one * changeAnchorValue;
                childs[i].anchoredPosition = (childs[i].anchoredPosition / resolution.changeValue) * changeAnchorValue;
                //만일 텍스트 컴포넌트를 갖고 있다면 Auto사이즈 조정
            }
        }
        //스케일 위드 설정이면 변경된 해상도로 설정
        if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        }
        //변경에 사용했던 값 저장 (값을 복원할때 사용)
        resolution.changeValue = changeAnchorValue;
        //현재 해상도 저장(변경 안되었을때 함수 빠르게 종료할때 사용)
        resolution.width = Screen.width;
        resolution.height = Screen.height;
    }

    float GetRatioValue()
    {
        //현재 비율 저장
        float currentRatio = (float)Screen.width / Screen.height;
        //변경된 비율이 가로가 큰지 세로가 큰지 비교
        if (currentRatio < resolution.originRatio)
        {
            //세로가 크면 변경된 가로 원본 기준 배수 반한
            return Screen.width / resolution.originWidth;
        }
        else
        {
            //세로 원본 기준 배수 반환
            return Screen.height / resolution.originHeight;
        }
    }

    IEnumerator ChangeFont(TextMeshProUGUI textMeshPro)
    {
        yield return null;
        textMeshPro.enableAutoSizing = false;
    }
}
