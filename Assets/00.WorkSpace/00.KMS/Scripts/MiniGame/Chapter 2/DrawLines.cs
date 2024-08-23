using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DrawLines : MonoBehaviour
{
    private RectTransform imageRectTransform; // 선 위치
    public RectTransform pointA; // 선의 시작점과 끝
    public RectTransform pointB;
    public float lineWitdh; // 선 두께

    private void Start()
    {
        imageRectTransform = GetComponent<RectTransform>(); // 연결된 오브젝트의 RectTransform
    }

    private void Update()
    {
        Vector3 differenceVector = pointB.anchoredPosition - pointA.anchoredPosition; // 선의 A와 B의 거리를 계산

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWitdh); // 선의 크기를 백터의 크기로 설정 두께도 설정
        imageRectTransform.pivot = new Vector2(0, 0.5f); // 선의 피벗을 시작점으로 설정하여 pointA에서 시작
        imageRectTransform.anchoredPosition = pointA.anchoredPosition; // 선의 시작 위치를 pointA로 설정
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg; // 선이 pointA에서 pointB를 향하도록 회전 각도를 계산
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle); // 계산된 각도를 사용하여 선을 회전
    }
}
