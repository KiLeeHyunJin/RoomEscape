using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenCircle : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] Image _myImage;
    [SerializeField] WrongImageQuiz quiz;

    [Header("TrueEffect")]
    [SerializeField] float filledTime;
    [SerializeField] float operateTime;
    [SerializeField] float delay;

    [Header("HintEffect")]
    [SerializeField] int twincleCount;
    [SerializeField] float twincleCooltime;
    private void OnEnable()
    {
        if (quiz._Founded == true)
        {
            _myImage = GetComponent<Image>();
            _myImage.fillAmount = 0;
            StartCoroutine(CircleSummon());
            Debug.Log(quiz._Founded);
        }
        else
        {
            Debug.Log(quiz._Founded);
            _myImage = GetComponent<Image>();
            _myImage.fillAmount = 1;
            HintView();
        }
    }
    IEnumerator CircleSummon()
    {
        _myImage.color = new Color(1, 1, 1, 1);
        while (filledTime < operateTime)
        {
            yield return new WaitForSeconds(delay);
            filledTime += delay;
            UpdateImage(filledTime / operateTime);
        }
    }
    private void HintView()
    {
        StartCoroutine(HintSummon());
    }
    IEnumerator HintSummon()
    {
        for (int i = 0; i < twincleCount; i++)
        {
            _myImage.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(twincleCooltime);
            _myImage.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(twincleCooltime);
            Debug.Log($"Twincle Count : {i}");
        }
        _myImage.color = new Color(1, 1, 1, 1);
        gameObject.SetActive(false);
    }
    private void UpdateImage(float fillAmount)
    {
        _myImage.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
