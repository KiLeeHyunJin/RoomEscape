using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMain : BaseUI
{
    enum Texts
    {
        TicketText,
        MoneyText,
        CashText,
    }

    enum Buttons
    {
        OptionUI,
        AlbumUI,

        SelectEnterBtn,
        SelectBackBtn,

        SelectReadyBtn,
        SelectReturnBtn,
    }

    enum StageState
    {
        BeforeChoose,
        MiddleChoose,
        AfterChoose,

        ChapterFront,
        ChapterInfo,
        ChapterReady,
    }

    enum SelectState
    {
        First,
        Middle,
        Last,
    }

    GameObject optionUI;
    GameObject albumUI;

    GameObject beforeChoose;
    GameObject middleChoose;
    GameObject afterChoose;
    
    ChapterInfoUI infoUI;
    ChapterReadyUI readyUI;

    GameFlow flowManager;

    Button playButton;
    Action swipeAction;

    [SerializeField] ChapterInfoData[] chapterData; 

    [SerializeField] PopUpUI prologue;
    [SerializeField] PopUpUI explain;

    [SerializeField] int chapterCount;
    [SerializeField] int chapterNum;

    [SerializeField] bool uiState;


    protected override void Start()
    {
        if (CheckTutorial())
            return;

        base.Start();
        BacktoMain();
        flowManager = FindObjectOfType<GameFlow>();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        chapterNum = default;
        uiState = true;

        BindButton(typeof(Buttons));
        BindObject(typeof(StageState));

        GetButton((int)Buttons.SelectReadyBtn)  .onClick.AddListener(AfterChoice);
        GetButton((int)Buttons.SelectReturnBtn) .onClick.AddListener(BacktoMain);
        GetButton((int)Buttons.SelectBackBtn)   .onClick.AddListener(BacktoMain);

        Button option   = GetButton((int)Buttons.OptionUI);
        Button album    = GetButton((int)Buttons.AlbumUI);

        option  .onClick.AddListener(() => ShowPopUpUI("UI/Popup/Lobby/OutGameOptionPanel"));
        album   .onClick.AddListener(() => ShowPopUpUI("UI/Popup/Lobby/AlbumOrGame"));

        optionUI    = option.gameObject;
        albumUI     = album.gameObject;

        playButton = GetButton((int)Buttons.SelectEnterBtn);
        this.playButton.onClick.AddListener(PlayChapter);

        InitChapterUI();

        return true;
    }

    void InitChapterUI()
    {
        ChapterFrontUI load_frontUI  = Manager.Resource.Load<ChapterFrontUI>("UI/Popup/Lobby/Before_Chapter0");
        ChapterInfoUI load_infoUI    = Manager.Resource.Load<ChapterInfoUI>("UI/Popup/Lobby/Info_Chapter0");
        ChapterReadyUI load_readyUI  = Manager.Resource.Load<ChapterReadyUI>("UI/Popup/Lobby/Select_Chapter0");

        GameObject frontParent    = GetObject((int)StageState.ChapterFront);
        GameObject infoParent     = GetObject((int)StageState.ChapterInfo);
        GameObject readyParent    = GetObject((int)StageState.ChapterReady);

        ChapterFrontUI  instantiateFront;
        ChapterInfoData chapterInfoData;
        for (int i = 0; i < chapterCount; i++)
        {
            instantiateFront = GameObject.Instantiate(load_frontUI, frontParent.transform, true);
            instantiateFront.gameObject.name = $"Before_Chapter0{i}";
            chapterInfoData = chapterData[i];
            instantiateFront.Init(ChoiceChapterNum, chapterInfoData.TitleId, i, chapterInfoData.Icon);
        }

        Transform uiTransform;

        infoUI = GameObject.Instantiate(load_infoUI, infoParent.transform, true);
        infoUI.gameObject.name = $"Info_Chapter";
        uiTransform = infoUI.transform;
        (uiTransform as RectTransform).anchoredPosition = Vector2.one;
        uiTransform.localScale = Vector3.one;

        readyUI = GameObject.Instantiate(load_readyUI, readyParent.transform, true);
        readyUI.gameObject.name = $"Select_Chapter";
        uiTransform = readyUI.transform;
        (uiTransform as RectTransform).anchoredPosition = Vector2.one;
        uiTransform.localScale = Vector3.one;

        SwipeController swipeController = frontParent.GetComponent<SwipeController>();
        swipeAction = swipeController.FocusOn;
        swipeController.Init(chapterCount);

        beforeChoose    = GetObject((int)StageState.BeforeChoose);
        middleChoose    = GetObject((int)StageState.MiddleChoose);
        afterChoose     = GetObject((int)StageState.AfterChoose);

        this.middleChoose.SetActive(false);
        this.afterChoose .SetActive(false);

        Manager.Text.TextChange();
    }

    void PlayChapter()
    {
        if (chapterNum == 0)
            PlayGame();
        else
        {
            string bundleName = $"chapter{string.Format("{0:D2}", chapterNum + 1)}";
            //번들을 갖고있는지 확인
            Manager.DownLoadBundle.HasBundle(bundleName, (state) =>
            {
                //있다면 게임 실행
                if (state)
                    PlayGame();
                else //없으면 번들 다운로드
                    Manager.DownLoadBundle.DownLoadPopUp(null, "basic", bundleName);
            });
        }
    }

    void PlayGame()
    {
        this.playButton.interactable = false;
        Manager.Chapter.chapter = chapterNum;
        Manager.Scene.LoadScene("InGameScene");
        //flowManager.ChangeGameScene(GameFlow.GameState.InGame);
    }

    void ChoiceChapterNum(int i)
    {
        chapterNum = i;
        ChangeStageState(SelectState.Middle);

        ChapterInfoData infoData = chapterData[chapterNum];
        infoUI.Init(infoData.TitleId, infoData.DescriptionId);

        Manager.Text.TextChange();
    }

    void AfterChoice()
    {
        ChangeStageState(SelectState.Last);

        ChapterInfoData infoData = chapterData[chapterNum];
        readyUI.Init(infoData.Icon, infoData.TitleId);

        Manager.Text.TextChange();
    }

    void BacktoMain()
    {
        ChangeStageState(SelectState.First);
        this.swipeAction.Invoke();
        chapterNum = -1;
    }

    void ChangeStageState(SelectState state)
    {
        bool isFirst = SelectState.First == state;
        ShowUIButtons(isFirst);

        this.beforeChoose.SetActive(isFirst);
        this.middleChoose.SetActive(SelectState.Middle == state);
        this.afterChoose .SetActive(SelectState.Last == state);
    }

    void ShowUIButtons(bool state)
    {
        if(uiState == state)
            return;

        uiState = state;
        this.optionUI.SetActive(uiState);
        this.albumUI .SetActive(uiState);
    }

    void ShowPopUpUI(string path)
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>(path));
    }



    bool CheckTutorial()
    {
#if UNITY_EDITOR
        return false;
#endif
        UserGameData userGameData = Manager.Data.UserGameData;

        int chapterState = userGameData.chapter;
        if (chapterState == 0)//튜토리얼 실행 메서드 추가
        {
            Manager.UI.ShowPopUpUI(prologue);
            return true;
        }
        if (userGameData.lobbyInfo)// 로비 첫 진입시 챕터슬라이드 설명 팝업
        {
            Message.Log($"lobbyInfo : {userGameData.lobbyInfo}");
            Manager.UI.ShowPopUpUI(explain);
        }
        return false;
    }

    [ContextMenu("ClearPopup")]
    public void ShowTuto()
    {
        Manager.UI.ShowPopUpUI(prologue);
    }
}
