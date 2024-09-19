using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryData;


// InventoryManager 클래스는 Singleton 패턴을 사용하여 인벤토리를 관리
public class InventoryManager : Singleton<InventoryManager>
{

    [SerializeField] InventoryData inventory;
    [SerializeField] RecipeList recipeList; // 조합 레시피 리스트

    [SerializeField] List<int> selectIdxList;
    [SerializeField] int selectSlotNum;
    [SerializeField] int usingItemIndex;

    List<string> checkItemNameList;

    Action<int, bool> SlotOutlineUpdate;
    Action ButtonStateUpdate;

    InventoryUI inventoryUI;

    public string SaveInventory()
    {
        return inventory.SaveJosonData() ;
    }

    public void LoadInventory(string json, int chapterNum)
    {
        if (string.IsNullOrEmpty(json))
            return;
        string chapterName = $"chapter0{chapterNum}";
        ItemDataSave data = JsonUtility.FromJson<ItemDataSave>(json);
        if (data == null)
            return;

        inventory.Init();
        for (int i = 0; i < data.saveArray.Length; i++)
        {
            if (data.saveArray[i].count <= 0 || string.IsNullOrEmpty(data.saveArray[i].itemName))
                continue;

            SettingItem(chapterName, i, data.saveArray[i].itemName, data.saveArray[i].count);
        }
    }

    void SettingItem(string chapterName, int idx, string itemName, int count)
    {
        Manager.Resource.GetAsset(chapterName, itemName, ResourceType.Scriptable, (obj) =>
        {
            inventory.SetItem(idx, (ScriptableItem)obj, count);
        }, false);
    }

    public int Count { get { return inventory.Count; } }

    bool isCombineMode;
    public int UsingItemIndex
    {
        get { return usingItemIndex; }
    }
    public bool CombineMode
    {
        get { return isCombineMode; }
    }
    public int SelectSlotNum
    {
        set { ChangeSelectSlotNum(value); }
    }
    public int SelectCombineNum
    {
        set { AddSelectIndex(value); }
    }
    //해당 슬롯의 아이콘 및 아이템 개수 반환
    public (Sprite icon, int count) this[int index]
    {
        get { return inventory[index]; }
    }

    //인벤토리 UI와 연결
    public InventoryUI InventoryUI
    {
        set
        {
            if (value != null)
                inventoryUI = value;
        }
    }

    public event Action OnCombineSuccess;

    public void UIOffState()
    {
        if (CombineMode)
        {
            isCombineMode = false;
            //선택 내역 초기화
            AddSelectIndex();
        }
    }
    public void SelectSlotOutlineUpdateAction(Action<int, bool> action)
    {
        SlotOutlineUpdate = action;
    }
    public void ButtonStateUpdateAction(Action action)
    {
        ButtonStateUpdate = action;
    }

    protected override void Awake()
    {
        base.Awake();
        inventory = new(Define.InventoryCount);
        selectIdxList = new();
        checkItemNameList = new();
        isCombineMode = default;
        usingItemIndex = -1;
        selectSlotNum = -1;
    }

    public void Init()
    {
        inventory.Init();
        if (inventoryUI != null)
            inventoryUI.SlotUpdate();
    }

