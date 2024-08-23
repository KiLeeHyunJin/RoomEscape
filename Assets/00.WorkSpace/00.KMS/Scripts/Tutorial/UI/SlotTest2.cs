using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotTest2 : MonoBehaviour
{
    [SerializeField] InventorySlot slotPrefab; // InventorySlot 타입으로 변경
    private InventorySlot slotInstance;
    public Transform[] tutorialPanelTransform;
    public Transform originalSlot;
    public InventoryUI inventoryUI; // InventoryUI 참조 추가
    public GridLayoutGroup gridLayoutGroup; // GridLayoutGroup 참조 추가

    private bool hasItem = false;

    void Start()
    {
        CreateSlotInstance();
        UpdateSlotAppearance();
    }

    private void CreateSlotInstance() // 인벤토리 슬롯 대신 보여질 슬롯을 생성
    {
        // slotInstance를 slotPrefab으로 초기화
        slotInstance = Instantiate(slotPrefab);
        RectTransform rect = slotInstance.transform as RectTransform;
        RectTransform originRect = originalSlot.transform as RectTransform;
        rect.sizeDelta = gridLayoutGroup.cellSize; // GridLayoutGroup의 cellSize로 설정
        rect.localScale = Vector3.one; // 로컬 스케일 설정
        slotInstance.transform.SetParent(originalSlot.parent, false);

        // InventoryUI를 참고하여 슬롯 초기화
        slotInstance.Init(inventoryUI, 0); // 첫 번째 슬롯으로 초기화
        slotInstance.UpdateSlot(); // 슬롯 업데이트

        slotInstance.gameObject.SetActive(false);
    }

    private void UpdateSlotAppearance()
    {
        CheckForItems();
        if (hasItem)
        {
            slotInstance.gameObject.SetActive(true);
            MoveSlotPrefabToTutorialPanel();
        }
        else
        {
            slotInstance.gameObject.SetActive(false);
        }
    }

    void CheckForItems()
    {
        int itemCount = GetItemCount();
        if (itemCount >= 1)
        {
            hasItem = true;
        }
        else
        {
            hasItem = false;
        }
    }

    int GetItemCount()
    {
        int count = 0;
        if (InventoryManager.Instance == null)
        {
            return count;
        }

        int idx = InventoryManager.Instance.FindItemIndex("KeyPart2");
        if (idx >= 0)
        {
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }

    public void MoveSlotPrefabToTutorialPanel()
    {
        foreach (Transform panelTransform in tutorialPanelTransform)
        {
            if (panelTransform.gameObject.activeSelf)
            {
                slotInstance.transform.SetParent(panelTransform, false);
                RectTransform rectTransform = slotInstance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector3(-210, 275, 0);
                break;
            }
        }
    }

    void Update()
    {
        // 인벤토리에 특정 아이템이 있을 때만 UpdateSlotAppearance를 호출
        CheckForItems();
        if (hasItem)
        {
            UpdateSlotAppearance();
        }
    }
}
