using BackEnd;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackendLogin : MonoBehaviour
{
    [SerializeField] Button guestLogin;
    [SerializeField] Button googleLogin;

    [SerializeField] GameObject buttonGroub;
    [SerializeField] GameObject loginSuccess;
    [SerializeField] Button touchButton;
    [SerializeField] TMP_Text errorMassege;

    private void Awake()
    {
        guestLogin.onClick.AddListener(GuestLogin);
        googleLogin.onClick.AddListener(GoogleLogin);
        touchButton.onClick.AddListener(SceneChange);
        touchButton.interactable = false;
        Manager.Data.RequestPermissions();
    }

    private void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            guestLogin.interactable = false;
            errorMassege.text = "오프라인 상태입니다. \n온라인으로 전환하여 재접속을 하시기 바랍니다.";
        }
    }

    public void SceneChange()
    {
        Manager.Scene.LoadScene("LobbyScene");
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
            // Backend.Dat파일이 있으면 오프라인 플레이 지원
            if (Backend.UserInDate != null)
            {
                buttonGroub.SetActive(false);
                loginSuccess.SetActive(true);

                //모든 차트 데이터 불러오기 
                BackendChartData.LoadAllChart();
            }

            //없을 경우 인터넷 연결 필요 안내
            else
            {
                errorMassege.text = "게임 정보 다운로드를 위해 최초 1회 데이터 연결이 필요합니다.";
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
                    buttonGroub.SetActive(false);   
                    loginSuccess.SetActive(true);

                    Debug.Log($"게스트 로그인에 성공했습니다 {callback}");

                    // 최초 접속 시 게임정보 생성
                    if (int.Parse(callback.GetStatusCode()) != 200)
                    {
                        BackendGameData.Instance.GameDataInsert();
                        BackendGameData.Instance.AllChapterDataInsert();
                        //다른 데이터 생성 추가(챕터, 힌트, 클리어)
                    }

                    //모든 차트 데이터 불러오기 
                    BackendChartData.LoadAllChart();

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

                touchButton.interactable = true;

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



