using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Outline outline;

    private InventoryIcon icon;
    private InventoryUI owner;
    private int idx;
    ItemDescription itemDescription;

    [SerializeField] Sprite[] swapImage = new Sprite[2];

    public void ClickDownChange()
    {
        GetComponent<Image>().sprite = swapImage[1]; // 파란색
    }
    public void ClickUpChange()
    {
        GetComponent<Image>().sprite = swapImage[0]; // 초록색
    }

    public InventoryUI InventoryUI
    {
        get
        {
            return owner;
        }
    }
    public Outline Outline
    {
        get
        {
            return outline;
        }
    }

    public int Index
    {
        get
        {
            return idx;
        }
    }
    private void Awake()
    {
        outline = GetComponent<Outline>(); // Outline 컴포넌트 가져오기
        //아웃라인 비활성화
        outline.enabled = false;
        //슬롯 충돌 비활성화
        GetComponent<Image>().raycastTarget = false;
    }
    void Start()
    {
        //아이콘이 없을 시 아이콘 가져오기
        //아이콘 초기화
        icon.Init(this);
        UpdateSlot();
    }
    public void Init(InventoryUI _owner, int _idx)
    {
        icon = icon != null ? icon : GetComponentInChildren<InventoryIcon>();
        owner = _owner;
        idx = _idx;

    }

    public void UpdateSlot()
    {
        //인벤토리 매니저에서 아이콘과 아이템 개수 로드
        (Sprite iconSprite, int count) = Manager.Inventory[idx];
        if (icon == null)
            icon = GetComponentInChildren<InventoryIcon>();
        //스프라이트가 없을 경우 아이템이 존재하지 않은것
        if (iconSprite == null)
        {
            icon.ResetEntry();
        }
        else
        {
            //세팅 업데이트
            icon.SetIconSprite(iconSprite);
            //icon.SetIconCount(count);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ClickUpChange();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClickDownChange();
        Manager.Sound.PlayButtonSound(0);
    }
}
