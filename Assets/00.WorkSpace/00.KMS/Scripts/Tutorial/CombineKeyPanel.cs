using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombineKeyPanel : MonoBehaviour, IPointerClickHandler
{
    public GameObject inventoryPanel;
    public GameObject showGetKeyPanel;
    public Button inventoryButton;
    private bool hasObtainedKeyParts = false;
    private bool isActivePanel = false;

    TutorialInventoryManager tutorialInventoryManager;

    private void Start()
    {
        Manager.Text.TextChange();
        if (hasObtainedKeyParts)
        {
            showGetKeyPanel.SetActive(true);
        }
        Message.Log("스타트 확인");
        showGetKeyPanel.SetActive(false);
        tutorialInventoryManager = FindObjectOfType<TutorialInventoryManager>(); // 아이템 클릭 시 다음 패널로 이동
        inventoryButton.onClick.AddListener(OnInventoryClick);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (showGetKeyPanel != null && inventoryPanel.activeSelf && hasObtainedKeyParts)
        {
            Message.Log("화면 클릭");
            showGetKeyPanel.SetActive(false);
            tutorialInventoryManager.NextPanel();
        }
    }

    void CheckForKeyParts() // 열쇠 조각 확인
    {
        Message.Log("열쇠 조각 확인");
        int keyPartCount = GetKeyPartCount();
        if (keyPartCount >= 1)
        {
            hasObtainedKeyParts = true;
        }
    }

    int GetKeyPartCount()
    {
        int count = 0;
        int idx = InventoryManager.Instance.FindItemIndex("KeyPart1");
        if (idx >= 0)
        {
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }

    public void OnInventoryClick()
    {
        CheckForKeyParts(); // 열쇠 있는지 확인
        if (hasObtainedKeyParts && !isActivePanel)
        {
            ShowHintPopup();
            HighlightItemByNames();
        }
    }

    void ShowHintPopup()
    {
        showGetKeyPanel.SetActive(true);
        isActivePanel = true;
    }

    void HighlightItemByNames()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        string[] keyPartNames = {"KeyPart1", "KeyPart2" };

        foreach (string keyPartName in keyPartNames)
        {
            inventoryManager.HighlightItemByName(keyPartName);
        }
    }
}
