using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class DownLoadBundleManager : Singleton<DownLoadBundleManager>
{
    const string DOWNLOADPATH    = "https://drive.google.com/uc?export=download&id=";
    const string SERVERVERSIONURL= "https://docs.google.com/spreadsheets/d/1pu9z0RT1m9YmvsiVd7uOAaL8JTutDJIVMhLrMT68RuI/export?format=csv";  // 서버 버전 테이블 접속 URL
    const string LOCALVERSIONURL = "/versionTable.csv"; // 로컬 버전 테이블 경로

    Dictionary<string, string[]> localVersionTable;
    Dictionary<string, string[]> serverVersionTable;
    Dictionary<string, string[]> VersionTable 
    { 
        get 
        {
            if (serverVersionTable != null)
                return serverVersionTable;
            if (localVersionTable != null)
                return localVersionTable;
            return null;
        } 
    }
    string LocalVersionPath
    {
        get { return Manager.Data.DataPath; }
    }

    PlayerBeforeCheckPopup instancePopupUI;
    [SerializeField] DownLoadUI downLoadUI;

    string titleText;
    string detailText;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(CreateDirectory(LoadToServerVersion));
    }

    Coroutine checking;
    Coroutine downloadCo;

    private void OnDestroy()
    {
        if (checking != null)
            StopCoroutine(checking);
    }
    private void Start()
    {
#if UNITY_EDITOR
        checking = StartCoroutine(CurrentLoadedBundle());
#endif
    }

    /// <summary>
    /// 현재 로드되어있는 번들 콘솔창에 출력
    /// </summary>
    IEnumerator CurrentLoadedBundle()
    {
        StringBuilder sb = new();

        while (true)
        {
            foreach (var item in AssetBundle.GetAllLoadedAssetBundles())
                sb.Append(item.name + ", ");

            string write = sb.ToString();
            if (string.IsNullOrEmpty(write) == false)
            {
                Message.Log(write);
                sb.Clear();
            }
            yield return new WaitForSeconds(10);
        }
    }

    /// <summary>
    /// 번들이 있으면 로드
    /// </summary>
    public void GetLoadBundle(string bundleName, Action<AssetBundle> bundleCall)
    {
        HasBundle(bundleName, (state) =>
        {
            if (state)
            {
                string[] bundleData = VersionTable[bundleName];
                UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(
                    ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(bundleData[(int)VersionTableColumn.Version]));

                uwr.SendWebRequest().completed +=
                (operat) =>
                {
                    bundleCall?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
                };
            }
            else
            {
                Message.Log("해당 번들이 없습니다.");
                bundleCall?.Invoke(null);
            }
        });
    }

    /// <summary>
    /// 번들이 있으면 로드하고 없으면 다운로드 후 로드
    /// </summary>
    public void GetLoadOrDownloadBundle(string bundleName, Action<AssetBundle> bundleCall)
    {
        //파일을 갖고있는지 확인
        HasBundle(bundleName, (state) =>
        {
            if (state)
            {
                StartCoroutine(GetBundleRoutine(bundleName, bundleCall));
            }
            else
            {
                //버전에 포함되어있는지 확인
                IncludeVersionCheck(bundleName, (checkNameState) =>
                {
                    if (checkNameState)
                    {
                        //다운로드 확인
                        StartCoroutine(DownloadBundle(bundleName, bundleCall));
                    }
                    else
                    {
                        //버전 쳌
                        LoadToServerVersion((refreshState) =>
                        {
                            //포함 확인
                            IncludeVersionCheck(bundleName, (reCheckNameState) =>
                            {
                                if (reCheckNameState)
                                {
                                    //다운로드 확인
                                    StartCoroutine(DownloadBundle(bundleName, bundleCall));
                                }
                                else
                                {
                                    Message.LogError($"CompulsoryDownload Bundle 이름이 잘못됨 : {bundleName}");
                                }
                            });
                        });
                    }
                });
            }
        });
    }

    /// <summary>
    /// 다운로드 팝업창 출력
    /// </summary>
    void SetText()
    {
        BackendChartData.logChart.TryGetValue(171, out LogChartData logChartData);
        if (logChartData != null)
        {
            if (Manager.Text._Iskr == true)
            {
                detailText = logChartData.korean;
            }
            else
            {
                detailText = logChartData.english;
            }
        }
        int datailTextNum = Manager.Chapter.chapter switch
        {
            1 => 2000,
            2 => 3000,
            3 => 4000,
            4 => 5000,
            5 => 6000,
            _ => 9999,
        };
        BackendChartData.logChart.TryGetValue(datailTextNum, out LogChartData logChartData2);
        if (logChartData != null)
        {
            if (Manager.Text._Iskr == true)
            {
                titleText = logChartData2.korean;
            }
            else
            {
                titleText = logChartData2.english;
            }
        }
        instancePopupUI.InitText(titleText, detailText);
    }

    

    /// <summary>
    /// 다운로드 중일 경우 다운로드 취소
    /// </summary>
    public void DownloadCancle()
    {
        if (downloadCo != null)
            StopCoroutine(downloadCo);

        if (Manager.Chapter.chapter == 0)
            return;

        string bundleName = $"chapter{string.Format("{0:D2}", Manager.Chapter.chapter)}";
        //번들 소유 확인
        HasBundle(bundleName, (state) =>
        {
            Manager.Resource.ReleaseBundle();
            Caching.ClearAllCachedVersions(bundleName);
        });
    }

    /// <summary>
    /// 해당 번들이 다운로드되어있는지 확인 후 콜백
    /// </summary>
    public void HasBundle(string bundleFileName, Action<bool> stateCall)
    {
        if (VersionTable == null)
        {
            LoadToServerVersion((refreshState) =>
            {
                if (refreshState)
                {
                    IsVersionCached(bundleFileName, stateCall);
                }
                else
                {
                    Message.LogError("버전 로드에서 실패함 : HasBundleCheck");
                    stateCall?.Invoke(false);
                }
            });
        }
        else
        {
            IsVersionCached(bundleFileName, stateCall);
        }
    }

    /// <summary>
    /// 해당 번들이 있는지 확인. 있다면 버전 확인
    /// </summary>
    void IsVersionCached(string bundleFileName, Action<bool> call)
    {
        if (localVersionTable.ContainsKey(bundleFileName))
        {
            string[] bundleData = VersionTable[bundleFileName];
            call?.Invoke(Caching.IsVersionCached(
                       ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
                       Hash128.Parse(bundleData[(int)VersionTableColumn.Version])));
        }
        else
            call?.Invoke(false);
    }

    /// <summary>
    /// 번들을 가져온다.
    /// </summary>
    IEnumerator GetBundleRoutine(string bundleName, Action<AssetBundle> bundleCall)
    {
        string[] bundleData = VersionTable[bundleName];

        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(
            ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
            Hash128.Parse(bundleData[(int)VersionTableColumn.Version]));

        UnityWebRequestAsyncOperation oper = uwr.SendWebRequest();

        while (oper.isDone == false)
            yield return new WaitForFixedUpdate();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
            bundleCall?.Invoke(bundle);
        }
    }

    /// <summary>
    /// 해당 번들이 버전에 포함되어있는지 확인
    /// </summary>
    public void IncludeVersionCheck(string bundleName, Action<bool> call)
    {
        if (localVersionTable != null)
        {
            bool state = VersionTable.ContainsKey(bundleName);
            call?.Invoke(state);
            return;
        }
        LoadToServerVersion((refreshState) =>
        {
            bool state = VersionTable.ContainsKey(bundleName);
            if (refreshState)
                call?.Invoke(state);
            else
                Message.LogError("상태 이상 : IncludeCurrentVersionCheck");
        });
    }

    /// <summary>
    /// 자동 다운로드
    /// </summary>
    IEnumerator DownloadBundle(string bundleName, Action<AssetBundle> callBundle = null)
    {
        int idx = -1;
        for (int i = 0; i < VersionTable.Count; i++)
        {
            if (string.Equals(VersionTable[bundleName][(int)VersionTableColumn.FileName], bundleName))
            {
                idx = i;
                break;
            }
        }
        if (idx < 0)
        {
            callBundle?.Invoke(null);
            yield break;
        }

        //번들 url 재설정 및 요청
        string[] bundleData = VersionTable[bundleName];
        string url = ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]);

        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();

        //번들 로드
        UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Parse(bundleData[(int)VersionTableColumn.Version]));
        yield return DownLoadRoutine(tupleUwr);

        if (tupleUwr.result == UnityWebRequest.Result.Success)
            callBundle?.Invoke(DownloadHandlerAssetBundle.GetContent(tupleUwr));
        else
            callBundle?.Invoke(null);
    }

    /// <summary>
    /// 번들 다운로드 팝업
    /// </summary>
    public void DownLoadPopUp(Action call, params string[] downloadFileName)
    {
        List<string> downloadFileNameList = new(downloadFileName);
        //로컬 테이블이 없다면 
        if (VersionTable == null)
        {
            LoadToServerVersion((refeshState) =>
            {
                if (refeshState)
                    DownLoadPopUpProcess(downloadFileNameList, call);
                else
                {
                    Message.LogError("DownLoad _ VersionRefresh refresh Error");
                }
            });
        }
        else
        {
            DownLoadPopUpProcess(downloadFileNameList, call);
        }
    }

    /// <summary>
    /// 다운로드 준비
    /// </summary>
    void DownLoadPopUpProcess(List<string> downloadFileNameList, Action call)
    {
        MakeDownloadList(downloadFileNameList, out List<string> downloadList);
        StartCoroutine(DownloadPopUpCoreRoutine(call, downloadList));
    }

    


    /// <summary>
    /// 팝업창 다운로드 진행
    /// </summary>
    IEnumerator DownLoadProcessRoutine(Queue<(UnityWebRequest, string)> waitQueue, int downloadCount, uint fetchSize)
    {
        foreach ((UnityWebRequest, string) pair in waitQueue)
            Caching.ClearCachedVersion(pair.Item2, Hash128.Parse(VersionTable[pair.Item2][(int)VersionTableColumn.Version]));

        //다운로드 팝업 활성화
        DownLoadUI _downloadUI = Manager.UI.ShowPopUpUI(downLoadUI);
        List<(UnityWebRequest, string)> progressList = new();

        ulong currentSize;
        ulong clearSize = default;

        int parallerSize = 4; // 병렬 동시 처리 요청 최대 개수 
        int completeCount = default;
        int progressDoneCount = default;

        float percentage;
        while (waitQueue.Count > 0 || progressList.Count > 0)
        {
            // 병렬 처리 요청 개수에 여유가 있다면 대기 큐에서 뽑아서 통신 시작
            if (progressList.Count < parallerSize && waitQueue.Count > 0)
            {
                var v = waitQueue.Dequeue();
                StartCoroutine(DownLoadRoutine(v.Item1));
                progressList.Add(v);
            }
            percentage = default;
            // 처리 진행 정보 확인 및 갱신
            for (int i = 0; i < progressList.Count; i++)
            {
                if (progressList[i].Item1.isDone)
                {
                    progressDoneCount++;
                    clearSize += progressList[i].Item1.downloadedBytes;
                    Manager.Resource.GetLoadBundle(progressList[i].Item2, DownloadHandlerAssetBundle.GetContent(progressList[i].Item1));
                    Message.Log($"DownloadClear : {progressList[i].Item2}");
                    progressList.RemoveAt(i);
                    completeCount++;
                }
                else
                {
                    percentage += progressList[i].Item1.downloadProgress;
                }
            }
            currentSize = clearSize;
            for (int i = 0; i < progressList.Count; i++)
            {
                currentSize += progressList[i].Item1.downloadedBytes;
            }
            _downloadUI.GetStateValue((percentage + (float)completeCount) / (float)downloadCount);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSecondsRealtime(1);
        _downloadUI.Complete();
    }

    /// <summary>
    /// 다운로드 목차 작성
    /// </summary>
    void MakeDownloadList(List<string> downloadBundleNameList, out List<string> downLoadArray)
    {
        if (downloadBundleNameList.Count == 0)
        {
            downLoadArray = null;
            return;
        }

        int checkCount = 0;
        downLoadArray = new();
        string[] bundleData;
        foreach (string downloadBundleName in downloadBundleNameList)
        {
            if (VersionTable.ContainsKey(downloadBundleName))
            {
                bundleData = VersionTable[downloadBundleName];
                checkCount++;
                //존재하는지 확인
                if (Caching.IsVersionCached(
                    ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(bundleData[(int)VersionTableColumn.Version])) == false)
                {
                    downLoadArray.Add(downloadBundleName);
                }
            }
        }

        if (downloadBundleNameList.Count != checkCount)
            Message.LogWarning($"WriteDownloadList _ Not Match Download List Count Check {downloadBundleNameList.Count} / {checkCount}");
        else
            Message.Log($"CheckCount : {downloadBundleNameList.Count} Download Count : {downLoadArray.Count}");
    }

    /// <summary>
    /// 다운로드 데이터 준비 및 다운로드 목록이 있으면 팝업창 출력
    /// </summary>
    IEnumerator DownloadPopUpCoreRoutine(Action call, List<string> downLoadArray)
    {
        Queue<(UnityWebRequest, string)> waitQueue = new();
        uint fetchSize = default;
        string[] bundleData;
        // 대기 큐 생성
        foreach (string bundleName in downLoadArray)
        {
            bundleData = VersionTable[bundleName];

            UnityWebRequest uwr = UnityWebRequest.Get(bundleData[(int)VersionTableColumn.DownloadLink]);
            UnityWebRequestAsyncOperation oper = uwr.SendWebRequest();

            oper.completed += (asyncOperation) =>
            {
                UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(
                    ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(bundleData[(int)VersionTableColumn.Version]),
                    0);

                waitQueue.Enqueue(new(tupleUwr, bundleName));
            };
        }

        while (waitQueue.Count != downLoadArray.Count)
        {
            yield return new WaitForFixedUpdate();
        }

        if (waitQueue.Count > 0)
        {
            OnDownloadPopup(waitQueue, downLoadArray, fetchSize);
        }

        call?.Invoke();
    }


    void OnDownloadPopup(Queue<(UnityWebRequest, string)> waitQueue, List<string> dowloadList, uint fetchSize)
    {
        //다운로드 팝업창 생성
        PopUpUI popUpUI = Manager.UI.ShowPopUpUI("CheckPopup");
        if (popUpUI == null)
            return;
        instancePopupUI = popUpUI as PlayerBeforeCheckPopup;
        instancePopupUI.Init();//팝업창 초기화
        SetText();

        instancePopupUI.InitAction(
        () =>
        {
            //모든 팝업 삭제
            Manager.UI.ClearPopUpUI();
            if (downloadCo != null)
                StopCoroutine(downloadCo);

            downloadCo = StartCoroutine(DownLoadProcessRoutine(waitQueue, dowloadList.Count, fetchSize));
            instancePopupUI = null;
        },
        () =>
        {
            Manager.UI.ClearPopUpUI();
            instancePopupUI = null;
        });
    }

   


    #region VersionTable

    /// <summary>
    /// 번들 버전 최신화
    /// </summary>
    public void LoadToServerVersion(Action<bool> refeshState = null, Action downloadAction = null)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Message.LogWarning("인터넷 끊김 ㄷㄷ");
            refeshState?.Invoke(false);
            return;
        }
        else
        {
            StartCoroutine(LoadServerVersionTableRoutine(refeshState, downloadAction));
        }
    }



    /// <summary>
    /// 버전 비교
    /// </summary>
    bool VersionCompleteCheck()
    {
        string key;
        foreach (KeyValuePair<string, string[]> bundleVersion in localVersionTable)
        {
            key = bundleVersion.Key;

            if (serverVersionTable.ContainsKey(key) &&
                bundleVersion.Value[(int)VersionTableColumn.Version] !=
                serverVersionTable[key][(int)VersionTableColumn.Version])
            {   continue;       }
            else
            {   return false;   }
        }
        return true;
    }

    /// <summary>
    /// 로컬 데이터 불러오기
    /// </summary>
    void LoadToLocalVersion()
    {
        if (System.IO.Directory.Exists($"{LocalVersionPath}{LOCALVERSIONURL}"))
        {
            string localVersion = System.IO.File.ReadAllText($"{LocalVersionPath}{LOCALVERSIONURL}");
            if (localVersion != null)
            {
                string[] rows = localVersion.Split('\n');
                int length = rows.Length;

                if (localVersionTable == null)
                    localVersionTable = new(length);
                else
                    localVersionTable.Clear();

                for (int i = 0; i < length; i++)
                {
                    string[] versionData = rows[i].Split(',');
                    localVersionTable.Add(versionData[(int)VersionTableColumn.FileName], versionData);
                }
            }
        }
        else
        {
            localVersionTable = null;
            Message.Log("로컬 데이터가 없습니다.");
        }
    }
    //서버 데이터 불러오기 
    IEnumerator LoadServerVersionTableRoutine(Action<bool> refeshState, Action call)
    {
        using UnityWebRequest uwr = UnityWebRequest.Get(SERVERVERSIONURL);
        yield return uwr.SendWebRequest();
       
        string[] rows = uwr.downloadHandler.text.Split('\n');
        int length = rows.Length;

        if (serverVersionTable == null)
            serverVersionTable = new();
        else
            serverVersionTable.Clear();

        for (int i = 0; i < length; i++)
        {
            string[] versionData = rows[i].Split(',');
            serverVersionTable.Add(versionData[(int)VersionTableColumn.FileName], versionData);
        }

        refeshState?.Invoke(serverVersionTable != null);
        call?.Invoke();
    }

    #endregion VersionTable


    #region Tools


    string ExtractSubstring(string inputString)
    {
        const string startMarker = "d/";
        const string endMarker = "/view";

        int startIndex = inputString.IndexOf(startMarker);
        if (startIndex == -1)
            return null;
        startIndex += startMarker.Length;
        int endIndex = inputString.IndexOf(endMarker, startIndex);
        if (endIndex == -1)
            return null;

        return $"{DOWNLOADPATH}{inputString[startIndex..endIndex]}";
    }

    ulong GetFreeDiskValue()
    {
        string driveLetter = Path.GetPathRoot(LocalVersionPath); // 유니티의 persistentDataPath가 위치한 드라이브의 문자 가져오기
        DriveInfo drive = new(driveLetter);
        return drive.IsReady ? (ulong)drive.AvailableFreeSpace : 0;
    }

    string FormatBytes(ulong bytes)
    {
        const int scale = 1024;
        string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
        ulong max = (ulong)Mathf.Pow(scale, orders.Length - 1);
        foreach (string order in orders)
        {
            if (bytes > max)
            {
                return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
            }
            max /= scale;
        }
        return "0 Bytes";
    }

    IEnumerator DownLoadRoutine(UnityWebRequest uwr)
    {
        yield return uwr.SendWebRequest();
    }
    #endregion Tools

    /// <summary>
    /// 권한이 없을 경우 권한 요청 , 경로 없을 경우 경로 생성
    /// </summary>
    private System.Collections.IEnumerator CreateDirectory(Action<Action<bool>, Action> call)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            int sdkVersion = 0;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
        sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