    //선택 슬롯 번호 변경
    void ChangeSelectSlotNum(int idx = -1)
    {
        if (selectSlotNum > -1)
        {
            //디폴트 넘버가 아니라면 해당 슬롯 아웃라인 해제
            SlotOutlineUpdate?.Invoke(selectSlotNum, false);
            //똑같은 슬롯 번호면 -1 아니라면 새로운 값 할당
            selectSlotNum = selectSlotNum == idx ? -1 : idx;
        }
        else
        {
            selectSlotNum = idx;
        }
        //할당된 번호가 디폴트인지 확인
        bool state = selectSlotNum != -1;
        //디폴트일경우 해당 슬롯의 아웃라인 해제
        SlotOutlineUpdate?.Invoke(selectSlotNum, state);
        ButtonStateUpdate?.Invoke();

    }
    void ObtainMultiItem(ScriptableItem item, int count = 1)
    {
        //아이템이 추가 시도 후 상태 반환
        if (inventory.AddItem(item, count))
        {
            //추가 되었으면 해당 슬롯 업데이트
            if (inventoryUI != null)
                inventoryUI.SlotUpdate();
        }
        else
        {
            //슬롯이 꽉찼을 경우
            //Debug.Log("아이템 지급에 실패하였습니다.");
        }
    }
    public void ObtainItem(ScriptableItem item, bool showPopup = false) // 인벤토리에 아이템 획득을 가져오는 함수
    {
        Debug.Log("Obtained: " + item.name);
        //아이템이 추가 시도 후 상태 반환
        if (inventory.AddItem(item))
        {
            //추가 되었으면 해당 슬롯 업데이트
            if (inventoryUI != null)
                inventoryUI.SlotUpdate();
        }
        else
        {
            //슬롯이 꽉찼을 경우
            //Debug.Log("아이템 지급에 실패하였습니다.");
        }
        if (showPopup)
        {
            if (inventoryUI != null)
                inventoryUI.ShowAddItem(item);
        }
    }


    public bool FillCheck(int idx)
    {
        //해당 슬롯의 아이템 존재여부 반환
        return inventory.FillCheck(idx);
    }

    public void SwapItemSlot(int item1, int item2)
    {
        //아이템 교체
        inventory.SwapItem(item1, item2);
        //슬롯 상태 업데이트
        if (inventoryUI == null)
            return;

        inventoryUI.SlotUpdate(item1);
        inventoryUI.SlotUpdate(item2);
    }

    void AddSelectIndex(int index = -1)
    {
        //인위적인 인덱스 번호인지 확인
        if (index < 0)
        {
            selectSlotNum = -1;
            selectIdxList.Clear();
            SlotOutlineUpdate?.Invoke(-1, false);
            ButtonStateUpdate?.Invoke();
            return;
        }
        //해당 번호가 선택되어있었는지 확인
        bool state = selectIdxList.Contains(index);
        //존재하면 번호 삭제 없으면 번호 추가
        if (state)
            selectIdxList.Remove(index);
        else
            selectIdxList.Add(index);
        //해당 번호의 아웃라인 설정 상태를 반환
        SlotOutlineUpdate?.Invoke(index, !state);
    }


    public (bool combine, bool disassemble, bool view, bool use) CheckSelectState()
    {
        //버튼 활성화 여부 
        if (selectSlotNum == -1)
            return (false, false, false, false);
        string selectItemName = inventory.GetItemName(selectSlotNum);
        //제작법이 있는지 확인
        bool combineState = FindSourceRecipe(selectItemName);
        //분해법이 있는지 확인
        bool disassembleState = ReverCombineCheck(selectItemName) != null;
        //상세보기가 있는지 확인
        bool viewState = inventory.GetPopupUI(selectSlotNum) != null;
        //checkItemNameList.Clear();
        return (combineState, disassembleState, viewState, true);
    }

    Coroutine errorCo;
    public ScriptableItem GetCurrentUsingItem() // 현재 사용 중인 아이템을 불러오는 함수
    {
        //에러 코드 실행
        this.ReStartCoroutine(ErrorRoutine(), ref errorCo);
        return null;
    }

    IEnumerator ErrorRoutine()
    {
        while (true)
        {
            //Debug.LogError("[사용하지 않는 함수입니다.] : InvnetoryManager.cs _ GetCurrentUsingItem()");
            yield return new WaitForSeconds(0.2f);
        }
    }

    public string GetCurrentUsingItemName() // 현재 사용 중인 아이템을 불러오는 함수
    {
        if (UsingItemIndex < 0)
            return null;
        //장착한 아이템의 이름을 반환
        return inventory.GetItemName(UsingItemIndex);
    }

