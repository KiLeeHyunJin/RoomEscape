using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LoginTitle : BaseUI
{
    [SerializeField] PopUpUI nickNameSetPop;
    TMP_Text errorMassege;


    enum Buttons
    {
        GuestLogin,
        GoogleLogin,
        GuestInfoDeleteButton,
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

    protected override void Start()
    {
        base.Start();

        //인터넷 비연결 상태
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            
        }
        errorMassege = GetText((int)Texts.ErrorMessage);

        GetButton((int)Buttons.GoogleLogin).onClick.AddListener(GoogleLogin);
        GetButton((int)Buttons.GuestLogin).onClick.AddListener(GuestLogin);
    }

    /// <summary>
    /// 구글 로그인(앱 등록 시 구현)
    /// </summary>
    public void GoogleLogin()
    {
        errorMassege.text = "준비중인 기능입니다.";
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
                errorMassege.text = "오프라인으로 게임에 접속합니다.";

                //모든 차트 데이터 불러오기 
                BackendChartData.LoadAllChart();

                //로비씬 이동
                Manager.UI.ShowScene("LobbyRect");
            }

            //닉네임 없을 경우 인터넷 연결 메세지 
            else
            {
                errorMassege.text = "게임 정보 다운로드를 위해 데이터 연결이 필요합니다.";
            }
        }

        //인터넷 연결 상태
        else
        {
            Backend.BMember.GuestLogin($"게스트 로그인으로 로그인함", callback =>
            {
                //게스트로그인 성공
                if (callback.IsSuccess())
                {

                    //로컬에 차트데이터 확인하여 없거나 다를 경우 저장


                    //회원가입하는 경우 OR 회원가입만 하고 닉네임을 만들지 않았던 경우
                    if (int.Parse(callback.GetStatusCode()) == 201 || Backend.UserNickName == "")
                    {
                        Debug.Log($"게스트 회원가입에 성공했습니다 {callback}");
                        //닉네임 설정 팝업 
                        PopUpUI nickNamePopup = Manager.UI.ShowPopUpUI(nickNameSetPop);
                    }

                    //기존에 아이디가 있는 경우
                    else
                    {
                        Debug.Log($"게스트 로그인에 성공했습니다 {callback}");

                        //모든 차트 데이터 불러오기 
                        BackendChartData.LoadAllChart();

                        //로비씬 이동
                        Manager.UI.ShowScene("LobbyRect");
                    }
                }

                //게스트로그인 실패
                else
                {
                    int errorCodeNum = int.Parse(callback.GetStatusCode());
                    switch (errorCodeNum)
                    {
                        case 400:
                            errorMassege.text = "undefined device_unique_id, device_unique_id을(를) 확인할 수 없습니다";
                            break;
                        case 401:
                            errorMassege.text = "bad customId, 잘못된 customId 입니다";
                            break;
                        case 403:
                            errorMassege.text = "Forbidden blocked user, 금지된 blocked user OR Forbidden blocked device, 금지된 blocked device";
                            break;
                        case 410:
                            errorMassege.text = "Gone user, 사라진 user 입니다";
                            break;
                    }
                    if (errorCodeNum >= 400)
                    {
                        StartCoroutine(AutoLogin());
                    }
                }
            });
        }
       
    }
    IEnumerator AutoLogin()
    {
        DeleteGuestInfo guestId = FindObjectOfType<DeleteGuestInfo>();
        if (guestId == false)
        {
            guestId.Delete();
        }
        yield return new WaitForSeconds(1);
        GuestLogin();
    }
}



