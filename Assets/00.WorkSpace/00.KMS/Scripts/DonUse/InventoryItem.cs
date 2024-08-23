using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour // 인벤토리에 슬롯에 들어가는 아이템 스크립트
{
    [Header("Components")]
    private RectTransform rectTransform;
    private Image image;

    [Header("Data")]
    private ScriptableItem item;

    private Transform parent;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        parent = transform.parent;
    }

    private InventorySlot GetCurSlot()
    {
        return transform.parent.GetComponent<InventorySlot>();
    }
}

