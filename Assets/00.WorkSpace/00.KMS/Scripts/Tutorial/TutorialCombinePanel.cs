using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialCombinePanel : MonoBehaviour//, IPointerClickHandler
{
    public GameObject tutorialCombinePanel;
    public Button combineButton;

    private bool isActivePanel = false;

    TutorialInventoryManager tutorialInventoryManager;
    InventoryManager inventoryManager;

    private void Start()
    {
        Message.Log("tutorialCombinePanel 진입");
        tutorialCombinePanel.SetActive(true);
        tutorialInventoryManager = FindObjectOfType<TutorialInventoryManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager != null)
        {
            inventoryManager.OnCombineSuccess += SuccessCombine;
            combineButton.onClick.RemoveListener(Manager.Inventory.CombineItems);
            combineButton.onClick.AddListener(OnCombineButtonClick);
        }

        // combineButton.onClick.AddListener(OnCombineButtonClick);
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnCombineSuccess -= SuccessCombine;
        }
    }

    public void OnCombineButtonClick()
    {
        if (inventoryManager != null)
        {
            inventoryManager.CombineItems();
        }
        else
        {
            Debug.LogError("inventoryManager가 초기화되지 않았습니다.");
        }
    }

    public void SuccessCombine()
    {
        Message.Log("조합 성공");
        if (!isActivePanel)
        {
            isActivePanel = true;
            Message.Log("다음 패널로");
            tutorialInventoryManager.NextPanel();
        }
    }
}