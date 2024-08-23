using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisassembleTutorialPanel : MonoBehaviour
{
    // 인벤토리 슬롯에 KeyBox가 있는 경우, disassembleTutorial이 활성화 keyBox가 없으면 disassembleTutorial 비활성화
    public GameObject disassembleTutorial;
    public GameObject disassembleResult;
    public GameObject inventoryPanel;
    public Button inventoryButton;
    public Button disassembleButton;
    private bool hasObtainedKeyBox = false;
    private bool isActivePanel = false;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        disassembleTutorial.SetActive(false);
        inventoryButton.onClick.AddListener(OnInventoryClick);
        disassembleButton.onClick.AddListener(DisassembleResult);
    }

    private void Update()
    {
        CheckInventoryPanelState();
    }

    private void CheckInventoryPanelState()
    {
        if (!inventoryPanel.activeSelf)
        {
            disassembleResult.SetActive(false);
        }
    }

    private void CheckKeyBox()
    {
        int keyPartCount = GetKeyBoxCount();
        if (keyPartCount >= 1)
        {
            hasObtainedKeyBox = true;
        }
    }

    int GetKeyBoxCount()
    {
        int count = 0;
        int idx = InventoryManager.Instance.FindItemIndex("KeyPart2Box"); // 인벤토리에 해당 아이템이 있는지 확인
        if (idx >= 0)
        {
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }
    public void OnInventoryClick()
    {
        CheckKeyBox();
        if (hasObtainedKeyBox && !isActivePanel)
        {
            ShowHintPopUp();
            Manager.Text.TextChange();
        }
    }

    private void ShowHintPopUp()
    {
        disassembleTutorial.SetActive(true);
        isActivePanel = true;
        Manager.Text.TextChange();
    }

    public void DisassembleResult() // 분해 결과 패널
    {
        disassembleTutorial.SetActive(false);
        disassembleResult.SetActive(true);
    }

}
