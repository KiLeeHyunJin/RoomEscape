using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance
    {
        get
        {
            if (instance == null)
            { CreateGameObject(); }
            return instance;
        }
    }
    static AddressableManager instance;


    List<string> downLoadLabeList;
    long loadSize;
    long patchSize;
    bool isDownloadScene;

    protected void Awake()
    {
        isDownloadScene = false;
    }

    static void CreateGameObject()
    {
        GameObject obj = new GameObject("DownLoadManager");
        instance = obj.AddComponent<AddressableManager>();
        DontDestroyOnLoad(obj);
    }


    //매개변수를 받아서 다운로드 체크를 실행 및 데이터를 전부 불러왔을때 실행할 콜백 함수를 전달 합니다.
    public void DownLoad(Action callBack, params string[] label)
    {
        loadSize = 0;
        StartCoroutine(CheckUpdateFiles(callBack, label));
    }

    /// <summary>
    /// 다운로드 받아야하는 파일이 있는지 확인 후 UI 출력
    /// </summary>
    /// <param name="callBack"> 다운로드가 완료되었을때 실행할 콜백 함수</param>
    /// <param name="labels"> 다운로드를 해서 가져올 레이블 목록</param>
    /// <returns></returns>
    IEnumerator CheckUpdateFiles(Action callBack, params string[] labels)
    {
        // 어드레서블 정보 초기화
        AsyncOperationHandle init = Addressables.InitializeAsync();
        yield return init;

        //다운로드에 필요한 레이블 리스트를 생성 및 초기화
        patchSize = default(long);

        //다운로드 용량 저장
        foreach (string label in labels)
        {
            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;
            patchSize += handle.Result;
        }
        Debug.Log($"PatchSize : {patchSize}");
        //다운로드씬이 존재
        if (isDownloadScene)
        {
            //다운로드 파일이 없다면 2초 대기
            if (DownLoadButtonCheck() == false)
            {
                yield return new WaitForSecondsRealtime(2);
                //씬 전환 추가할지 말지 고민
            }
        }
        else
        {
            //다운로드씬이 존재하지 않는다면 다운로드 실행 확인 및 다운로드
            StartRoutine(PatchFiles(callBack, labels));
        }
    }

    /// <summary>
    /// 로딩씬이 있고 다운로드를 선택사항으로 사용할 시 다운로드 버튼을 활성화(버튼을 통해 다운로드를 진행하고 싶을 시)
    /// </summary>
    /// <returns></returns>
    bool DownLoadButtonCheck()
    {
        //다운로드 용량 확인
        if (patchSize > decimal.Zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void StartRoutine(IEnumerator routine) => StartCoroutine(routine);

    /// <summary>
    /// 다운로드 코루틴
    /// </summary>
    IEnumerator PatchFiles(Action callBack, params string[] labels)
    {
        downLoadLabeList = new List<string>(labels);
        //다운로드 파일 용량 체크
        var handle = Addressables.GetDownloadSizeAsync(downLoadLabeList);
        yield return handle;
        Debug.Log($"DownloadSize : {handle.Result}");
        if (handle.Result != decimal.Zero)
        {
            //다운로드 실행
            StartRoutine(DownLoadGroup(downLoadLabeList));
            //StartCoroutine();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("다운로드 파일을 갖고있습니다.");
#endif

        }
        // 다운로드 용량 표기 실행
        yield return CheckDownLoadValue(callBack);
    }


    /// <summary>
    /// 레이블에 맞는 리소스를 다운로드
    /// </summary>
    IEnumerator DownLoadGroup(List<string> labelList)
    {
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(labelList, Addressables.MergeMode.Union);
        yield return handle;
        //다운로드가 끝나지 않았다면
        while (handle.IsDone == false)
        {
            //현재 다운받는 레이블의 다운로드 용량을 대입
            loadSize = handle.GetDownloadStatus().DownloadedBytes;
#if UNITY_EDITOR
            Debug.Log($"DownLoadLabel : {handle.IsDone}");
#endif
            yield return null;
            //yield return new WaitForEndOfFrame();
        }
#if UNITY_EDITOR
        Debug.Log("다운로드 완료하였습니다.");
#endif
        //총 다운로드 용량 대입
        loadSize = handle.GetDownloadStatus().TotalBytes;
        //핸들 해제
        Addressables.Release(handle);
    }

    /// <summary>
    /// 다운로드 용량 체크
    /// </summary>
    IEnumerator CheckDownLoadValue(Action callBack)
    {
        //로딩 씬이 존재한다면 값 표기
        if (isDownloadScene)
        {

        }

        while (true)
        {
            //현재 다운로드 용량 표기
            if (isDownloadScene)
            {
                //다운로드 받아야할 용량과 받은 용량이 같다면 
                if (loadSize == patchSize)
                {
                    break;
                }
            }
            //로딩 데이터를 모두 받았다면 반복문 탈출
            if (loadSize == patchSize)
                break;
            yield return new WaitForEndOfFrame();
        }
        //콜백함수 실행
        callBack?.Invoke();
    }


    #region 비트 사이즈를 문자열로 변환
    /// <summary>
    /// 바이트 값을 문자열로 변환
    /// </summary>
    /// <param name="byteCnt"></param>
    /// <returns></returns>
    //string GetFileSize(long byteCnt)
    //{
    //    byteStrBuilder.Clear();
    //    if (byteCnt >= 1073741824.0)
    //    {
    //        byteStrBuilder.Append(string.Format("{0:##.##}", byteCnt / 1073741824.0));
    //        byteStrBuilder.Append("GB");
    //    }
    //    else if (byteCnt >= 1048576.0)
    //    {
    //        byteStrBuilder.Append(string.Format("{0:##.##}", byteCnt / 1048576.0));
    //        byteStrBuilder.Append("MB");
    //    }
    //    else if (byteCnt >= 1024.0)
    //    {
    //        byteStrBuilder.Append(string.Format("{0:##.##}", byteCnt / 1024.0));
    //        byteStrBuilder.Append("KB");
    //    }
    //    else if (byteCnt > 0 && byteCnt < 1024.0)
    //    {
    //        byteStrBuilder.Append(string.Format("{0:##.##}", byteCnt / 1073741824.0));
    //        byteStrBuilder.Append("Byte");
    //    }
    //    else
    //    {
    //        byteStrBuilder.Append("0 Bytes");
    //    }
    //    return byteStrBuilder.ToString();
    //}
    #endregion

}
