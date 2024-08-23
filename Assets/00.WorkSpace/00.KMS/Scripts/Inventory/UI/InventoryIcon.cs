
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour, IPointerClickHandler//, IBeginDragHandler, IEndDragHandler, IDragHandler, 
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI text;
    InventorySlot slot;
    public int Index { get { return slot.Index; } }
    public void Init(InventorySlot _slot)
    {
        slot = _slot;
        ResetEntry(true);
    }

    /*public void SetIconCount(int num)
    {
        //텍스트창이 없을 경우 부모에서 자식들 중 텍스트메시프로 컴포넌트를 가져옴
        if (text == null)
            text = transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (text.enabled == false)
            text.enabled = true;
        //개수 표기
        text.text = num.ToString();
    }*/
    public void SetIconSprite(Sprite _icon)
    {
        //아이콘 표기
        if (icon == null)
            icon = GetComponentInChildren<Image>();
        if (icon.enabled == false)
            icon.enabled = true;
        icon.sprite = _icon;
    }
    public void ResetEntry(bool state = false)
    {
        //초기화 및 비활성화
        icon.sprite = null;
        icon.enabled = state;
        /*text.text = "";
        text.enabled = state;*/
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭 시 인벤토리에 본인 슬롯이 클릭되었다고 전달
        if (Manager.Inventory.CombineMode)
            Manager.Inventory.SelectCombineNum = Index;
        else
            Manager.Inventory.SelectSlotNum = Index;
    }
    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Manager.Inventory.CombineMode)
            return;
        //선택 슬롯 초기화 및 슬롯 아웃라인 초기화
        Manager.Inventory.SelectCombineNum = -1;

        //드래그 정보를 담아놓는 클래스의 객체가 없으면 생성
        if (slot.InventoryUI.dragData == null)
            slot.InventoryUI.dragData = new InventoryUI.InventoryDragData();
        //드래그 대상을 자신으로 저장 및 부모 정보를 부모로 저장
        slot.InventoryUI.dragData.entry = this;
        slot.InventoryUI.dragData.parent = (RectTransform)transform.parent;
        //부모를 캔버스로 변경(계층구조 변경)
        transform.SetParent(slot.InventoryUI.Scaler.transform, true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Manager.Inventory.CombineMode)
            return;
        //드래그중일때 위치 변경
        transform.localPosition = eventData.position;
        //transform.localPosition = transform.localPosition + UnscaleEventDelta(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Manager.Inventory.CombineMode)
            return;
        //드롭 시 충돌하는 슬롯 찾기 시도
        slot.InventoryUI.HandledDroppedEntry(eventData.position);
        //아이콘의 위치를 원래 부모 위치로 이동
        transform.SetParent(slot.InventoryUI.dragData.parent, true);
        //맨 첫번째 자식으로 변경
        icon.transform.SetAsFirstSibling();

        RectTransform rect = transform as RectTransform;
        //오프셋을 0으로 설정(꽉차게 설정)
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
    */
    

    //스케일에 따른 대응
    //Vector3 UnscaleEventDelta(Vector3 vec)
    //{
    //    Vector2 referenceResolution = slot.InventoryUI.Scaler.referenceResolution;
    //    Vector2 currentResolution = new(Screen.width, Screen.height);

    //    float widthRatio = currentResolution.x / referenceResolution.x;
    //    float heightRatio = currentResolution.y / referenceResolution.y;
    //    float ratio = Mathf.Lerp(widthRatio, heightRatio, slot.InventoryUI.Scaler.matchWidthOrHeight);

    //    return vec / ratio;
    //}
}
