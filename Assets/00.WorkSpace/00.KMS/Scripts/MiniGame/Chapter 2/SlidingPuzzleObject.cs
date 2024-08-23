using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// SlidingPuzzleObject 클래스는 슬라이딩 퍼즐 조각을 나타내며, 
// 클릭했을 때 조각을 빈 칸으로 이동시키는 기능을 수행
public class SlidingPuzzleObject : MonoBehaviour, IPointerClickHandler
{
    // 슬라이딩 퍼즐 전체를 관리하는 SlidingPuzzle 클래스의 인스턴스를 저장하는 변수
    public SlidingPuzzle slidingPuzzle;

    // Start 메서드는 MonoBehaviour의 기본 메서드로, 게임 오브젝트가 활성화될 때 실행
    private void Start()
    {
        // 씬에서 SlidingPuzzle 객체를 찾아 slidingPuzzle 변수에 할당
        slidingPuzzle = FindObjectOfType<SlidingPuzzle>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭된 조각이 빈 칸과의 거리가 700f 이하일 경우에만 이동을 허용
        if (Vector2.Distance(slidingPuzzle.emptyPosition.anchoredPosition, ((RectTransform)transform).anchoredPosition) <= 700f)
        {
            // 현재 빈 칸의 위치를 저장
            Vector2 lastEmptyPosition = slidingPuzzle.emptyPosition.anchoredPosition;

            // 클릭된 조각의 위치를 빈 칸의 위치로 변경
            slidingPuzzle.emptyPosition.anchoredPosition = ((RectTransform)transform).anchoredPosition;

            // 빈 칸의 위치를 클릭된 조각의 이전 위치로 설정
            ((RectTransform)transform).anchoredPosition = lastEmptyPosition;

            // 퍼즐이 올바르게 맞춰졌는지 확인하는 메서드를 호출
            slidingPuzzle.CheckAnswer();
        }
    }
}
