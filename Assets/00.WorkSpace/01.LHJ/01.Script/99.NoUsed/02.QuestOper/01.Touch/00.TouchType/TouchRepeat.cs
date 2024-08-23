using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchRepeat : MonoBehaviour, IPointerDownHandler
{
    public Action action;
    [SerializeField] Image fillRect;
    float fillValue;
    float currentFillValue;
    Coroutine chargeCo;
    [Range(1,15)]
    [SerializeField] int fillDividValue;
    [Range(0,1)]
    [SerializeField] float minusValue;
    void Start()
    {
        SetTouchCount(10);
    }
    public void SetTouchCount(int _count)
    {
        fillValue = 0;
        if (fillDividValue != 0)
            _count = fillDividValue;
        fillValue =  1f / _count ;
        if (minusValue == 0)
            minusValue = 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //목표에 달생했으면 종료
        if (currentFillValue >= 1)
            return;
        //터치한 위치를 저장한다.
        Vector2 touchPos = Input.mousePosition;
        //터치한 위치가 영역한에 존재한지 확인한다.
        if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, touchPos))
        {
            //일정량을 채워준다.
            currentFillValue += fillValue;
            //루틴이 진행중이 아니면 진행을 시킨다.
            if (chargeCo == null)
                chargeCo = StartCoroutine(UpGaugeRoutine());
        }
    }


    IEnumerator UpGaugeRoutine()
    {
        float currentVelocity = 0.1f;
        while (true)
        {
            //비선형으로 이미지를 채운다.
            fillRect.fillAmount = Mathf.SmoothDamp(fillRect.fillAmount, currentFillValue, ref currentVelocity, 0.2f,1, Time.deltaTime);
            yield return null;
            //값이 초기값도 아니고 목표값도 아니면 채운 양을 지워낸다.
            if (currentFillValue < 1 && currentFillValue > 0)
            {
                currentFillValue -= Time.deltaTime * minusValue;
            }
            //목표값에 도달했으면 종료
            if (fillRect.fillAmount >= 1)
            {
                Clear();
                break;
            }
            // 전부 지워졌으면 초기값으로 변경 및 종료
            else if (fillRect.fillAmount <= 0)
            {
                currentFillValue = 0;
                break;
            }
        }
        //루틴 종료
        chargeCo = null;
    }

    void Clear()
    {
        //목표치로 전부 바꾼 후 할당 해제를 한다.
        fillRect.fillAmount = 1;
        currentFillValue = 1;
        Debug.Log("Clear!");
        Addressables.Release(gameObject);
        Destroy(gameObject);
    }
}
