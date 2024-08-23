using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBeforeCheckPopup : PopUpUI
{
    [SerializeField] Button continueBtn;
    [SerializeField] Button cancleBtn;
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI informationTxt;
    Action playAction;
    Action closeAction;
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 버튼에 액션 연결
    /// </summary>
    protected override void Start()
    {
        base.Start();
        continueBtn.onClick.AddListener(Continue);
        cancleBtn.onClick.AddListener(Cancle);
        Manager.Text.TextChange();
    }

    /// <summary>
    /// 텍스트 값 변경
    /// </summary>
    public void InitText(string _titleTxt, string _informationTxt)
    {
        if(_titleTxt != null)
            titleTxt.text = _titleTxt;
        if(_informationTxt != null)
            informationTxt.text = _informationTxt;
    }

    /// <summary>
    /// 액션 연결
    /// </summary>
    public void InitAction(Action ofcourseButton, Action _cancleButton)
    {
        playAction = ofcourseButton;
        closeAction = _cancleButton;
    }

    /// <summary>
    /// 이어서 하기 값 설정 후 플레이 실행, 팝업 종료
    /// </summary>
    void Continue()
    {
        playAction?.Invoke();
    }

    /// <summary>
    /// 플레이 실행, 팝업 종료
    /// </summary>
    void Cancle()
    {
        closeAction?.Invoke();
        Manager.Chapter.chapterCost = 1;
    }

}
