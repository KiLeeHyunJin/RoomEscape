using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimingGame : MonoBehaviour
{
    public GameObject Button;
    public bool _success = false;
    public bool _clicked = false;
    public RectTransform imageRectTransform; // 이미지 RectTransform
    public RectTransform image2RectTransform; // 버튼 RectTransform
    private Vector2 startSize; // 이미지 시작 크기
    private Vector2 endSize; // 이미지 끝 크기
    private float startTime; // 시작 시간
    private float duration = 5.0f; // 사라지는 데 걸리는 시간 (초)

    void Start()
    {
        startSize = imageRectTransform.sizeDelta; // 이미지 시작 크기 저장
        endSize = Vector2.zero; // 이미지 끝 크기 (0, 0)으로 설정
        startTime = Time.time; // 시작 시간 저장
    }

    void Update()
    {
        _success = false;
        _clicked = false;
        float progress = (Time.time - startTime) / duration; // 진행 상황 계산 (0에서 1 사이)
        Vector2 newSize = Vector2.Lerp(startSize, endSize, progress); // lerp 함수를 사용하여 새로운 크기 계산
        imageRectTransform.sizeDelta = newSize; // 이미지 크기 변경
        if (imageRectTransform.sizeDelta.x <= image2RectTransform.sizeDelta.x)
        {
            _clicked = true;
            Destroy(gameObject);
        }
    }
    private void OnMouseDown()
    {
        PointJudgement();
    }
    public void PointJudgement()
    {
        if (imageRectTransform.sizeDelta.x - image2RectTransform.sizeDelta.x >= 0 && imageRectTransform.sizeDelta.x - image2RectTransform.sizeDelta.x <= 80)
        {
            Debug.Log("정확");
            _success = true;
            _clicked = true;
            Destroy(gameObject);
        }
        else if (imageRectTransform.sizeDelta.x - image2RectTransform.sizeDelta.x >= 80 && imageRectTransform.sizeDelta.x - image2RectTransform.sizeDelta.x <= 160)
        {
            _clicked = true;
            Debug.Log("꽤 정확");
            Destroy(gameObject);
        }
        else
        {
            _clicked = true;
            Debug.Log("틀림");
            Destroy(gameObject);
        }
    }
}
