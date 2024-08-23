using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchTiming : PooledObject, IPointerDownHandler
{
    static readonly string Start = "Start";
    static readonly string Effect = "Effect";

    int startId;
    int effectId;
    public TouchTimingType currentTimingType;
    Func<float, TouchTiming.TouchTimingType> checkAction;
    Animator anim;
    public void Init(bool timer = false)
    {
        startId = Animator.StringToHash(Start);
        effectId = Animator.StringToHash(Effect);
        anim ??= GetComponent<Animator>();
        if (timer)
            anim.SetTrigger(startId);
        GetComponent<Image>().raycastTarget = true;
    }
    public void SetCheckAction(Func<float, TouchTiming.TouchTimingType> _checkAction)
        => checkAction = _checkAction;
    public void OnPointerDown(PointerEventData eventData)
    {
        PopNote();
        if (checkAction != null)
        {
            TouchTimingType type = checkAction.Invoke((transform as RectTransform).sizeDelta.x * transform.localScale.x);
#if UNITY_EDITOR
            Debug.Log($"{type}");
#endif
        }
    }
    public void PopNote()
    {
        anim.SetTrigger(effectId);
        GetComponent<Image>().raycastTarget = false;
        StartCoroutine(ReleaseRoutine(0.5f));
    }

    IEnumerator ReleaseRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Release();
    }
    public void Out()
    {
        Release();
    }
    public enum TouchTimingType
    {
        Miss,
        Bad,
        Normal,
        Good,
        Excellent,
        Perferct
    }
}
