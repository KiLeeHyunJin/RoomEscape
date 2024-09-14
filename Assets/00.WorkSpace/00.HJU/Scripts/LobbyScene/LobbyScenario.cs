using UnityEngine;

public class LobbyScenario : MonoBehaviour
{
    [SerializeField] PopUpUI prologue;
    [SerializeField] PopUpUI explain;
    //[SerializeField] UserInfo user;
    private void Awake()
    {
        //주석처리 이유 -> 현재 인게임에서 유저정보를 활용하지 않음.

        //로그인한 유저 정보 불러오기
        //user.GetUserInfoFromBackend(); 
    }
    private void Start()
    {
        //제이슨에서 데이터 로드
        //Manager.Data.LoadData(DataManager.DataName.UserData);
        //Manager.Data.LoadData(DataManager.DataName.ChapterClear);

        //챕터 진행도가 0인지 확인
        if (Manager.Data.UserGameData.chapter == 0)
        {
            Debug.Log($"chapter : {Manager.Data.UserGameData.chapter}");
            Manager.UI.ShowPopUpUI(prologue);
            //튜토리얼 실행 메서드 추가
        }
        else if (Manager.Data.UserGameData.chapter != 0 && Manager.Data.UserGameData.lobbyInfo)
        {
            Debug.Log($"lobbyInfo : {Manager.Data.UserGameData.lobbyInfo}");
            // 로비 첫 진입시 챕터슬라이드 설명 팝업
            Manager.UI.ShowPopUpUI(explain);
        }
        else
        {

        }
        //서버에서 데이터 로드
        //서버 데이터로 최신화하기 위함이 아닌 GameDataUpdate();를 사용하기 위해 게임정보의고유값(inDate)을 불러오는 용도
        //BackendGameData.Instance.GameDataLoad();
    }

    [ContextMenu("ClearPopup")]
    public void ShowTuto()
    {
        Manager.UI.ShowPopUpUI(prologue);
    }
}
