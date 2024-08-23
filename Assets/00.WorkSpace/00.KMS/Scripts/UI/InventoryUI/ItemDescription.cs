using TMPro;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private GameObject toolTipObj; // 툴팁
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _itemName; // 아이템 이름
    [SerializeField] private TextMeshProUGUI _itemDescription; // 아이템 설명

    //[Header("Image")]
    //[SerializeField] private Image itemImage;

    private InventorySlot _slot;
    private RectTransform rectTransform; //UI 트랜스폼

    //private ItemData itemData;

    //private StringBuilder stringBuilder; //스트링 빌더

    private void Start()
    {
        //textArea = toolTipObj.GetComponentInChildren<TextMeshProUGUI>();
        //itemData = FindObjectOfType<ItemData>();

        if (_canvas != null)
            rectTransform = _canvas.GetComponent<RectTransform>();

        //stringBuilder = new StringBuilder();

        toolTipObj.SetActive(false);
        Manager.Text.TextChange();
    }

    public void LateUpdate()
    {
        if (toolTipObj.activeInHierarchy)
        {
            CalcPosition();
        }
    }

    /// InventorySlot에서 호출되며 아이템 정보를 보여줌

    public void OpenUI(ScriptableItem item, InventorySlot Slot)
    {
        //_itemName.SetText(item.itemID);
        //_itemName.name = item.itemID;
        if (Manager.Text._Iskr)
        {
            _itemName.SetText(item.itemID);
        }
        else
        {
            _itemName.SetText(item.itemID);
        }

        if (Manager.Text._Iskr)
        {
            _itemDescription.SetText(item.description);
        }
        else
        {
            _itemDescription.SetText(item.description);
        }

        _slot = Slot;
        Manager.Text.TextChange();
        /*stringBuilder.Clear();

        //이름 가져오기
        stringBuilder.Append("<b>");
        stringBuilder.AppendLine(item.itemName);
        stringBuilder.Append("</b>");

        //설명 가져오기
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(item.Description);
        stringBuilder.AppendLine();*/

        /*if (itemImage != null && item.itemIcon != null)
        {
            itemImage.sprite = item.itemIcon;
            itemImage.enabled = true;
        }
        else if (itemImage == null)
        {
            itemImage.enabled = false; // 아이템 이미지가 없으면 비활성화
        }*/

        //텍스트 replace
        //textArea.SetText(stringBuilder.ToString());
        toolTipObj.SetActive(true);

        Invoke("CloseUI", 1f);
    }

    public void CloseUI()
    {
        toolTipObj.SetActive(false);
    }

    public void CalcPosition()
    {
        if (_slot == null) return;

        RectTransform rt = toolTipObj.transform as RectTransform;
        RectTransform slotRectTransform = _slot.GetComponent<RectTransform>();

        Vector2 slotPosition = slotRectTransform.anchoredPosition;
        Vector2 localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, new Vector2(slotPosition.x, slotPosition.y), _canvas.worldCamera, out localPosition);

        /*localPosition.x += slotRectTransform.sizeDelta.x;
        localPosition.y -= rt.rect.height;*/

        rt.anchoredPosition = localPosition;

        /*Vector2 localPosition; // 변환된 canvas내 현재 좌표
        Vector2 mousePosition = new Vector2(); //마우스의 현재 위치
        //Vector2 touchPosition = Input.GetTouch(0).position; // 터치로 변경

        // 텍스트 라벨 가져옴
        RectTransform rt = toolTipObj.transform as RectTransform;

        // 마우스 좌표를 canvas내에서의 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, _canvas.worldCamera, out localPosition);

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, _canvas.worldCamera, out localPosition); // 터치일 때

        //localPosition.y += rt.rect.height * 0.5f;

        if (mousePosition.x > Screen.width * 0.75f) { localPosition.x -= rt.sizeDelta.x * 0.1f; }

        *//*if (touchPosition.x > Screen.width * 0.75f)
        {
            localPosition.x -= rt.sizeDelta.x * 0.1f; // 터치일 때
        }*//*

        // 위치 변경
        rt.anchoredPosition = localPosition;*/
    }
}
