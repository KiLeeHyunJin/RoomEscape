using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 영역 재설정(SafeArea)
/// </summary>
public class ResolutionChangeScaler : MonoBehaviour
{
    public RectTransform SafeArea { get { return safeArea; } }

    [SerializeField] RectTransform safeArea;
    const string Area = "safeArea";

    void Start()
    {
        //캔버스 스캐일러를 가져온다.
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        //캔버스 스캐일러의 스캐일 모드가 ScaleWithScreenSize가 아니라면 ScaleWithScreenSize로 설정한다.
        if (CanvasScaler.ScaleMode.ScaleWithScreenSize != scaler.uiScaleMode)
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        //스캐일 크기를 S20NoteUltra로 설정한다.
        scaler.referenceResolution = Define.S20;

        //세이프 에어리어가 비어있다면
        if (safeArea == null)
        {
            //세이프 에어리어를 찾는다.
            SearchSafeArea();
        }

        //세이프 에어리어 있다면
        if (safeArea != null)
        {
            //세이프 에어리어의 가장 작은 앵커를 UIManager에 있는 세이프 에어리어 작은 값을 대입한다.
            safeArea.anchorMin = Manager.UI.SafeAreaAnchorMin;
            //세이프 에어리어의 가장 큰 앵커를 UIManager에 있는 세이프 에어리어 큰 값을 대입한다.
            safeArea.anchorMax = Manager.UI.SafeAreaAnchorMax;
            //세이프 에어리어의 오프셋 범위를 0으로 설정한다.
            safeArea.offsetMax = safeArea.offsetMin = Vector2.zero;
        }
    }

    /// <summary>
    /// 세이프 에어리어를 찾는다.
    /// </summary>
    void SearchSafeArea()
    {
        //현재 오브젝트의 자식을 순회한다.
        foreach (Transform child in transform)
        {
            //자식의 이름이 "safeArea"인지 비교한다.
            if (string.Equals(Area,child.gameObject.name))
            {
                //자식의 활성화 여부가 비활성화라면 활성화로 변경한다.
                if (child.gameObject.activeSelf == false)
                    child.gameObject.SetActive(true);
                //자식의 트랜스폼을 다운캐스팅하고 safeArea에 대입한다.
                safeArea = child as RectTransform;
                return;
            }
        }
        //없을 경우 "safeArea"게임 오브젝트를 생성
        GameObject _safeArea = new(Area);
        //해당 오브젝트를 자식으로 배치
        _safeArea.transform.SetParent(this.transform);
        //해당 오브젝트를 다운 캐스팅 후 연결
        safeArea = _safeArea.transform as RectTransform;

        //오브젝트의 크기 및 위치를 초기화
        safeArea.localScale = Vector2.one;
        safeArea.anchoredPosition = Vector2.zero;
        safeArea.offsetMin = safeArea.offsetMax = Vector2.zero;
    }
}
