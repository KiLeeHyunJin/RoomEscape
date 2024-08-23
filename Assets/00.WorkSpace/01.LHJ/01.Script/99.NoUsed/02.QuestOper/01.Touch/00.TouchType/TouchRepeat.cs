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
        //��ǥ�� �޻������� ����
        if (currentFillValue >= 1)
            return;
        //��ġ�� ��ġ�� �����Ѵ�.
        Vector2 touchPos = Input.mousePosition;
        //��ġ�� ��ġ�� �����ѿ� �������� Ȯ���Ѵ�.
        if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, touchPos))
        {
            //�������� ä���ش�.
            currentFillValue += fillValue;
            //��ƾ�� �������� �ƴϸ� ������ ��Ų��.
            if (chargeCo == null)
                chargeCo = StartCoroutine(UpGaugeRoutine());
        }
    }


    IEnumerator UpGaugeRoutine()
    {
        float currentVelocity = 0.1f;
        while (true)
        {
            //�������� �̹����� ä���.
            fillRect.fillAmount = Mathf.SmoothDamp(fillRect.fillAmount, currentFillValue, ref currentVelocity, 0.2f,1, Time.deltaTime);
            yield return null;
            //���� �ʱⰪ�� �ƴϰ� ��ǥ���� �ƴϸ� ä�� ���� ��������.
            if (currentFillValue < 1 && currentFillValue > 0)
            {
                currentFillValue -= Time.deltaTime * minusValue;
            }
            //��ǥ���� ���������� ����
            if (fillRect.fillAmount >= 1)
            {
                Clear();
                break;
            }
            // ���� ���������� �ʱⰪ���� ���� �� ����
            else if (fillRect.fillAmount <= 0)
            {
                currentFillValue = 0;
                break;
            }
        }
        //��ƾ ����
        chargeCo = null;
    }

    void Clear()
    {
        //��ǥġ�� ���� �ٲ� �� �Ҵ� ������ �Ѵ�.
        fillRect.fillAmount = 1;
        currentFillValue = 1;
        Debug.Log("Clear!");
        Addressables.Release(gameObject);
        Destroy(gameObject);
    }
}
