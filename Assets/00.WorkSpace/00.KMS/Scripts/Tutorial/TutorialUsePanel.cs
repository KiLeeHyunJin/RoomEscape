using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUsePanel : MonoBehaviour
{
    public GameObject tutorialUsePanel;
    public GameObject tutorialUseItemPanel;
    public GameObject inventoryPanel;
    public Button useButton;
    public Button transParentsButton;
    TutorialInventoryManager tutorialInventoryManager;
    InventoryManager inventoryManager;

    private void Start()
    {
        Message.Log("tutorialUsePanel 진입");
        tutorialInventoryManager = FindObjectOfType<TutorialInventoryManager>();
        inventoryManager = InventoryManager.Instance;

        if (useButton != null)
        {
            tutorialUsePanel.SetActive(true);
            transParentsButton.onClick.AddListener(OnUseButtonClick);
        }

        // transParentsButton 이벤트 리스너 추가
        // transParentsButton.onClick.RemoveListener(Manager.Inventory.UseItem);
        // transParentsButton.onClick.AddListener(OnUseButtonClick);
    }

    public void OnUseButtonClick()
    {
        // inventoryManager.UseItem();
        tutorialUsePanel.SetActive(false);
        TransParentsButtonClick();
        tutorialUseItemPanel.SetActive(true);
        // tutorialInventoryManager.NextPanel();
    }

    public void TransParentsButtonClick()
    {
        transParentsButton.gameObject.SetActive(false);
    }
}
