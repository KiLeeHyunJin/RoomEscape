using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HintWindow : MonoBehaviour
{
    int questionIdx;
    public HintPopup HintUsePopUpUI;
    [SerializeField] HintSlotEntry HintSlotPrefab;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] PopUpUI enlargementViewer;
    [SerializeField] NotEnoughPopup notEnugh;
    [SerializeField] Button resetBtn;
    List<HintSlotEntry> slots = new();

    void AddHint(UnityEngine.Object obj)
    {

    }

    void Init()
    {
        //Manager.Chapter.LoadHintData();
        for (int i = 0; i < Manager.Chapter.HintData.GetQuestionCount(); i++)
        {
            if (Manager.Chapter.HintData.GetQuestionState(i) == false)
            {
                questionIdx = i;
                return;
            }
        }
        questionIdx = Manager.Chapter.HintData.GetQuestionCount() - 1;
        //this.gameObject.FontInit(Define.Font.MLight);
    }

    private void Start()
    {
        if (Manager.Chapter.chapter == 0)
            return;

        Init();
        CreateSlot();
        if (gridLayoutGroup == null)
            gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        if (resetBtn != null)
            resetBtn.onClick.AddListener(()=> 
            { 
                Manager.Chapter.ResetHintAll();
                UpdateSlotDisplay();
            });
    }

    public void CurrentHintCheck()
    {
        for (int i = 0; i < Manager.Chapter.HintData.GetQuestionCount(); i++)
        {
            if (Manager.Chapter.HintData.GetQuestionState(i) == false)
            {
                questionIdx = i;
                return;
            }
        }
        questionIdx = Manager.Chapter.HintData.GetQuestionCount();
    }

    private void OnEnable()
    {
        Init();
        if (slots.Count == 0)
            return;
        UpdateSlotDisplay();
    }

    public void CreateSlot()
    {
        slots.Clear();
        for (int i = 0; i < Manager.Chapter.HintData.GetHintCount(questionIdx); i++)
        {
            var obj = Instantiate(HintSlotPrefab);
            obj.transform.SetParent(gridLayoutGroup.transform);
            //this.gameObject.FontInit(Define.Font.MLight);
            (obj.transform as RectTransform).localScale = Vector2.one;

            int currentIdx = i;
            AddEvent(obj.gameObject, EventTriggerType.PointerClick, delegate { OnClickpointer(currentIdx); });
            slots.Add(obj);
        }
        UpdateSlotDisplay();
    }

    void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetOrAddComponent<EventTrigger>();
        if (trigger != null)
        {
            var eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

    }
    public void OnClickpointer(int idx) // 마지막 힌트는 앞 힌트 다 열려야 열수 있게
    {
        //HintObject hintObj = slotsOnInterface[obj].hintObject;
        int saveIdx = idx;
        int hintLength = Manager.Chapter.HintData.GetHintCount(questionIdx);

        Message.Log($"Hint Count {Manager.Data.UserGameData.hint}");

        if (saveIdx == hintLength - 1 && AllOpened())
        {
            OpenCheck(saveIdx);
        }
        else if (saveIdx < hintLength - 1)
        {
            OpenCheck(saveIdx);
        }
    }

    void OpenCheck(int hintId)
    {
        if (Manager.Chapter.HintData.GetOpenHintState(questionIdx, hintId))
        {
            Manager.Chapter.HintPopUpSprite = Manager.Chapter.HintData.GetHint(questionIdx, hintId).sprite;
            Manager.UI.ShowPopUpUI(enlargementViewer);
        }
        else
        {

            if (Manager.Chapter.CheckLimitHintState) 
            {
                NotEnoughPopup notEnoughPopup = Manager.UI.ShowPopUpUI<NotEnoughPopup>(notEnugh);
                notEnoughPopup.InitState(NotEnoughPopup.InitType.Reset);
            }
            else
            {
                if (Manager.Data.UserGameData.hint <= 0)
                {
                    NotEnoughPopup notEnoughPopup = Manager.UI.ShowPopUpUI<NotEnoughPopup>(notEnugh);
                    notEnoughPopup.InitState(NotEnoughPopup.InitType.NotEnough);
                }
                else
                {
                    HintPopup hintPopup = Manager.UI.ShowPopUpUI(HintUsePopUpUI);
                    hintPopup.SetHintAction(() =>
                    {
                        Manager.Chapter.HintData.SetOpenHintState(questionIdx, hintId);
                        UpdateSlotDisplay();
                    });
                }
            }
        }
    }

    private bool AllOpened()
    {
        for (int i = 0; i < Manager.Chapter.HintData.GetHintCount(questionIdx) - 1; i++)
        {
            if (Manager.Chapter.HintData.GetOpenHintState(questionIdx,i) == false)
                return false;
        }
        return true;
    }

    public bool CheckQuestion()
    {
        for (int i = 0; i < Manager.Chapter.HintData.GetQuestionCount(); i++)
        {
            if (Manager.Chapter.HintData.GetQuestionState(i) == false)
            {
                return false;
            }
        }
        return true;
    }

    void UpdateSlotDisplay()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            HintSlotEntry slot = slots[i];

            if (Manager.Chapter.HintData.GetOpenHintState(questionIdx, i))
            {
                slot.Icon = Manager.Chapter.HintData.GetHint(questionIdx, i).sprite;
                slot.Information = null;
                slot.IconColor = new Color(1, 1, 1, 1);
            }
            else
            {
                slot.Icon = null;
                slot.Information = Manager.Chapter.HintData.GetHint(questionIdx, i).information;
                slot.IconColor = new Color(1, 1, 1, 0);
            }

            //if (slot.RayCastTarget)
            //    slot.RayCastTarget = false;
        }
    }
}

public static class ExtentionMethods
{
    // 슬롯 스프라이트 업데이트 하는 부분
   
}