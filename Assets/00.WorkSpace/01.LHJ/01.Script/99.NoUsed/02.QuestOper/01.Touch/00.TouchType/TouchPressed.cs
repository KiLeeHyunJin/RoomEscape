using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class TouchPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] public float pressedTime { get; private set; }
    float operatTime;
    Coroutine chargeCo;
    [SerializeField] RectTransform parent;
    public void SetTime(float _time)
    {
        operatTime = _time;
    }
    void OnEnable()
    {

    }

    //��ư�� ������ �����ϸ� ��¡������ ��ƾ ����
    public void OnPointerDown(PointerEventData eventData)
    {
        chargeCo = StartCoroutine(ChargingGaugeRoutine());
    }
    //��ư�� ���ٸ� ��ƾ ����
    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(chargeCo);
    }
    IEnumerator ChargingGaugeRoutine()
    {
        //���� �ð� �ʱ�ȭ
        pressedTime = 0;
        while (true)
        {
            //0.5�ʸ��� ��¡
            yield return new WaitForSeconds(0.5f);
            pressedTime += 0.5f;
            if(pressedTime >= operatTime)
            {
                Clear();
            }
        }
    }
    void Clear()
    {
        Addressables.Release(parent.gameObject);

    }
}