#endif
            Debug.Log("Android SDK Version: " + sdkVersion);

            // 안드로이드 13버전부터 sdkVersion >= 33 
            if (sdkVersion >= 33)
            {
                string[] permissions = { "android.permission.READ_MEDIA_VIDEO" };

                // 저장공간(Write) 권한 체크(선택 권한)
                if (Permission.HasUserAuthorizedPermission(permissions[0]) == false)
                {
                    // 권한 요청
                    Permission.RequestUserPermissions(permissions);

                    yield return new WaitForSeconds(1f);

                    yield return new WaitUntil(() => Application.isFocused == true);
                }
            }
            else // 안드로이드 13버전 이전
            {
                // 저장공간(Write) 권한 체크(선택 권한)
                if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) == false)
                {
                    // 권한 요청
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);

                    yield return new WaitForSeconds(1f);

                    yield return new WaitUntil(() => Application.isFocused == true);
                }
            }
        }
        
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        // 디렉토리가 존재하지 않는다면 생성
        //if (!Directory.Exists(LocalVersionPath))
        //{
        //    Directory.CreateDirectory(LocalVersionPath);
        //}
        call?.Invoke(null, null);
    }


    public enum VersionTableColumn // 버전 테이블 Column(열) 정보
    {
        FileName, // 번들 파일 명
        Version, // 번들 버전 정보
        DownloadLink, // 번들 설치 링크
    }
}
