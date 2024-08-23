using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlashChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Image checkBox;
    Transform checkBoxParent;
    public RectTransform start;
    public RectTransform end;

    public bool currentState { get; private set; }
    Coroutine checkCo;

    void Awake()
    {
        //터치 영역을 생성한다.
        Init();
    }
    void Init()
    {
        //값을 초기화한다.
        checkBox = null;
        //체크 영역의 부모를 시작 지점으로 지정한다.
        checkBoxParent = start.transform;
        if (start != null)
            CreateCheckBox();
    }

    void CreateCheckBox()
    {
        //체크 박스가 없다면
        if(checkBox == null)
        {
            //체크박스 생성
            GameObject obj = new("checkBox");
            checkBox = obj.AddComponent<Image>();
            //객층 구조 조정
            obj.transform.SetParent(checkBoxParent);
        }

        //길이 및 사이즈 조정
        checkBox.rectTransform.sizeDelta =
            new Vector2(
                start.sizeDelta.x,
                Vector2.Distance(start.position, end.position) + (start.sizeDelta.y * 2)
            );
        checkBox.rectTransform.position = (start.position + end.position) * 0.5f;

        //회전 방향 설정
        Vector2 direction = (end.position - start.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //세로 방향으로 보고 있기때문에 -90
        angle -= 90;
        checkBox.rectTransform.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    //출발 버튼을 출렸을때
    public void OnPointerDown(PointerEventData eventData)
    {
        //터치 지점이 시작지점의 객체 범위 안인지 확인한다.
        if(Vector2.Distance(eventData.position, start.position) < start.sizeDelta.magnitude)
        {
            //체크 범위를 벗어났는지 확인한다.
            checkCo = StartCoroutine(PressCheckRoutine());
            Debug.Log("시작!");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //도착 위치 저장
        if(checkCo != null)
        {
            //터치를 땐 부분이 도착지점인지 확인한다.
            if (Vector2.Distance(eventData.position, end.position) < end.sizeDelta.magnitude)
            {
                //endPos = eventData.position;
                StopCoroutine(checkCo);
#if UNITY_EDITOR
                Debug.Log("도착!");
#endif
                if (checkBox != null)
                {
                    checkBox.gameObject.SetActive(false);
                    //Destroy(checkBox.gameObject);
                }
            }
        }

    }
    IEnumerator PressCheckRoutine()
    {
        currentState = true;
        do
        {
            //터치 범위가 이동 경로 안에 위치해있는지 확인
            if (CursorCheck(Input.mousePosition))
            {
#if UNITY_EDITOR
                Debug.Log("진입!");
#endif
            }
            else
            {
                currentState = false;
#if UNITY_EDITOR
                Debug.Log("탈락!");
#endif
            }
            yield return null;
        }
        while (currentState);
    }

    bool CursorCheck(Vector2 pos)
    {
        //터치 범위가 체크 범위 안인지 확인한다.
        return RectTransformUtility.RectangleContainsScreenPoint(checkBox.rectTransform, pos);
    }
}
