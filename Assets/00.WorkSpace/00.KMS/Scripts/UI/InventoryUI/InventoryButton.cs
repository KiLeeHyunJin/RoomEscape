using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour // 인벤토리 UI를 제어하는 클래스
{
    GameObject inventoryPanel;
    [SerializeField] LoadGameTable chapterTable;
    bool activeInventory;
    bool inventoryLink = false;
    private void Start()
    {
        chapterTable = GetComponentInParent<LoadGameTable>();
        if (inventoryLink == false)
        {
            inventoryPanel = chapterTable.InventoryPanel;
            Init();
        }
        activeInventory = false;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    public void SetInventoryPanel(GameObject _inventoryPanel)
    {
        inventoryPanel = _inventoryPanel;
        Init();
    }

    void Init()
    {
        if (inventoryPanel == null || inventoryLink)
            return;
        inventoryLink = true;
        inventoryPanel.SetActive(false);
        GetComponentInChildren<Button>().onClick.AddListener(ToggleInventory);
        Manager.Inventory.CloseAction(ToggleInventory);
    }


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        Debug.Log("인벤토리 열림/닫힘");
    //        //activeInventory = !activeInventory;
    //        //if (inventoryPanel == null)
    //        //    return;
    //        //inventoryPanel.SetActive(activeInventory);
    //    }
    //}

    void ToggleInventory()
    {
        //bool activeInventory = inventoryPanel.activeSelf;
        activeInventory = !activeInventory; // 인벤토리 버튼 클릭 시 인벤토리 활성화 상태를 토글
        if (inventoryPanel == null)
            return;
        inventoryPanel.SetActive(activeInventory);
    }

    //public void Open() // 인벤토리를 여는 메서드
    //{
    //    //activeInventory = true;
    //    //if (inventoryPanel == null)
    //    //    return;
    //    //inventoryPanel.SetActive(true);
    //}

    //public void Close() // 인벤토리를 닫는 메서드
    //{
    //    //activeInventory = false;
    //    //if (inventoryPanel == null)
    //    //    return;
    //    //inventoryPanel.SetActive(false);
    //}
}