    bool IsSameList(List<ScriptableItem> recipeList, List<string> selectList) // 두 리스트가 동일한 지 확인하는 함수
    {
        //순회하면서 이름이 같은지 비교
        for (int i = 0; i < recipeList.Count; i++)
        {
            if (string.Equals(recipeList[i].name, selectList[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    bool FindSourceRecipe(string itemName)
    {
        //레시피 순회
        foreach (Recipe recipe in recipeList.recipes)
        {
            //레시피 재료 순회
            foreach (ScriptableItem recipeSourceItem in recipe.source)
            {
                //레시피 재료를 사용하는 아이템이 있을시 참 반환
                if (string.Equals(recipeSourceItem.name, itemName))
                    return true;
            }
        }
        return false;
    }

    Recipe FindRecipe(List<string> selectList) // 선택된 아이템들로 조합할 수 있는 레시피를 찾는 함수
    {
        //아이템 이름 리스트를 정렬
        selectList.Sort();
        foreach (Recipe recipe in recipeList.recipes)
        {
            //레시피 재료를 가져와서 정렬
            List<ScriptableItem> left = new(recipe.source);
            left.Sort();
            //개수 비교
            if (left.Count != selectList.Count)
                continue;
            //양측 이름을 비교
            if (IsSameList(left, selectList) == false)
                continue;

            return recipe;
        }
        return null;
    }
    public bool RemoveItem(string itemName, int count = 1)
    {
        inventory.RemoveItmeName(itemName, count);
        return true;
    }

    public void OnSuccessUseItem() // 아이템 사용 성공했을 시 호출되는 함수
    {
        //장착? 한 아이템을 삭제
        inventory.RemoveItem(UsingItemIndex);
        //슬롯 업데이트
        if (inventoryUI != null)
            inventoryUI.SlotUpdate(UsingItemIndex);
        usingItemIndex = -1;
    }

    public void UseItem() // 아이템을 사용하는 함수
    {
        if (selectSlotNum < 0)
        {
            //Debug.LogWarning("사용할 아이템이 선택되지 않음");
            usingItemIndex = -1;
            return;
        }
        //마지막 선택한 아이템을의 슬롯번호를 장착? 형식으로 설정
        usingItemIndex = selectSlotNum;
        //Debug.Log($"사용하려고 선택한 아이템의 index {UsingItemIndex}");
        ChangeSelectSlotNum();
        closeAction?.Invoke();


        if (!Manager.Sound.itemUseSoundMapping.TryGetValue(inventory.GetItem(UsingItemIndex).name, out int soundIndex))
        {
            soundIndex = 5; // 기본값 사운드 인덱스
        }
        Manager.Sound.PlayItemSound(soundIndex);
        Debug.Log(inventory.GetItem(UsingItemIndex).name);
    }

    Action closeAction;

    public void CloseAction(Action call)
    {
        closeAction = call;
    }

    //아이템 소유여부 확인
    public bool FindItem(string itemName)
    {
        return inventory.FindIndex(itemName) >= 0;
    }
    public bool FindItem(params string[] itemName)
    {
        return inventory.FindIndex(itemName) >= 0;
    }
    public int FindItemIndex(string itemName)
    {
        return inventory.FindIndex(itemName);
    }
    public int FindItemIndex(params string[] itemName)
    {
        return inventory.FindIndex(itemName);
    }


    //아이템 소유여부 확인
    public bool FindItem(ScriptableItem item)
    {
        return inventory.FindIndex(item) >= 0;

    }
    public void CombineItems() // 아이템을 조합하는 함수
    {
        if (isCombineMode == false)
        {
            CombineModeOn();
            return;
        }
        isCombineMode = false;

        //선택한 아이템들을 이름으로 저장
        foreach (int idx in selectIdxList)
        {
            checkItemNameList.Add(inventory.GetItemName(idx));
        }
        //해당 아이템 이름들과 레시피 비교 후 레시피 반환
        Recipe recipe = FindRecipe(checkItemNameList);
        checkItemNameList.Clear();
        int[] selectArray = selectIdxList.ToArray();
        //선택 내역 초기화
        AddSelectIndex();
        if (recipe == null)
        {
            //Debug.Log("선택한 아이템들로 조합할 수 있는 조합법이 없음");
            return;
        }
        //선택한 아이템들 삭제
        foreach (int idx in selectArray)
        {
            if (inventory.RemoveItem(idx) == false)
            {
                //Debug.Log("조합 과정에서 문제 발생하였습니다. : 아이템 개수가 맞지 않거나 아이템이 존재하지 않은 슬롯에서 아이템 삭제를 시도하였습니다.");
            }
        }

        OnCombineSuccess?.Invoke();
        // 조합 성공시 결과 아이템 추가
        ObtainItem(recipe.result);

        if (!Manager.Sound.itemCombineSoundMapping.TryGetValue(recipe.result.name, out int soundIndex))
        {
            soundIndex = 5; // 기본값 사운드 인덱스
        }
        Manager.Sound.PlayItemSound(soundIndex);
        Debug.Log(recipe.result.name);
    }

    void CombineModeOn()
    {
        usingItemIndex = -1;
        //조합 모드 활성화
        isCombineMode = true;
        //사용하기 전 리스트 초기화
        selectIdxList.Clear();
        selectIdxList.Add(selectSlotNum);
        //조합 버튼만 활성화
        if (inventoryUI != null)
            inventoryUI.SetActiveButtonOnlyOne(InventoryUI.InventoryButtonEnum.Combine, true);
    }

    Recipe ReverCombineCheck(string selectList) // 선택된 아이템들로 조합할 수 있는 레시피를 찾는 함수
    {
        //레시피 순회
        foreach (Recipe recipe in recipeList.recipes)
        {
            //조합 결과 아이템의 이름이랑 비교
            if (string.Equals(recipe.result.name, selectList))
            {
                //같으면 해당 레시피 반환
                return recipe;
            }
        }
        return null;
    }
    public void DisassembleItem() // 아이템을 분해하는 함수
    {
        if (selectSlotNum < 0)
        {
            //Debug.LogWarning("분해할 아이템이 선택되지 않음");
            return;
        }
        //해당 슬롯의 아이템 이름을 저장
        string dissableItemName = inventory.GetItemName(selectSlotNum);
        //해당 아이템의 조합법 검색
        Recipe reverItem = ReverCombineCheck(dissableItemName);
        //선택 리스트 초기화 및 버튼 상태 재검사
        if (reverItem == null)
        {
            ChangeSelectSlotNum();
            return;
        }

        //분해할 아이템 삭제
        inventory.RemoveItem(selectSlotNum);
        //분해한 아이템의 조합 재료를 추가
        foreach (ScriptableItem item in reverItem.source)
            ObtainItem(item);
        ChangeSelectSlotNum();
    }

    public void DetailView() // 아이템 상세보기를 위한 함수
    {
        if (selectSlotNum < 0)
            return;
        
        //마지막 선택 아이템이 팝업 프리펩이 있다면 출력
        PopUpUI itemView = inventory.GetPopupUI(selectSlotNum);
        if (itemView != null)
            Manager.UI.ShowPopUpUI(itemView);
        else
        {
            // PopupUI가 없을 경우 toolTipObj를 사용하여 상세 정보를 보여줌
            ScriptableItem selectedItem = inventory.GetItem(selectSlotNum);
            ItemDescription itemDescription = FindObjectOfType<ItemDescription>();
            if (itemDescription != null)
            {
                InventorySlot selectedSlot = inventoryUI.GetComponent<InventorySlot>();
                itemDescription.OpenUI(selectedItem, selectedSlot);
            }
        }
        ChangeSelectSlotNum();
    }


    public void HighlightItemByName(string itemName)
    {
        //Message.Log("하이라이트 아이템: " + itemName);
        // 모든 슬롯의 테두리를 초기화
        SlotOutlineUpdate?.Invoke(-1, false);

        for (int i = 0; i < inventory.Count; i++)
        {
            //Debug.Log("슬롯 아이템 확인: " + inventory.GetItemName(i));
            if (string.Equals(inventory.GetItemName(i), itemName))
            {
                //Message.Log("아이템 일치: " + itemName);
                SlotOutlineUpdate?.Invoke(i, true);
            }
        }
    }

    private void OnDestroy()
    {
        ChapterSaveLoad saveLoad = FindObjectOfType<ChapterSaveLoad>();
        if (saveLoad == null)
            return;
        saveLoad.SaveCurrentChapter();
    }

    public ScriptableItem GetItem(int index)
    {
        return inventory.GetItem(index);
    }
}
