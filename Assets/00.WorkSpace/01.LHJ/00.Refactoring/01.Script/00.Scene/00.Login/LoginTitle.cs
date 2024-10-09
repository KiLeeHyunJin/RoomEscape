using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginTitle : BaseUI
{
    enum Buttons
    {
        GuestLogin,
        GoogleLogin,
        GuestInfoDeleteButton,
        LoginBackground
    }
    enum Texts
    {
        ErrorMessage,
    }
    enum InputField
    {
        NickNameSetInput
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindInputField(typeof(InputField));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        return true;
    }

    TMP_Text errorMassege;
    GameFlow flowManager;
    string ErrorMassege 
    { 
        set 
        { 
            if(errorMassege.gameObject.activeSelf == false)
                errorMassege.gameObject.SetActive(true);
            errorMassege.text = value;
        } 
    }

    protected override void Awake()
    {
        flowManager = FindObjectOfType<GameFlow>();
        flowManager.ChangeGameScene(GameFlow.GameState.Login);
    }

    protected override void Start()
    {
        base.Start();


        errorMassege            = GetText((int)Texts.ErrorMessage);

        Button guestLogin       = GetButton((int)Buttons.GuestLogin);
        Button googleLogin      = GetButton((int)Buttons.GoogleLogin);
        Button loginButton      = GetButton((int)Buttons.LoginBackground);
        Button guestDeleteInfo  = GetButton((int)Buttons.GuestInfoDeleteButton);

        guestLogin      .onClick.AddListener(GuestLogin);
        googleLogin     .onClick.AddListener(GoogleLogin);
        loginButton     .onClick.AddListener(ChangeScene);
        guestDeleteInfo .onClick.AddListener(Delete);

        loginButton .gameObject.SetActive(false);
        errorMassege.gameObject.SetActive(false);

        //인터넷 비연결 상태
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ErrorMassege = "인터넷 연결을 확인해주시기 바랍니다.";
            GetButton((int)Buttons.GuestLogin).interactable = false;
            GetButton((int)Buttons.GoogleLogin).interactable = false;
        }
    }

    void GoogleLogin()
    {
        ErrorMassege = "준비중인 기능입니다.";
    }

    void ChangeScene()
    {
        //Manager.Scene.LoadScene("LobbyScene");
        Manager.Backend.SetLogin();
        flowManager.ChangeGameScene(GameFlow.GameState.Lobby);
    }
    void Delete()
    {
        Backend.BMember.DeleteGuestInfo();
        Caching.ClearCache();
    }

    /// <summary>
    /// 게스트로그인
    /// </summary>
    public void GuestLogin()
    {
        //인터넷 비연결 상태
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //닉네임으로 Backend.Dat파일 유무를 판단하여 오프라인 플레이 지원
            if (Backend.UserNickName != null)
            {
                ErrorMassege = "오프라인으로 게임에 접속합니다.";
                BackendChartData.LoadAllChart();//모든 차트 데이터 불러오기 
                Manager.UI.ShowScene("LobbyRect");//로비씬 이동
            }
            else//닉네임 없을 경우 인터넷 연결 메세지 
            {
                ErrorMassege = "게임 정보 다운로드를 위해 데이터 연결이 필요합니다.";
            }
        }
        else//인터넷 연결 상태
        {
            Backend.BMember.GuestLogin($"게스트 로그인으로 로그인함", callback =>
            {
                //게스트로그인 성공
                if (callback.IsSuccess())
                {
                    LoginSuccess(callback);
                }
                else //게스트로그인 실패
                {
                    LoginFail(int.Parse(callback.GetStatusCode()));
                }
            });
        }
    }

    void LoginSuccess(BackendReturnObject callback)
    {
        Manager.DownLoadBundle.LoadToServerVersion();
        GetButton((int)Buttons.GoogleLogin).gameObject.SetActive(false);
        GetButton((int)Buttons.GuestLogin).gameObject.SetActive(false);
        GetButton((int)Buttons.LoginBackground).gameObject.SetActive(true);

        Debug.Log($"게스트 로그인에 성공했습니다 {callback}");

        // 최초 접속 시 게임정보 생성
        if (int.Parse(callback.GetStatusCode()) != 200)
        {
            BackendGameData.Instance.GameDataInsert();
            BackendGameData.Instance.AllChapterDataInsert();
        }

        //모든 차트 데이터 불러오기 
        BackendChartData.LoadAllChart();
    }

    void LoginFail(int errorCodeNum)
    {
        if (errorCodeNum > 400)
        {
            StartCoroutine(AutoLogin());
        }
        else
        {
            ErrorMassege = errorCodeNum switch
            {
                400 => "undefined device_unique_id, device_unique_id을(를) 확인할 수 없습니다",
                401 => "bad customId, 잘못된 customId 입니다",
                403 => "Forbidden blocked user, 금지된 blocked user OR Forbidden blocked device, 금지된 blocked device",
                410 => "Gone user, 사라진 user 입니다",
                _ => "",
            };
        }
    }


    IEnumerator AutoLogin()
    {
        DeleteGuestInfo guestId = FindObjectOfType<DeleteGuestInfo>();
        if (guestId == false)
            guestId.Delete();

        yield return new WaitForSeconds(1);
    }
}



