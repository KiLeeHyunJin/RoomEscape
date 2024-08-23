using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUseItemPanel : MonoBehaviour
{
    public GameObject tutorialUseItemPanel;
    public GameObject tutorialUsePanel;
    public GameObject inventoryPanel;
    public Button useItemButton;
    TutorialInventoryManager tutorialInventoryManager;
    InventoryManager inventoryManager;

    private void Start()
    {
        Manager.Text.TextChange();
        Message.Log("TutorialUseItemPanel 진입");
        tutorialInventoryManager = FindObjectOfType<TutorialInventoryManager>();
        inventoryManager = InventoryManager.Instance;

        // useItemButton = GetComponentInChildren<Button>();
        if (useItemButton != null)
        {
            // Manager.Inventory.UseItem(); // OnSuccessUseItem
            tutorialUseItemPanel.SetActive(true);
            useItemButton.onClick.AddListener(OnUseItemButtonClick);
        }
    }

    public void OnUseItemButtonClick()
    {
        Message.Log("아이템 사용 버튼 클릭");
        // Manager.Inventory.OnSuccessUseItem();
        inventoryPanel.SetActive(false);
        // tutorialInventoryManager.NextPanel(); // 필요시 추가
    }
}
