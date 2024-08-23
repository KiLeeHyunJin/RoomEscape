using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedXImage : PooledObject
{
    [SerializeField] Image[] redlines;

    [Header("Time")]
    [SerializeField] float filledTime;
    [SerializeField] float operatedTime;
    [SerializeField] float delay;
    [SerializeField] float offTime;

    [Header("DecreaseTime")]
    [SerializeField] float decreaseTime;

    private void Start()
    {
        ResetFill();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        filledTime = 0;
        for (int i = 0; i < redlines.Length; i++)
        {
            redlines[i].fillAmount = 0;
            redlines[i].color = new Color(1, 1, 1, 1);
        }
        StartCoroutine(XEffect());
    }

    private void ResetFill()
    {
        for (int i = 0; i < redlines.Length; i++)
        {
            redlines[i].fillAmount = 0;
            redlines[i].color = new Color(1, 1, 1, 1);
        }
    }

    IEnumerator XEffect()
    {
        for (int i = 0; i < redlines.Length; i++)
        {
            redlines[i].fillAmount = 0;
            redlines[i].color = new Color(1, 1, 1, 1);
        }
        yield return new WaitForSeconds(0.01f);
        for (int i = 0; i < redlines.Length;i++)
        {
            filledTime = 0;
            while (filledTime < operatedTime)
            {
                yield return new WaitForSeconds(delay);
                filledTime += delay;
                redlines[i].fillAmount = filledTime / operatedTime;
            }
        }

        float deleteTime = 1;

        while(deleteTime > 0)
        {
            deleteTime -= delay;
            yield return new WaitForSeconds(delay);
            redlines[0].color = new Color(1, 1, 1, deleteTime);
            redlines[1].color = new Color(1, 1, 1, deleteTime);
        }
        filledTime = 0;
        Release();
    }
}
