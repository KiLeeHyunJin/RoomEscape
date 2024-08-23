using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongGameMenu : PopUpUI
{
    [Header("ClearCheck")]
    [SerializeField] List<bool> openState;

    [Header("GroupList")]
    [SerializeField] WrongGameGroup[] gameGroup;

    protected override void Awake()
    {
        base.Awake();
        //openState 할당 및 초기화
        StateSetting();
    }
    protected void OnEnable()
    {
        CheckClear();
        Manager.Text.TextChange();
        //Manager.Sound.PlayBGMByIndex(3);
    }
    void ListInit()
    {
        if (openState == null) //리스트가 선언되어있지않다면 카파시티 설정과 선언
            openState = new((int)DataManager.GameDataName.END);
        else //존재한다면 리스트 초기화 후 카파시티 설정
        {
            openState.Clear();
            if (openState.Capacity != (int)DataManager.GameDataName.END)
                openState.Capacity = (int)DataManager.GameDataName.END;
        }
    }
    void StateSetting()
    {
        ListInit();
        for (int i = 0; i < openState.Capacity; i++)
        {
            //nullable변수
            bool? state = Manager.Data[i].isCleared;
            //state 변수가 값을 갖고있고 값이 true면 true 반환 
            //state가 null이거나 state의 값이 false면 false 반환
            openState.Add(state.HasValue && state.Value);
        }
    }
    void CheckClear()
    {
        for (int i = 0; i < openState.Count; i++)
        {
            if (openState[i] == true)
            {
                gameGroup[i].ChapterClearMode();
            }
            else
            {
                gameGroup[i].ChapterUnClearMode();
            }
        }
    }
    public void SummonPopUp(PopUpUI popupGame)
    {
        Manager.UI.ShowPopUpUI(popupGame);
    }
}
