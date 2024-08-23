using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ScriptableItem : ScriptableObject, IComparable<ScriptableItem>
{
    public string itemID; // newChart.korean
    public string description; // newChart.korean
    public int itemNum; // 10000번 부터 시작하는 TableID
    public int descriptionNum; // 10100번부터 시작하는 TableID

    public Sprite itemIcon;
    public PopUpUI popUpUI;
    // public Dictionary<Item, int> requiredItems; // 아이템을 조합할 때 필요한 아이템과 그 수량

    public ScriptableItem[] requires; // 아이템 조합을 할 때 필요한 아이템

    public int CompareTo(ScriptableItem other) // 아이템들을 비교
    {
        return name.CompareTo(other.name);
    }
}
