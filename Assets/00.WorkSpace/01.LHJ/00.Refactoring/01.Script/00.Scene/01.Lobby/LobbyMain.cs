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

    GameObject[] chapters;
    GameObject[] middles;

    Button playButton;

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
        album   .onClick.AddListener(() => ShowPopUpUI("UI/Popup/Lobby/AlbumUI"));

        optionUI    = option.gameObject;
        albumUI     = album.gameObject;

        playButton = GetButton((int)Buttons.SelectEnterBtn);
        playButton.onClick.AddListener(PlayChapter);

        InitChapterUI();

        return true;
    }

    void InitChapterUI()
    {
        middles     = new GameObject[chapterCount];
        chapters    = new GameObject[chapterCount];

        FrontChapterUI frontUI  = Manager.Resource.Load<FrontChapterUI>("UI/Popup/Lobby/Before_Chapter0");
        ChapterInfoUI InfoUI    = Manager.Resource.Load<ChapterInfoUI>("UI/Popup/Lobby/Info_Chapter0");
        ChapterReadyUI readyUI  = Manager.Resource.Load<ChapterReadyUI>("UI/Popup/Lobby/Select_Chapter0");

        GameObject front    = GetObject((int)StageState.ChapterFront);
        GameObject info     = GetObject((int)StageState.ChapterInfo);
        GameObject ready    = GetObject((int)StageState.ChapterReady);

        FrontChapterUI  instantiateFront;
        ChapterInfoUI   instantiateInfo;
        ChapterReadyUI  instantiateReady;

        for (int i = 0; i < chapterCount; i++)
        {
            instantiateFront = GameObject.Instantiate(frontUI, front.transform, true);
            instantiateFront.gameObject.name = $"Before_Chapter0{i}";
            instantiateFront.Init(ChoiceChapterNum, i * 1000, i);

            instantiateInfo = GameObject.Instantiate(InfoUI, info.transform, true);
            instantiateInfo.gameObject.name = $"Info_Chapter0{i}";
            (instantiateInfo.transform as RectTransform).anchoredPosition = Vector2.one;
            instantiateInfo.transform.localScale = Vector3.one;
            instantiateInfo.gameObject.SetActive(false);
            middles[i] = instantiateInfo.gameObject;

            instantiateReady = GameObject.Instantiate(readyUI, ready.transform, true);
            instantiateReady.gameObject.name = $"Select_Chapter0{i}";
            (instantiateReady.transform as RectTransform).anchoredPosition = Vector2.one;
            instantiateReady.transform.localScale = Vector3.one;
            instantiateReady.gameObject.SetActive(false);
            chapters[i] = instantiateReady.gameObject;
        }

        beforeChoose    = GetObject((int)StageState.BeforeChoose);
        middleChoose    = GetObject((int)StageState.MiddleChoose);
        afterChoose     = GetObject((int)StageState.AfterChoose);

        middleChoose.SetActive(false);
        afterChoose .SetActive(false);

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
            Manager.DownLoadBundle.HasBundleCheck(bundleName, (state) =>
            {
                //있다면 게임 실행
                if (state)
                    PlayGame();
                else //없으면 번들 다운로드
                    Manager.DownLoadBundle.DownLoad(null, "basic", bundleName);
            });
        }
    }


    void ChoiceChapterNum(int i)
    {
        chapterNum = i;
        ChangeStageState(SelectState.Middle);
        
        middles[chapterNum].SetActive(true);
    }

    void AfterChoice()
    {
        ChangeStageState(SelectState.Last);

        chapters[chapterNum].SetActive(true);
    }

    void BacktoMain()
    {
        ChangeStageState(SelectState.First);

        middles[chapterNum] .SetActive(false);
        chapters[chapterNum].SetActive(false);

        chapterNum = -1;
    }

    public void BackToMiddle()
    {
        middleChoose.SetActive(true);
        afterChoose .SetActive(false);

        Manager.Text.TextChange();
    }

    void ChangeStageState(SelectState state)
    {
        bool isFirst = SelectState.First == state;
        ShowUIButtons(isFirst);

        beforeChoose.SetActive(isFirst);
        middleChoose.SetActive(SelectState.Middle == state);
        afterChoose .SetActive(SelectState.Last == state);

        if(isFirst == false)
        {
            Manager.Text.TextChange();
        }
    }

    void ShowUIButtons(bool state)
    {
        if(uiState == state)
            return;

        uiState = state;
        optionUI.SetActive(uiState);
        albumUI .SetActive(uiState);
    }

    void ShowPopUpUI(string path)
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>(path));
    }



    void PlayGame()
    {
        playButton.interactable = false;
        Manager.Chapter.chapter = chapterNum;
        Manager.Scene.LoadScene("InGameScene");
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
