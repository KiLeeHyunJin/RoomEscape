using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이전 인벤토리 버튼 
/// </summary>
public class BeforeInventoryButton : MonoBehaviour
{
    public Button inventoryButton;
    public GameObject inventoryPanel;
    bool activeInventory;

    private void Start()
    {
        //버튼 활성화 여부는 False로 설정
        activeInventory = false;

        if(inventoryPanel != null) //인벤토리 패널이 연결중일 경우 
            inventoryPanel.SetActive(activeInventory); // 시작할 때 인벤토리 패널을 비활성화로 설정
        
        if (inventoryButton == null) //인벤토리 버튼이 연결되어있지 않을 경우
            inventoryButton = GetComponentInChildren<Button>();//하위 객체에서 버튼을 가져온다.
        
        if (inventoryButton != null) //버튼이 연결되어있다면
        {
            //인벤토리 버튼에 함수를 연결한다.(액션)
            inventoryButton.onClick.AddListener(ToggleInventory);
            Debug.Log("인벤토리 버튼 입력");
        }
        //인벤토리 매니저의 인벤토리 닫기 액션에 토글 인벤토리 클릭 함수를 연결한다.
        Manager.Inventory.CloseAction(ToggleInventory);
    }

    /// <summary>
    /// 인밴토리 OnOff메소드다.
    /// </summary>
    void ToggleInventory()
    {
        activeInventory = !activeInventory; // 인벤토리 버튼 클릭 시 인벤토리 활성화 상태를 토글
        inventoryPanel.SetActive(activeInventory);
    }

    public void CloseInventory()
    {
        if (activeInventory)
        {
            activeInventory = false;
            inventoryPanel.SetActive(false);
        }
    }
}
