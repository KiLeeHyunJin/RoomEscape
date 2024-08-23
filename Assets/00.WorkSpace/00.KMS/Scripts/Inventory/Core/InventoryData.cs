using System;
using UnityEngine;


[Serializable]
public class InventoryData
{
    public InventoryData(int slotCount)
    {
        itemData = new(slotCount);

        itemArray = new ItemData[slotCount];
        for (int i = 0; i < itemArray.Length; i++)
        {
            itemArray[i].Init();
            itemData.saveArray[i].Init();
        }
    }
    [Serializable]
    public struct ItemSaveData
    {
        public void Init()
        {
            itemName = null;
            count = 0;
        }
        public void SetData(ScriptableItem _item, int _count)
        {
            count = _count;
            itemName = _item.name;
        }
        public string itemName;
        public int count;
    }
    [Serializable]

    public class ItemDataSave
    {
        public ItemDataSave(int slotCount)
        {
            saveArray = new ItemSaveData[slotCount];
        }
        [SerializeField] public ItemSaveData[] saveArray;
    }
    [Serializable]
    public struct ItemData
    {
        public ScriptableItem item;
        public SlotState state;
        public void Init()
        {
            SetData(null, SlotState.Empty);
        }

        public void SetData(ScriptableItem _item, SlotState _state)
        {
            item = _item;
            state = _state;
        }
    }

    [SerializeField] ItemData[] itemArray;
    [SerializeField] ItemDataSave itemData;
    public ItemSaveData[] ReadData { get { return itemData.saveArray; } }
    public string SaveJosonData()
    {
        return JsonUtility.ToJson(itemData);
    }
    public int Count
    {
        get
        {
            return itemArray.Length;
        }
    }
    public int EmptyCount
    {
        get
        {
            int count = default;
            foreach (var slotData in itemArray)
            {
                if (slotData.state == SlotState.Empty)
                    count++;
            }
            return count;
        }
    }
    public int EmptyIndex
    {
        get
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i].state == SlotState.Empty)
                    return i;
            }
            return -1;
        }
    }

    public void Init()
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            itemArray[i].Init();
            itemData.saveArray[i].Init();
        }
    }
    public bool FillCheck(int idx)
    {
        return itemArray[idx].state == SlotState.Fill;
    }

    public (Sprite icon, int count) this[int index]
    {
        get
        {
            if (itemArray[index].state == SlotState.Empty)
                return (null, -1);

            Sprite icon = itemArray[index].item != null ? itemArray[index].item.itemIcon : null;
            return (icon, itemData.saveArray[index].count);
        }
    }

    public void SwapItem(int idx1, int idx2)
    {
        ItemData idx1Item = itemArray[idx1];
        ItemSaveData idx1Data = itemData.saveArray[idx1];

        itemArray[idx1] = itemArray[idx2];
        itemData.saveArray[idx1] = itemData.saveArray[idx2];

        itemArray[idx2] = idx1Item;
        itemData.saveArray[idx2] = idx1Data;
    }

    public void SetItem(int idx, ScriptableItem item, int count)
    {
        itemArray[idx].SetData(item, SlotState.Fill);
        itemData.saveArray[idx].SetData(item, count);
    }


    public bool AddItem(ScriptableItem item, int count = 1)
    {
        int emptyIdx = -1;
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].state == SlotState.Fill)
            {
                if (string.Equals(item.name, itemData.saveArray[i].itemName))
                {
                    itemData.saveArray[i].count += count;
                    Message.Log($"{item.name} Add Item Count {count}");
                    return true;
                }
            }
            else if (emptyIdx < 0)
            {
                emptyIdx = i;
            }
        }
        if (emptyIdx >= 0)
        {
            itemArray[emptyIdx].SetData(item, SlotState.Fill);
            itemData.saveArray[emptyIdx].SetData(item, count);
            return true;
        }
        return false;
    }

    public bool RemoveItem(int index, int count = 1)
    {
        if (itemArray[index].state == SlotState.Fill)
        {
            if (itemData.saveArray[index].count >= count)
            {
                itemData.saveArray[index].count -= count;
                if (itemData.saveArray[index].count <= 0)
                {
                    itemData.saveArray[index].Init();
                    itemArray[index].Init();
                }
                return true;
            }
        }
        return false;
    }
    public bool RemoveItmeName(string itemName, int count = 1)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].state == SlotState.Fill)
            {
                if (string.Equals(itemData.saveArray[i].itemName, itemName))
                {
                    return RemoveItem(i, count);
                }
            }
        }
        return false;
    }
    public bool RemoveItem(ScriptableItem item, int count = 1)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].state == SlotState.Fill)
            {
                if (string.Equals(itemData.saveArray[i].itemName, item.name))
                {
                    return RemoveItem(i, count);
                }
            }
        }
        return false;
    }

    public (Sprite icon, int count) GetSlotData(int index)
    {
        if (itemArray[index].state == SlotState.Empty)
            return (null, -1);
        return (itemArray[index].item != null ? itemArray[index].item.itemIcon : null, itemData.saveArray[index].count);
    }

    public int FindIndex(ScriptableItem item)
    {
        return item == null ? -1 : FindIndex(item.name);
    }

    public int FindIndex(string itemName)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].state == SlotState.Fill)
            {
                if (string.Equals(itemName, itemData.saveArray[i].itemName))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public int FindIndex(params string[] itemName)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i].state == SlotState.Fill)
            {
                foreach (string itemCheckName in itemName)
                {
                    if (string.Equals(itemCheckName, itemData.saveArray[i].itemName))
                    {
                        return i;
                    }
                }

            }
        }
        return -1;
    }

    public PopUpUI GetPopupUI(int idx)
    {
        return itemArray[idx].item == null ? null : itemArray[idx].item.popUpUI;
    }

    public string GetItemName(int idx)
    {
        if (itemArray[idx].state == SlotState.Fill)
            return itemData.saveArray[idx].itemName;
        return "";
    }

    public ScriptableItem GetItem(int index)
    {
        if (index < 0 || index >= itemArray.Length)
            return null;

        return itemArray[index].item;
    }

    public enum SlotState
    {
        Empty,
        Fill
    }

}
