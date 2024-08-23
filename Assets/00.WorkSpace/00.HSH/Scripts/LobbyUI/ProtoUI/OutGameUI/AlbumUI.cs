using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumUI : PopUpUI
{
    List<bool> openState;
    [SerializeField] GameObject[] viewerBlocker;
    [SerializeField] PopUpUI albumViewer;
    [SerializeField] Image[] chapterPictures;
    [SerializeField] GameObject lastBlocker;
    [SerializeField] Image lastViewer;

    [SerializeField] PopUpUI finale;
    protected override void Awake()
    {
        base.Awake();
        //openState 할당 및 초기화
        StateSetting();
    }

    private void OnDestroy()
    {
        Manager.Sound.PlayBGMByIndex(2);
    }

    protected override void Start()
    {
        base.Start();
        CheckClear();
        Manager.Text.TextChange();
        Manager.Sound.PlayBGMByIndex(3);
    }

    //클리어되었는지 확인 후 변수에 값 대입 (튜토리얼은 제외 1스테이지는 0번째 위치, 2스테이지는 1번째 위치에 저장 이후 )
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
    void CheckClear()
    {
        for (int i = 0; i < openState.Count; i++)
        {
            if (openState[i] == true)
            {
                viewerBlocker[i].SetActive(false);
                chapterPictures[i].color = Color.white;
            }
            else
            {
                chapterPictures[i].color = Color.gray;
                lastViewer.color = Color.gray;
            }
        }
        for (int i = 0; i < openState.Count; i++)
        {
            if (openState[i] != true)
            {
                lastBlocker.SetActive(true);
                return;
            }
            else
            {
                lastBlocker.SetActive(false);
                lastViewer.color = Color.white;
            }
        }

    }
    public void OpenViewer(int i)
    {
        Manager.Chapter.chapter = i;
        Manager.UI.ShowPopUpUI(albumViewer);
    }
    public void OpenFinale()
    {
        Manager.Chapter.chapter = 6;
        Manager.UI.ShowPopUpUI(finale);
    }
}