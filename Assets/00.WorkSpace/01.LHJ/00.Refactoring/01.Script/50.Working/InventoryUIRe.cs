using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIRe : PopUpUI
{
    enum Buttons
    {
        CombineButton,
        DisassembleButton,
        UseButton,
        ContentViewButton
    }

    public class InventoryDragData
    {
        public InventoryIcon entry;
        public Transform parent;
    }

    [SerializeField] Button combine;
    [SerializeField] Button view;
    [SerializeField] Button disassemble;
    [SerializeField] Button use;

    [SerializeField] Sprite onButton;
    [SerializeField] Sprite offButton;


    [Header("Components")]
    // GridLayoutGroup 컴포넌트는 인벤토리 슬롯을 그리드 형식으로 정렬하는 데 사용
    GridLayoutGroup gridLayoutGroup;

    [Header("Prefabs")]
    // slotPrefab은 인벤토리 슬롯의 프리팹
    [SerializeField] InventorySlot slotPrefab;

    [Header("Properties")]
    // numRows는 인벤토리의 행 개수
    [SerializeField] int numRows;
    // numCols는 인벤토리의 열 개수
    [SerializeField] int numCols;

    CanvasScaler scaler;
    InventorySlot[,] grid;
    Vector2 originSize;
    public InventoryDragData dragData;
    public CanvasScaler Scaler
    {
        get
        {
            if (scaler == null)
                scaler = GetComponentInParent<CanvasScaler>();
            return scaler;
        }
    }

    public void ShowAddItem(ScriptableItem item)
    {
        Debug.Log($"{item.name}을 획득하였습니다.");
        Debug.Log($"{item.name}을 획득하였습니다.");
        Debug.Log($"{item.name}을 획득하였습니다.");
    }

    private void OnEnable()
    {
        SlotUpdate();
        SlotOutlineState(-1);
        int selectItemIdxNum = Manager.Inventory.UsingItemIndex;
        if (selectItemIdxNum > -1)
        {
            SlotOutlineState(selectItemIdxNum, true);
            Manager.Inventory.SelectSlotNum = selectItemIdxNum;
        }
    }
    private void OnDisable()
    {
        Manager.Inventory.UIOffState();
    }

    protected override void Awake()
    {
        base.Awake();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        InitSlots();
        //Manager.Inventory.InventoryUI = this;

        RectTransform rect = transform as RectTransform;
        float currentWidth = rect.rect.width;
        float currenHeight = rect.rect.height;
        originSize = new Vector2(currentWidth, currenHeight);
        gameObject.SetActive(false);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButton(typeof(Buttons));

        disassemble = GetButton((int)Buttons.DisassembleButton);
        combine = GetButton((int)Buttons.CombineButton);
        view = GetButton((int)Buttons.ContentViewButton);
        use = GetButton((int)Buttons.UseButton);

        return true;
    }

    protected override void Start()
    {
        base.Start();
        (transform as RectTransform).sizeDelta = originSize;
        //액션 설정
        Manager.Inventory.SelectSlotOutlineUpdateAction(SlotOutlineState);
        Manager.Inventory.ButtonStateUpdateAction(ButtonStateUpdate);

        InitInventoryButton();
    }

    void InitInventoryButton()
    {
        //버튼 액션 함수 연결
        combine.onClick.AddListener
            (Manager.Inventory.CombineItems);
        disassemble.onClick.AddListener
            (Manager.Inventory.DisassembleItem);
        view.onClick.AddListener
            (Manager.Inventory.DetailView);
        use.onClick.AddListener
            (Manager.Inventory.UseItem);

        //버튼 상태에 따른 이미지 설정
        SpriteState spriteState;
        spriteState.selectedSprite = onButton;
        spriteState.pressedSprite = offButton;
        spriteState.disabledSprite = offButton;
        spriteState.highlightedSprite = onButton;

        InitButtonAnim(combine, spriteState);
        InitButtonAnim(view, spriteState);
        InitButtonAnim(disassemble, spriteState);
        InitButtonAnim(use, spriteState);

        //초기 버튼 및 아웃라인 상태 설정
        Manager.Inventory.SelectCombineNum = -1;
        Manager.Text.TextChange();
    }


    void InitButtonAnim(Button button, SpriteState spriteState)
    {
        //버튼 이미지 변경 방식 스프라이트로 설정 및 스프라이트 상태 애니메이션 설정
        if (button.transition != Selectable.Transition.SpriteSwap)
            button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = spriteState;
    }


    void InitSlots()
    {
        // slotSize는 slotPrefab의 RectTransform 크기를 가져옴
        // gridLayoutGroup 컴포넌트를 가져옴
        gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        // numCols는 GridLayoutGroup의 constraintCount로 설정
        numRows = numRows < 1 ? 3 : numRows;
        numCols = gridLayoutGroup.constraintCount;
        // grid 배열을 numRows와 numCols 크기로 초기화
        grid = new InventorySlot[numRows, numCols];
        // InitSlots를 호출하여 슬롯을 초기화
        CreatSlots();
    }

    // InitSlots 메서드는 시작할 때 빈 인벤토리 슬롯을 생성
    void CreatSlots()
    {
        int col = grid.GetLength(1);
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                // slotPrefab을 인스턴스화하고 InventorySlot 컴포넌트를 가져옴
                InventorySlot slot = Instantiate(slotPrefab);
                //부모 및 인덱스 설정
                //slot.Init(this, (r * col) + c);
                // 슬롯을 현재 오브젝트의 자식으로 설정
                slot.transform.SetParent(gridLayoutGroup.transform);
                (slot.transform as RectTransform).localScale = Vector2.one;
                // grid 배열에 슬롯을 저장
                grid[r, c] = slot;
            }
        }
    }

    public void SlotUpdate(int index = -1)
    {
        if (grid == null)
            return;

        //슬롯 업데이트 여부 확인
        if (index < 0)
        {
            //모든 슬롯 업데이트
            foreach (InventorySlot itemSlot in grid)
            {
                if (itemSlot == null)
                    continue;
                itemSlot?.UpdateSlot();
            }
            return;
        }
        //인덱스를 2차원 배열의 인덱스로 변환
        (int x, int y) = GetArrayNum(index);
        //해당 슬롯 업데이트
        grid[x, y].UpdateSlot();
    }

    public bool HandledDroppedEntry(Vector3 position)
    {
        //드롭 시 충돌한 슬롯을 찾아서 반환
        foreach (InventorySlot itemSlot in grid)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(itemSlot.transform as RectTransform, position))
            {
                //본인 슬롯인지 확인
                if (itemSlot.Index != dragData.entry.Index)
                {
                    //아닐 시 아이템 스왑
                    Manager.Inventory.SwapItemSlot(itemSlot.Index, dragData.entry.Index);
                    return true;
                }
            }
        }
        return false;
    }


    void SlotOutlineState(int idx, bool state = false)
    {
        if (idx < 0)
        {
            //모든 아이템 슬롯 아웃라인 비활성화
            foreach (InventorySlot itemSlot in grid)
                itemSlot.Outline.enabled = false;
            return;
        }

        //2차원 배열 인덱스로 변환
        (int x, int y) = GetArrayNum(idx);
        //해당 슬롯의 아웃라인 상태를 매개변수 상태로 변환
        grid[x, y].Outline.enabled = state;
    }

    void ButtonStateUpdate()
    {
        //버튼 활성화 상태 업데이트
        (bool combine, bool disassemble, bool view, bool use) = Manager.Inventory.CheckSelectState();
        SetActiveButton(InventoryButtonEnum.Combine, combine);
        SetActiveButton(InventoryButtonEnum.Disassemble, disassemble);
        SetActiveButton(InventoryButtonEnum.View, view);
        SetActiveButton(InventoryButtonEnum.Use, use);
    }

    (int x, int y) GetArrayNum(int index)
    {
        //1차원 배열 인덱스를 2차원 배열 인덱스로 변환
        int col = grid.GetLength(1);
        return (index / col, index % col);
    }

    public void SetActiveButtonOnlyOne(InventoryButtonEnum button, bool state = false)
    {
        foreach (InventoryButtonEnum buttonType in Enum.GetValues(typeof(InventoryButtonEnum)))
        {
            bool activeState = buttonType == button ? state : !state;
            SetActiveButton(buttonType, activeState);
        }
    }

    void SetActiveButton(InventoryButtonEnum buttonType, bool state = false)
    {
        //인벤토리 버튼 상태 변경
        Button changeStateButton = buttonType switch
        {
            InventoryButtonEnum.Combine => combine,
            InventoryButtonEnum.Disassemble => disassemble,
            InventoryButtonEnum.View => view,
            InventoryButtonEnum.Use => use,
            _ => null,
        };
        if (changeStateButton != null)
            changeStateButton.interactable = state;
    }
    //인벤토리 버튼 종류 열거형
    public enum InventoryButtonEnum
    {
        Combine,
        Disassemble,
        View,
        Use
    }

    public InventorySlot GetSlotByIndex(int index) // 테스트
    {
        (int x, int y) = GetArrayNum(index);
        return grid[x, y];
    }
}
