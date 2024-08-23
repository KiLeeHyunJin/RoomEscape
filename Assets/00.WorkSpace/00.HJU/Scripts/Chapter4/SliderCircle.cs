using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderCircle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public SliderLock sliderLock;
    


    public int id;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (sliderLock == null)
        {
            Debug.LogError("sliderLock is not set for " + gameObject.name);
            return;
        }
        sliderLock.OnMouseDownCircle(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sliderLock == null)
        {
            Debug.LogError("sliderLock is not set for " + gameObject.name);
            return;
        }
        sliderLock.OnMouseEnterCircle(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sliderLock == null)
        {
            Debug.LogError("sliderLock is not set for " + gameObject.name);
            return;
        }
        sliderLock.OnMouseExitCircle(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (sliderLock == null)
        {
            Debug.LogError("sliderLock is not set for " + gameObject.name);
            return;
        }
        sliderLock.OnMouseUpCircle(this);
    }
}
