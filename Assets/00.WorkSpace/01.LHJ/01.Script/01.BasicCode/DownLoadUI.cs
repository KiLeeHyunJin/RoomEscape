using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadUI : PopUpUI
{
    //[SerializeField] Slider slider;
    //[SerializeField] Button nextButton;
    //[SerializeField] Button cancleButton;
    //[SerializeField] TextMeshProUGUI stateTxt;
    //LobbyMain lobby;
    //FixedUI fixedUI;
    //Action downloadAction;
    //protected override void Awake()
    //{
    //    base.Awake();
    //}
    ///// <summary>
    ///// 초기화 함수
    ///// </summary>
    //public override bool Init()
    //{
    //    if (base.Init() == false)
    //        return false;
    //    //다음 버튼 비활성화
    //    nextButton.interactable = false;
        
    //    return true;
    //}

    //protected override void Start()
    //{
    //    base.Start();
    //    //챕터 선택 클래스가 없다면
    //    if(lobby == null) //찾아서 대입
    //        lobby = FindObjectOfType<LobbyMain>();

    //    //아웃게임 버튼 컴포넌트 대입
    //    fixedUI = FindObjectOfType<FixedUI>();
    //    //다음버튼에 콜백 메소드 대입
    //    nextButton.onClick.AddListener(NextAction);
    //    //취소버튼에 콜백 메소드 대입
    //    cancleButton.onClick.AddListener(CancleAction);
    //    //ui상태 비활성화
    //    UIState(false);
    //    //다운로드중 텍스트 출력
    //    startText();
    //    Manager.Text.TextChange();
    //}
    //void startText()
    //{
    //    BackendChartData.logChart.TryGetValue(174, out LogChartData logChartData);
    //    if (logChartData != null)
    //    {
    //        if (Manager.Text._Iskr == true)
    //        {
    //            stateTxt.text = logChartData.korean;
    //        }
    //        else
    //        {
    //            stateTxt.text = logChartData.english;
    //        }
    //    }
    //}
    //void CompleteText()
    //{
    //    BackendChartData.logChart.TryGetValue(175, out LogChartData logChartData);
    //    if (logChartData != null)
    //    {
    //        if (Manager.Text._Iskr == true)
    //        {
    //            stateTxt.text = logChartData.korean;
    //        }
    //        else
    //        {
    //            stateTxt.text = logChartData.english;
    //        }
    //    }
    //}
    ///// <summary>
    ///// 다운로드 취소 메소드
    ///// </summary>
    //void CancleAction()
    //{
    //    Close();
    //    Manager.DownLoadBundle.DownloadCancle();
    //}

    ///// <summary>
    ///// 씬을 전환하고 UI 전부 true
    ///// </summary>
    //void NextAction()
    //{
    //    //ui전부 활성화
    //    UIState(true);
    //    StartCoroutine(StartGame());
    //}

    //IEnumerator StartGame()
    //{
    //    do
    //    {
    //        yield return null;
    //        //choiceUI컴포넌트가 null이 아닐때까지 반복
    //        lobby = FindObjectOfType<LobbyMain>();
    //        Debug.Log("Recall");
    //    }
    //    while (lobby == null);
    //    //게임 시작 메소드
    //    lobby.PlayChapter();
    //    //팝업 전부 종료
    //    Manager.UI.ClearPopUpUI();
    //}

    ///// <summary>
    ///// ui상태를 전부 매개변수 상태로 설정
    ///// </summary>
    //void UIState(bool state)
    //{
    //    if (fixedUI != null)
    //        fixedUI.gameObject.SetActive(state);
    //    if (lobby != null)
    //        lobby.gameObject.SetActive(state);
    //}

    ///// <summary>
    ///// 다운로드 다음 버튼에 콜백 메소드 대입
    ///// </summary>
    //public void DownloadAction(Action action)
    //{
    //    if(action != null)
    //        downloadAction = action;
    //    nextButton.onClick.AddListener(()=> downloadAction?.Invoke());
    //}

    ///// <summary>
    ///// 슬라이드 값을 매개변수값으로 설정
    ///// </summary>
    public void GetStateValue(float value)
    {
        //slider.value = value;
        //if (nextButton.interactable)
        //    nextButton.interactable = false;
        //Message.Log($" Percentage : {value}");
    }

    /// <summary>
    /// 슬라이드 값을 최대값으로 설정 및 다음 버튼 활성화
    /// </summary>
    public void Complete()
    {
        //slider.value = 1;
        //nextButton.interactable = true;
        //// 다운로드 완료 텍스트 출력
        //CompleteText();
    }

    /// <summary>
    /// UI상태를 전부 활성화
    /// </summary>
    private void OnDestroy()
    {
        //UIState(true);
    }
}
