using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalTest : MonoBehaviour
{
    [SerializeField] InventorySlot slotPrefab; // InventorySlot 타입으로 변경
    private InventoryIcon icon;
    private InventorySlot[] slotInstances;
    public Transform[] tutorialPanelTransform;
    public Transform originalSlot;
    public InventoryUI inventoryUI; // InventoryUI 참조 추가
    public GridLayoutGroup gridLayoutGroup; // GridLayoutGroup 참조 추가

    private bool[] hasItems = new bool[3];
    private string[] itemNames = { "KeyPart2Box", "KeyPart2", "KeyPart1" };
    private Vector3[] slotPositions = { new Vector3(-210, 275, 0), new Vector3(-210, 275, 0), new Vector3(-70, 275, 0) };

    void Start()
    {
        slotInstances = new InventorySlot[3];
        for (int i = 0; i < slotInstances.Length; i++)
        {
            CreateSlotInstance(i);
            UpdateSlotAppearance(i);
        }
    }

    private void CreateSlotInstance(int index)
    {
        // slotInstance를 slotPrefab으로 초기화
        slotInstances[index] = Instantiate(slotPrefab);
        RectTransform rect = slotInstances[index].transform as RectTransform;
        rect.sizeDelta = gridLayoutGroup.cellSize; // GridLayoutGroup의 cellSize로 설정
        rect.localScale = Vector3.one; // 로컬 스케일 설정
        slotInstances[index].transform.SetParent(originalSlot.parent, false);

        // InventoryUI를 참고하여 슬롯 초기화
        slotInstances[index].Init(inventoryUI, index); // 슬롯 초기화
        slotInstances[index].UpdateSlot(); // 슬롯 업데이트

        slotInstances[index].gameObject.SetActive(false);
    }

    private void UpdateSlotAppearance(int index)
    {
        CheckForItems(index);
        if (hasItems[index])
        {
            slotInstances[index].gameObject.SetActive(true);
            MoveSlotPrefabToTutorialPanel(index);
        }
        else
        {
            slotInstances[index].gameObject.SetActive(false);
        }
    }

    void CheckForItems(int index)
    {
        int itemCount = GetItemCount(index);
        hasItems[index] = itemCount >= 1;
    }

    int GetItemCount(int index)
    {
        int count = 0;
        int idx = InventoryManager.Instance.FindItemIndex(itemNames[index]);
        if (idx >= 0)
        {
            Message.Log("아이템 확인");
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }

    public void MoveSlotPrefabToTutorialPanel(int index)
    {
        foreach (Transform panelTransform in tutorialPanelTransform)
        {
            if (panelTransform.gameObject.activeSelf)
            {
                slotInstances[index].transform.SetParent(panelTransform, false);
                RectTransform rectTransform = slotInstances[index].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = slotPositions[index];
                break;
            }
        }
    }

    void Update()
    {
        // 인벤토리에 특정 아이템이 있을 때만 UpdateSlotAppearance를 호출
        for (int i = 0; i < slotInstances.Length; i++)
        {
            CheckForItems(i);
            if (hasItems[i])
            {
                UpdateSlotAppearance(i);
            }
        }
    }
}
