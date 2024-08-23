using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserGameData;

public class GameEndPopup : PopUpUI // 게임(챕터)이 종료될 때 생성될 팝업
{
    [SerializeField] GameObject offPanel; 
    [SerializeField] GameObject openPanel; 
    [SerializeField] TextMeshProUGUI title; 
    [SerializeField] TextMeshProUGUI subTitle; 
    [SerializeField] TextMeshProUGUI content; 
    [SerializeField] string[] titleNums; 
    [SerializeField] string[] subTitleNums;  
    [SerializeField] string[] contentNums;  

    /// <summary>
    /// 챕터를 클리어한 경우 호출 /
    /// 게임 클리어 처리 후 로비로 이동
    /// </summary>
    /// <param name="sceneName"> 해당 씬으로 이동 </param>
    public void ChapterClear(string sceneName)
    {
        Manager.Game.GameClear(); // 게임 클리어 처리
        ReturnLobby(sceneName); // 로비로 이동
    }

    /// <summary>
    /// 챕터를 클리어하지 않고 나가거나, 타이머가 종료되는 경우 호출 /
    /// 게임 포기 처리 후 로비로 이동
    /// </summary>
    /// <param name="sceneName"> 해당 씬으로 이동 </param>
    public void ChapterGiveUp(string sceneName)
    {
        Manager.Game.SetGiveUp(); // 게임 포기 처리
        ReturnLobby(sceneName); // 로비로 이동
    }

    /// <summary>
    /// 로비로 이동하는 함수 /
    /// 유저 데이터를 초기화하고, 씬을 변경
    /// </summary>
    /// <param name="sceneName"> 이동할 씬 이름 </param>
    void ReturnLobby(string sceneName)
    {
        Manager.Data.UserGameData.SetData(GameDataEnum.Chapter, -1); // 진행중인 챕터 데이터를 초기화
        Manager.Inventory.Init(); // 인벤토리 초기화
        Manager.UI.ClearPopUpUI(); // 모든 팝업 UI 제거
        Manager.Game.ShowNewAbs(); 
        Manager.Scene.LoadScene(sceneName); // 지정된 씬으로 이동
    }

    /// <summary>
    /// 게임 종료 팝업이 활성화될 때 호출 /
    /// 텍스트 업데이트 및 패널 전환 루틴 시작
    /// </summary>
    private void OnEnable()
    {
        TextName(); // 텍스트 이름 설정
        if (offPanel != null && openPanel != null)
        {
            StartCoroutine(openPanelRoutine()); // 패널 전환 루틴 시작
        }

        // 게임의 제한 시간이 남아있는 경우
        if (Manager.Game.LimitTime > 0)
        {
            Manager.Sound.PlayBGMByIndex(3); // BGM 재생
            Manager.Sound.PlayButtonSound(15); // 버튼 클릭 소리 재생
        }
        else // 제한 시간이 없을 경우
        {
            Manager.Sound.StopBGM(); // BGM 중지
            Manager.Sound.PlayButtonSound(16); // 다른 버튼 클릭 소리 재생
        }
    }

    /// <summary>
    /// 팝업의 텍스트 요소들의 이름을 설정 /
    /// 타이틀, 서브 타이틀, 내용의 이름을 설정하고 텍스트를 갱신
    /// </summary>
    private void TextName()
    {
        if (title != null && subTitle != null && content != null)
        {
            title.name = titleNums[Manager.Chapter.chapter]; // 타이틀 이름 설정
            subTitle.name = subTitleNums[Manager.Chapter.chapter]; // 서브 타이틀 이름 설정
            content.name = contentNums[Manager.Chapter.chapter]; // 내용 이름 설정
            Manager.Text.TextChange(); // 텍스트 변경 처리
        }
    }

    /// <summary>
    /// 패널 전환 루틴 /
    /// 일정 시간 후에 비활성화 패널을 끄고 활성화 패널을 켬
    /// </summary>
    IEnumerator openPanelRoutine()
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        offPanel.SetActive(false); // 비활성화 패널 끄기
        openPanel.SetActive(true); // 활성화 패널 켜기
        Manager.Text.TextChange(); // 텍스트 변경 처리
    }

    /// <summary>
    /// 재시작 버튼을 클릭했을 때 호출 /
    /// 인게임 씬으로 이동하고 팝업 UI 제거
    /// </summary>
    public void ReStart()
    {
        Manager.Scene.LoadScene("InGameScene"); // 인게임 씬으로 이동
        Manager.UI.ClearPopUpUI(); // 모든 팝업 UI 제거
    }
}
