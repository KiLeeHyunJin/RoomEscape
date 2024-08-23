using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickChanger : PooledObject, IPointerClickHandler
{
    public TextMeshProUGUI Text { get; private set; }
    ClickChangerSystem owner;
    int idx;
    public void SetParent(ClickChangerSystem _owner)
    {
        Text = GetComponentInChildren<TextMeshProUGUI>();
        owner = _owner;
        idx = 0;
        Text.text = owner.changerArrayNum[idx];
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        idx = ++idx < owner.changerArrayNum.Length ? idx : 0;
        Text.text = owner.changerArrayNum[idx];
        owner?.CheckPassword();
    }
}
