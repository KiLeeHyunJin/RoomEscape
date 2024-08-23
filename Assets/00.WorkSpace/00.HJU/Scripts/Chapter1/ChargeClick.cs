using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChargeClick : PopUpUI, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float pressedTime;
    [SerializeField] float operatTime;
    [SerializeField] float updateInterval = 0.05f;

    [SerializeField] GameObject dougImage;
    [SerializeField] GameObject cooKieImage;
    [SerializeField] GameObject chargeImage;
    [SerializeField] GameObject clearMessage;

    [SerializeField] Coroutine chargeRoutine;
    [SerializeField] Image gaugeImage;
    [SerializeField] Image gaugeImage2;

    [SerializeField] ScriptableItem item;

    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData) 
    {
        chargeRoutine = StartCoroutine(ChargingRoutine());
        Manager.Sound.PlayButtonSound(33);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(chargeRoutine);
        ResetGauge();
        Manager.Sound.StopSFX();
    }

    IEnumerator ChargingRoutine()
    {
        //시작 시간 초기화
        pressedTime = 0;
        while (pressedTime < operatTime)
        {
            //0.5초마다 차징
            yield return new WaitForSeconds(updateInterval);
            pressedTime += updateInterval;
            UpdateGauge(pressedTime / operatTime);
        }
        Clear();
    }

    private void UpdateGauge(float fillAmount)
    {
        gaugeImage.fillAmount = Mathf.Clamp01(fillAmount);
        gaugeImage2.fillAmount = Mathf.Clamp01(fillAmount);
    }

    // 게이지를 초기화하는 메소드
    private void ResetGauge()
    {
        pressedTime = 0;
        gaugeImage.fillAmount = 0;
        gaugeImage2.fillAmount= 0;

    }

    private void Clear()
    {
        Manager.Chapter._clickObject.state = 3;
        Manager.Chapter._clickObject.GetItem(item);
        if (changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
        dougImage.SetActive(false);
        cooKieImage.SetActive(true);
        chargeImage.SetActive(false);
        clearMessage.SetActive(true);
        ResetGauge();
        StopCoroutine(chargeRoutine);
        Manager.Sound.StopSFX();
    }
}

