using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosetObject : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject closet;
    public Button closetButton;
    public ScriptableItem item;

    public void OpenCloset()
    {
        if (closet != null)
        {
            closet.SetActive(false);
        }

    }

    private void RewardItem()
    {
        Manager.Inventory.ObtainItem(item);
    }
}
