using BackEnd;
using LitJson;
using System;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    [Serializable]
    public class UserInfoEvent : UnityEngine.Events.UnityEvent { }
    public UserInfoEvent onUserInfoEvent = new UserInfoEvent();

    private static UserInfoData data = new UserInfoData();
    public static UserInfoData Data => data;

    /// <summary>
    /// 현재 로그인한 유저의 정보 불러오기
    /// </summary>
    public void GetUserInfoFromBackend()
    {
        Backend.BMember.GetUserInfo(callback =>
        {
            //불러오기 성공
            if (callback.IsSuccess())
            {
                //Json 데이터 파싱 성공
                try
                {
                    JsonData json = callback.GetReturnValuetoJSON()["row"];

                    data.gamerId = json["gamerId"].ToString();
                    data.countryCode = json["countryCode"]?.ToString();
                    data.nickname = json["nickname"]?.ToString();
                    data.inDate = json["inDate"].ToString();
                    data.emailForFindPassword = json["emailForFindPassword"]?.ToString();
                    data.subscriptionType = json["subscriptionType"].ToString();
                    data.federationId = json["federationId"]?.ToString();
                }
                //Json 데이터 파싱 실패
                catch (Exception ex)
                {
                    //유저 정보를 기본 상태로 설정 
                    data.Reset();
                    //try-catch 에러 출력
                    Debug.Log(ex);
                }
            }
            //불러오기 실패
            else
            {
                //유저 정보를 기본 상태로 설정
                //오프라인 상태를 대비해 기본적인 정보를 저장해두고 오프라인일 때 불러와서 사용
                data.Reset();
                Debug.Log(callback.GetMessage());
            }

            //불러오기가 끝난 뒤 onUserInfoEvent에 등록된 이벤트 메소드 호출
            onUserInfoEvent?.Invoke();
        });
    }
}



public class UserInfoData
{
    public string gamerId;                   //유저의 gamerID;
    public string countryCode;               //국가코드, 설정 안했으면 null
    public string nickname;                  //닉네임, 설정 안했으면 null
    public string inDate;                    //유저의 inDate
    public string emailForFindPassword;      //이메일주소, 설정안했으면 null
    public string subscriptionType;          //커스텀, 페더레이션 타입
    public string federationId;              //구글, 애플, 페이스북 페더레이션 ID, 커스텀 계정은 null
    
    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void Reset() 
    {
        gamerId = "offline";
        countryCode = "Unknown";
        nickname = "Noname";
        inDate = string.Empty;
        emailForFindPassword = string.Empty;
        subscriptionType = string.Empty;
        federationId = string.Empty;
    }
}
