using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        Hint,
        Option,
        Start,
        Reset,
        Next,
        Cancle,
        Chapter0Button,
        Chapter01Button,
        Chapter02Button,
    }

    enum StageState
    {
        BeforeChoose,
        MiddleChoose,
        AfterChoose,

        Chapter0Story,
        Chapter1Story,
        Chapter2Story,

        Chapter00Sign,
        Chapter01Sign,
        Chapter02Sign,
    }

    enum SelectState
    {
        First,
        Middle,
        Last,
    }

    GameObject optionUI;

    GameObject beforeChoose;
    GameObject middleChoose;
    GameObject afterChoose;

    GameObject[] chapters;
    GameObject[] middles;

    Button playButton;

    [SerializeField] int chapternum;
    [SerializeField] string chapterName;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        BacktoMain();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        chapternum  = 0;
        chapterName = null;

        //BindText    (typeof(Texts));
        BindButton  (typeof(Buttons));
        BindObject  (typeof(StageState));


        beforeChoose    = GetObject((int)StageState.BeforeChoose);
        middleChoose    = GetObject((int)StageState.MiddleChoose);
        afterChoose     = GetObject((int)StageState.AfterChoose);

        playButton      = GetButton((int)Buttons.Start);
        playButton.onClick.AddListener(PlayChapter);

        GetButton((int)Buttons.Hint).onClick.AddListener(PopUpHintUI);
        GetButton((int)Buttons.Next).onClick.AddListener(AfterChoice);
        GetButton((int)Buttons.Cancle).onClick.AddListener(BacktoMain);

        GetButton((int)Buttons.Reset).onClick.AddListener(BacktoMain);

        GetButton((int)Buttons.Chapter0Button).onClick.AddListener(()=>
        {
            ChoiceChapterNum(0 , "Chapter0Scene");
        });
        GetButton((int)Buttons.Chapter01Button).onClick.AddListener(() =>
        {
            ChoiceChapterNum(1, "Chapter1Scene");
        });
        GetButton((int)Buttons.Chapter02Button).onClick.AddListener(() =>
        {
            ChoiceChapterNum(2, "Chapter2Scene");
        });

        Button option = GetButton((int)Buttons.Option);

        option.onClick.AddListener(PopUpOptionUI);

        optionUI = option.gameObject;

        middles = new GameObject[] 
        {
            GetObject((int)StageState.Chapter00Sign),
            GetObject((int)StageState.Chapter01Sign),
            GetObject((int)StageState.Chapter02Sign)
        };

        chapters = new GameObject[]
        {
            GetObject((int)StageState.Chapter0Story),
            GetObject((int)StageState.Chapter1Story),
            GetObject((int)StageState.Chapter2Story)
        };

        return true;
    }

    void PlayGame()
    {
        playButton.interactable = false;
        Manager.Scene.LoadScene(chapterName);
    }

    void PlayChapter()
    {
        if (chapternum == 0)
            PlayGame();
        else
        {
            string bundleName = $"chapter{string.Format("{0:D2}", chapternum)}";
            //번들을 갖고있는지 확인
            Manager.DownLoadBundle.HasBundleCheck(bundleName, (state) =>
            {
                //있다면 게임 실행
                if (state)
                    PlayGame();
                else //없으면 번들 다운로드
                    Manager.DownLoadBundle.DownLoad(null,"basic",bundleName);
            });
        }
    }

    void ChoiceChapterNum(int i, string name)
    {
        ShowStageState(SelectState.Middle);
        ShowRightButtons(false);

        chapterName = name;
        chapternum  = i;
        Manager.Chapter.chapter = i;

        middles[chapternum].SetActive(true);
        Manager.Text.TextChange();
    }

    void AfterChoice()
    {
        ShowStageState(SelectState.Last);

        chapters[chapternum].SetActive(true);
        Manager.Text.TextChange();
    }

    void BacktoMain()
    {
        ShowStageState(SelectState.First);
        ShowRightButtons(true);

        middles[chapternum].SetActive(false);

        chapters[chapternum].SetActive(false);
        Manager.Text.TextChange();
    }

    public void BackToMiddle()
    {
        middleChoose.SetActive(true);
        afterChoose.SetActive(false);

        Manager.Text.TextChange();
    }

    void ShowRightButtons(bool state)
    {
        optionUI.SetActive(state);
        //mailUI.SetActive(state);
        //shopUI.SetActive(state);
    }

    void ShowStageState(SelectState state)
    {
        beforeChoose.SetActive  (SelectState.First == state);
        middleChoose.SetActive  (SelectState.Middle == state);
        afterChoose.SetActive   (SelectState.Last == state);
    }

    void PopUpOptionUI()
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>($"UI/Popup/OutGameOptionPanel"));
    }

    void PopUpHintUI()
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>($"UI/Popup/Work/HintShopUI"));
    }

    void PopUpShopUI()
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>($"UI/Popup/Work/ShopUI"));
    }

    void PopUpMailUI()
    {
        Manager.UI.ShowPopUpUI(Manager.Resource.Load<PopUpUI>($"UI/Popup/Work/MailPortUI"));
    }
}
