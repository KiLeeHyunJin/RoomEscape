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

    //버튼을 누르기 시작하면 차징게이지 루틴 실행
    public void OnPointerDown(PointerEventData eventData)
    {
        chargeCo = StartCoroutine(ChargingGaugeRoutine());
    }
    //버튼을 땐다면 루틴 종료
    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(chargeCo);
    }
    IEnumerator ChargingGaugeRoutine()
    {
        //시작 시간 초기화
        pressedTime = 0;
        while (true)
        {
            //0.5초마다 차징
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
