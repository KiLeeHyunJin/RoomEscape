using BackEnd;
using System.Collections;
using UnityEngine;

public class BackendManager : Singleton<BackendManager>
{

    protected override void Awake()
    {
        base.Awake();

        //뒤끝 서버 초기화
        BackendSetup();

    }

   

    private void Update()
    {
        //서버의 비동기 메소드 호출(콜백 함수 풀링)을 위해 작성
        if (Backend.IsInitialized)
        {
            Backend.AsyncPoll();
        }
    }

    /// <summary>
    /// 뒤끝 서버 초기화
    /// </summary>
    private void BackendSetup()
    {
#if UNITY_EDITOR
        
#endif
        //뒤끝 초기화
        var bro = Backend.Initialize(true);

        //뒤끝 초기화 응답값
        if (bro.IsSuccess())
        {
            //초기화 성공 시 statusCode 204 Success
            Debug.Log($"뒤끝 초기화 성공{bro}");
        }
        else
        {
            //초기화 실패 시 statusCode 400대 에러 발생
            Debug.Log($"뒤끝 초기화 실패{bro}");
        }
    }
}
