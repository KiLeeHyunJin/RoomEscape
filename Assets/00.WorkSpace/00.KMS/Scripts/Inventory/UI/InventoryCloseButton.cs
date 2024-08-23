using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCloseButton : MonoBehaviour
{
    public GameObject inventoryPanel;
    //bool activeInventory;

    /// <summary>
    /// 인밴토리 OnOff메소드다.
    /// </summary>
    void ToggleInventory()
    {
        //activeInventory = !activeInventory; // 인벤토리 버튼 클릭 시 인벤토리 활성화 상태를 토글
        //inventoryPanel.SetActive(activeInventory);
    }

    public void CloseInventory()
    {
       // activeInventory = false;
        inventoryPanel.SetActive(false);
    }
}
