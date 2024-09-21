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
    public enum LoadTable
    {
        NULL,
        Local,
        Server,
    }

    enum VersionTableColumn // 버전 테이블 Column(열) 정보
    {
        FileName, // 번들 파일 명
        Version, // 번들 버전 정보
        DownloadLink, // 번들 설치 링크
    }

    const string DOWNLOAD_PATH = "https://drive.google.com/uc?export=download&id=";
    //const string SERVER_VERSION_URL = ;  // 서버 버전 테이블 접속 URL
    const string LOCAL_VERSION_FILENAME = "/versionTable.csv"; // 로컬 버전 테이블 경로
    string LocalVersionPath { get { return Manager.Data.DataPath; } }
    const string ORIGIN_SERVER_URL = "https://docs.google.com/spreadsheets/d/1pu9z0RT1m9YmvsiVd7uOAaL8JTutDJIVMhLrMT68RuI";
    const string TEST_SERVER_URL = "https://docs.google.com/spreadsheets/d/1L_w-MwmMCwQ55ayl7H-CVtI6Vlie7AUQ0Lr_wkLNw4Y";
     string GetServerVersionURL {
        get
        {
            return $"{TEST_SERVER_URL}/export?format=csv";
        }
    }

    string titleText;
    string detailText;

    Dictionary<string, string[]> VersionTable   { get;  set; }
    

    PlayerBeforeCheckPopup instancePopupUI;
    [SerializeField] DownLoadUI downLoadUI;
    [field : SerializeField] public LoadTable CurrentTable { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        CurrentTable = LoadTable.NULL;
    }

    Coroutine checking;
    Coroutine downloadCo;

    private void OnDestroy()
    {
        this.StopCoroutine(checking);
    }

    private void Start()
    {
#if UNITY_EDITOR
        checking = StartCoroutine(CurrentLoadedBundle());
#endif
    }

    public string GetBundleURL(string bundleName)
    {
        Dictionary<string, string[]> verionDic = VersionTable;
        if (verionDic == null || verionDic.ContainsKey(bundleName) == false)
            return null;
        return ExtractSubstring(verionDic[bundleName][(int)VersionTableColumn.DownloadLink]);
    }
    /// <summary>
    /// 현재 로드되어있는 번들 콘솔창에 출력
    /// </summary>
    IEnumerator CurrentLoadedBundle()
    {
        StringBuilder sb = new();
        string write;

        while (true)
        {
            foreach (var item in AssetBundle.GetAllLoadedAssetBundles())
            {   sb.Append(item.name + ", ");    }

            write = sb.ToString();
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
        if (CurrentTable != LoadTable.NULL)
        {
            IsVersionCached(bundleFileName, stateCall);
            return;
        }

        LoadToServerVersion((refreshState) =>
        {
            if (refreshState)
            {
                IsVersionCached(bundleFileName, stateCall);
            }
            else
            {
                Message.LogError("버전 로드에서 실패함 : HasBundle");
                stateCall?.Invoke(false);
            }
        });
    }

    /// <summary>
    /// 해당 번들이 있는지 확인. 있다면 버전 확인
    /// </summary>
    void IsVersionCached(string bundleFileName, Action<bool> call)
    {
        if (VersionTable.ContainsKey(bundleFileName))
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
    void GetBundleRoutine(string bundleName, Action<AssetBundle> bundleCall)
    {
        string[] bundleData = VersionTable[bundleName];

        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(
            ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]),
            Hash128.Parse(bundleData[(int)VersionTableColumn.Version]));

        uwr.SendWebRequest().completed += (oper)=> 
        {
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                bundleCall?.Invoke(bundle);
            }
        };
    }


    /// <summary>
    /// 자동 다운로드
    /// </summary>
    IEnumerator DownloadBundle(string bundleName, Action<AssetBundle> callBundle = null)
    {
        int idx             = -1;
        string[] bundleData = VersionTable[bundleName];

        for (int i = 0; i < VersionTable.Count; i++)
        {
            if (string.Equals(bundleData[(int)VersionTableColumn.FileName], bundleName))
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
        string url = ExtractSubstring(bundleData[(int)VersionTableColumn.DownloadLink]);

        UnityWebRequest uwr = UnityWebRequest.Head(url);
        yield return uwr.SendWebRequest();


        //번들 로드
        UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Parse(bundleData[(int)VersionTableColumn.Version]));
        tupleUwr.SendWebRequest().completed +=(oper) =>
        {
            if (tupleUwr.result == UnityWebRequest.Result.Success)
                callBundle?.Invoke(DownloadHandlerAssetBundle.GetContent(tupleUwr));
            else
                callBundle?.Invoke(null);
        };

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
                    Message.LogError("DownLoad _ VersionRefresh refresh Error");

                return;
            });
        }

        DownLoadPopUpProcess(downloadFileNameList, call);
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
            Caching.ClearCachedVersion(
                pair.Item2, 
                Hash128.Parse(VersionTable[pair.Item2][(int)VersionTableColumn.Version]));

        List<(UnityWebRequest, string)> progressList = new();
        //다운로드 팝업 활성화
        DownLoadUI _downloadUI  = Manager.UI.ShowPopUpUI(downLoadUI);

        ulong currentSize       = default;
        ulong clearSize         = default;

        int completeCount       = default;
        int progressDoneCount   = default;
        int i                   = default;
        int parallerSize        = 4; // 병렬 동시 처리 요청 최대 개수 

        float percentage;

        UnityWebRequest request;
        AssetBundle     bundle;
        string          bundleName;
        while (waitQueue.Count > 0 || progressList.Count > 0)
        {
            // 병렬 처리 요청 개수에 여유가 있다면 대기 큐에서 뽑아서 요청
            if (progressList.Count < parallerSize && waitQueue.Count > 0)
            {
                (UnityWebRequest request, string bundleName) queueData = waitQueue.Dequeue();
                /*yield return */
                queueData.request.SendWebRequest();
                //StartCoroutine(DownLoadRoutine(v.Item1));
                progressList.Add(queueData);
            }
            percentage = default;
            // 처리 진행 정보 확인 및 갱신
            for (i = 0; i < progressList.Count; i++)
            {
                request = progressList[i].Item1;

                if (request.isDone)
                {
                    bundleName  = progressList[i].Item2;
                    bundle      = DownloadHandlerAssetBundle.GetContent(request);

                    Manager.Resource.GetLoadBundle(bundleName, bundle);
                    Message.Log($"DownloadClear : {bundleName}");

                    clearSize += request.downloadedBytes;
                    progressDoneCount++;
                    completeCount++;

                    progressList.RemoveAt(i);
                }
                else
                {
                    percentage += request.downloadProgress;
                }
            }

            currentSize = clearSize;

            for (i = 0; i < progressList.Count; i++)
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
        int count = downloadBundleNameList.Count;
        if (count == 0)
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

        if (count != checkCount)
            Message.LogWarning($"WriteDownloadList _ Not Match Download List Count Check {count} / {checkCount}");
        else
            Message.Log($"CheckCount : {count} Download Count : {downLoadArray.Count}");
    }

    /// <summary>
    /// 다운로드 데이터 준비 및 다운로드 목록이 있으면 팝업창 출력 요청
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

    /// <summary>
    /// 다운로드 팝업창 출력
    /// </summary>
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
    public void LoadToServerVersion(Action<bool> refeshState = null)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Message.LogWarning("DisConnected Internet");
            refeshState?.Invoke(false);
            return;
        }
        if (CurrentTable == LoadTable.Server)
        {
            refeshState?.Invoke(false);
            return;
        }
        UnityWebRequest uwr = UnityWebRequest.Get(GetServerVersionURL);
        uwr.SendWebRequest().completed += (oper) => { CompleteServerTable(uwr, refeshState); };
    }

    /// <summary>
    /// 버전 비교
    /// </summary>
    bool VersionDifferentCheck(List<string[]> serverVersionTableList)
    {
        string key;
        string[] bundleData;
        int count = serverVersionTableList.Count;

        if (VersionTable == null)
            return false;

        for (int i = 0; i < count; i++)
        {
            bundleData = serverVersionTableList[i];
            key = bundleData[(int)VersionTableColumn.FileName];

            if (VersionTable.ContainsKey(key) == false ||
                bundleData          [(int)VersionTableColumn.Version] !=
                VersionTable[key]   [(int)VersionTableColumn.Version])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 로컬 데이터 불러오기
    /// </summary>
    public bool LoadToLocalVersion()
    {
        bool returnValue = false;
        if (CurrentTable != LoadTable.NULL)
            return returnValue;

        if (System.IO.Directory.Exists($"{LocalVersionPath}") && 
            System.IO.File.Exists($"{LocalVersionPath}{LOCAL_VERSION_FILENAME}"))
        {
            string localVersion = System.IO.File.ReadAllText($"{LocalVersionPath}{LOCAL_VERSION_FILENAME}");
            if (localVersion != null)
            {
                string[] rows = localVersion.Split('\n');
                string[] versionData;
                int count = rows.Length;

                if (VersionTable == null)
                    VersionTable = new(count);
                else
                    VersionTable.Clear();

                for (int i = 0; i < count; i++)
                {
                    versionData = rows[i].Split(',');
                    VersionTable.Add(versionData[(int)VersionTableColumn.FileName], versionData);
                }
            }
            returnValue = true;
        }
        else
        {
            Message.Log("로컬 데이터가 없습니다.");
            LoadToServerVersion();
        }
        CurrentTable = LoadTable.Local;
        return returnValue;
    }

    /// <summary>
    /// 서버 데이터 불러오기 
    /// </summary>

    /// <summary>
    /// 서버 데이터와 현재 테이블 비교 및 저장 시도
    /// </summary>
    void CompleteServerTable(UnityWebRequest uwr, Action<bool> refeshState)
    {
        string[] rows = uwr.downloadHandler.text.Split('\n');
        string[] bundleData;

        int count = rows.Length;
        List<string[]> serverVersionTableList = new(count);

        for (int i = 0; i < count; i++)
        {
            bundleData = rows[i].Split(',');
            serverVersionTableList.Add(bundleData);
        }

        //로드 데이터 테이블과 차이점이 있으면 변경
        if (VersionDifferentCheck(serverVersionTableList) == false)
        {
            if (VersionTable != null)
                VersionTable.Clear();
            else
                VersionTable = new(count);

            for (int i = 0; i < count; i++)
            {
                bundleData = serverVersionTableList[i];
                VersionTable.Add(bundleData[(int)VersionTableColumn.FileName], bundleData);
            }

            byte[] data = uwr.downloadHandler.data;

            if (Directory.Exists(LocalVersionPath) == false)
                Directory.CreateDirectory(LocalVersionPath);

            FileStream fs = new FileStream($"{LocalVersionPath}{LOCAL_VERSION_FILENAME}", FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Dispose();
        }

        CurrentTable = LoadTable.Server;

        refeshState?.Invoke(true);
    }

    #endregion VersionTable

    #region 텍스트 언어 변경
    /// <summary>
    /// 다운로드 팝업창 출력
    /// </summary>
    void SetText()
    {
        BackendChartData.logChart.TryGetValue(171, out LogChartData logChartData);
        if (logChartData != null)
        {
            detailText = Manager.Text._Iskr ?
                logChartData.korean : logChartData.english;
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
            titleText = Manager.Text._Iskr ?
                logChartData2.korean : logChartData2.english;
        }
        instancePopupUI.InitText(titleText, detailText);
    }
    #endregion

    #region Tools


    string ExtractSubstring(string inputString)
    {
        const string startMarker    = "d/";
        const string endMarker      = "/view";

        int startIndex;
        int endIndex;

        startIndex = inputString.IndexOf(startMarker);
        if (startIndex == -1)
            return null;

        startIndex += startMarker.Length;
        endIndex = inputString.IndexOf(endMarker, startIndex);
        if (endIndex == -1)
            return null;

        return $"{DOWNLOAD_PATH}{inputString[startIndex..endIndex]}";
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
    
    #endregion Tools

    /// <summary>
    /// 권한이 없을 경우 권한 요청 , 경로 없을 경우 경로 생성
    /// </summary>
    private System.Collections.IEnumerator CreateDirectory()
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
    }



}
